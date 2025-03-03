using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace RazorLibrary.HttpServices
{
   public class OPDPatientHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUri;
        private readonly string _publicKey;

        public OPDPatientHttpService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            // Get values from appsettings.json (from main project)

            _baseUri = configuration["BaseUri"] ?? "";
            //_publicKey = configuration["PublicKey"] ?? throw new ArgumentNullException("Public Key is missing.");

            // Configure HttpClient
            if (_httpClient.BaseAddress is null)
                _httpClient.BaseAddress = new Uri(_baseUri);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_publicKey}");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        #region IPD Create 
        public async Task<ResponseOutcome<OPDPatient>> Create(OPDPatient OPDPatient)
        {
            ResponseOutcome<OPDPatient> outcome = new ResponseOutcome<OPDPatient>();

            try
            {
                using (HttpResponseMessage response = await _httpClient.PostAsJsonAsync("OPDPatient", OPDPatient))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        outcome.ResponseStatus = ResponseStatus.Success;
                        outcome.Entity = await response.Content.ReadFromJsonAsync<OPDPatient>();
                    }
                    else
                    {
                        outcome.Message = JsonConvert.DeserializeObject<string>(await response.Content.ReadAsStringAsync());
                    }
                }
            }
            catch (Exception ex)
            {
                outcome.Message = ex.Message;
            }

            return outcome;
        }
        #endregion

        #region IPD region
        public async Task<ResponseOutcome<List<OPDPatient>>> ReadOPDPatients()
        {


            var response = await _httpClient.GetAsync("OPDPatient");
            var outcome = new ResponseOutcome<List<OPDPatient>>();

            if (response.IsSuccessStatusCode)
            {
                outcome.ResponseStatus = ResponseStatus.Success;
                outcome.Entity = await response.Content.ReadFromJsonAsync<List<OPDPatient>>();
            }
            else
            {
                outcome.Message = await response.Content.ReadAsStringAsync();
            }

            return outcome;
        }
        #endregion
    }
}
