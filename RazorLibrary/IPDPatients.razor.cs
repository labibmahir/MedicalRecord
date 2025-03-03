using Domain.Entities;
using RazorLibrary.HttpServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorLibrary
{
    public partial class IPDPatients
    {
        private List<IPDPatient> IPDPatientsList = new();
        private List<IPDPatient> Patients = new();
        private IPDPatient newPatient = new();
        List<Guid> PatientIds = new List<Guid>();
        private bool isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            await LoadPatients();
        }

        private async Task LoadPatients()
        {
            isLoading = true;
            for (var i = 0; i <= 5; i++)
            {
                Guid patientId = Guid.NewGuid();
                PatientIds.Add(patientId);

            }
            //  IPDPatients = await IPDPatientService.GetAllPatientsAsync();
            //Patients = IPDPatientsList; // For dropdown, replace with real patients if needed.
            isLoading = false;
            var response = await IPDPatientService.ReadIPDPatients();
            if (response.ResponseStatus == ResponseStatus.Success)
            {
                IPDPatientsList = response.Entity;
            }
        }

        private async Task HandleSubmit()
        {
            newPatient.Oid = Guid.NewGuid();
            await IPDPatientService.Create(newPatient);
            await LoadPatients();
        }
    }
}
