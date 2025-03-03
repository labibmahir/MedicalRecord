using Domain.Dto;
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
    public class IPDPatientHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUri;
        private readonly string _publicKey;

        public IPDPatientHttpService(HttpClient httpClient, IConfiguration configuration)
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
        public async Task<ResponseOutcome<IPDPatient>> Create(IPDPatient iPDPatient)
        {
            ResponseOutcome<IPDPatient> outcome = new ResponseOutcome<IPDPatient>();

            try
            {
                using (HttpResponseMessage response = await _httpClient.PostAsJsonAsync("IPDPatient", iPDPatient))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        outcome.ResponseStatus = ResponseStatus.Success;
                        outcome.Entity = await response.Content.ReadFromJsonAsync<IPDPatient>();
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
        public async Task<ResponseOutcome<List<IPDPatient>>> ReadIPDPatients()
        {
            

            var response = await _httpClient.GetAsync("IPDPatient");
            var outcome = new ResponseOutcome<List<IPDPatient>>();

            if (response.IsSuccessStatusCode)
            {
                outcome.ResponseStatus = ResponseStatus.Success;
                outcome.Entity = await response.Content.ReadFromJsonAsync<List<IPDPatient>>();
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
