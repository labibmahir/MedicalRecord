using Domain.Entities;
using RazorLibrary.HttpServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorLibrary
{
    public partial class OPDPatients
    {
        private List<OPDPatient> OPDPatientsList = new();
        private List<OPDPatient> Patients = new();
        private OPDPatient newPatient = new();
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
            //  OPDPatients = await OPDPatientService.GetAllPatientsAsync();
            //Patients = OPDPatientsList; // For dropdown, replace with real patients if needed.
            isLoading = false;
            var response = await OPDPatientService.ReadOPDPatients();
            if (response.ResponseStatus == ResponseStatus.Success)
            {
                OPDPatientsList = response.Entity;
            }
        }

        private async Task HandleSubmit()
        {
            newPatient.Oid = Guid.NewGuid();
            await OPDPatientService.Create(newPatient);
            await LoadPatients();
        }
    }
}
