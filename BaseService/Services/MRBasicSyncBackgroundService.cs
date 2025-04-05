using Domain.MongoEntities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace BaseService.Services
{
    public class MRBasicSyncBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMongoCollection<ReadPatient> mongoCollection;

        public MRBasicSyncBackgroundService(IServiceScopeFactory serviceScopeFactory, IMongoCollection<ReadPatient> mongoCollection)
        {
            _serviceScopeFactory = serviceScopeFactory;
            this.mongoCollection = mongoCollection;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await SyncPatientsAsync();
                await Task.Delay(1000, stoppingToken); // Delay of 1 second
            }
        }

        private async Task SyncPatientsAsync()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                // Step 1: Fetch patients from MSSQL (LINQ is correct here for MSSQL)
                var mssqlPatients = await dataContext.Patients
                    .Where(p => !p.IsSynced)
                    .ToListAsync();  // Fetch data from MSSQL

                foreach (var patient in mssqlPatients)
                {
                    // Step 2: Query MongoDB for the patient using MongoDB's Find method
                    var mongoPatient = await mongoCollection
                        .Find(p => p.Oid == patient.Oid)  // Correct MongoDB query method
                        .FirstOrDefaultAsync();           // Get first matching patient or null

                    if (mongoPatient == null)
                    {
                        // Patient not found in MongoDB, insert and update MSSQL
                        var newMongoPatient = new ReadPatient
                        {
                            Oid = patient.Oid,
                            FirstName = patient.FirstName,
                            Surname = patient.Surname,
                            Age = patient.Age,
                            DateCreated = patient.DateCreated,
                            DateModified = patient.DateModified
                           
                        };

                        await mongoCollection.InsertOneAsync(newMongoPatient); // Insert patient in MongoDB

                        patient.IsSynced = true; // Mark as synced in MSSQL
                        await dataContext.SaveChangesAsync(); // Save changes in MSSQL
                    }
                    else
                    {
                        // Patient found in MongoDB, update if necessary
                        if (!patient.IsSynced)
                        {
                            mongoPatient.FirstName = patient.FirstName;
                            mongoPatient.Surname = patient.Surname;
                            mongoPatient.Age = patient.Age;
                            mongoPatient.DateCreated = patient.DateCreated;
                            mongoPatient.DateModified = patient.DateModified;

                            var filter = Builders<ReadPatient>.Filter.Eq(p => p.Oid, mongoPatient.Oid);
                            await mongoCollection.ReplaceOneAsync(filter, mongoPatient); // Update patient in MongoDB

                            patient.IsSynced = true; // Mark as synced in MSSQL
                            await dataContext.SaveChangesAsync(); // Save changes in MSSQL
                        }
                    }
                }

                // Step 3: Check for patients in MongoDB that don't exist in MSSQL
                var mongoPatients = await mongoCollection
                    .Find(p => true)  // Correct MongoDB query to find all documents
                    .ToListAsync();   // Get all patients from MongoDB

                foreach (var mongoPatient in mongoPatients)
                {
                    var patientInMssql = await dataContext.Patients
                        .FirstOrDefaultAsync(p => p.Oid == mongoPatient.Oid);  // Find corresponding patient in MSSQL

                    if (patientInMssql == null)
                    {
                        // Patient exists in MongoDB but not in MSSQL, delete from MongoDB
                        var filter = Builders<ReadPatient>.Filter.Eq(p => p.Oid, mongoPatient.Oid);
                        await mongoCollection.DeleteOneAsync(filter); // Delete from MongoDB
                    }
                }
            }
        }
    }
}