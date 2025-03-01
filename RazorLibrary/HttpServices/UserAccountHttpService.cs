using Domain.Dto;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace RazorLibrary.HttpServices
{
    public class UserAccountHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUri;
        private readonly string _publicKey;

        public UserAccountHttpService(HttpClient httpClient, IConfiguration configuration)
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
        #region Create
        public async Task<ResponseOutcome<UserAccount>> CreateUserAccount(UserAccount userAccount)
        {
            ResponseOutcome<UserAccount> outcome = new ResponseOutcome<UserAccount>();
            try
            {
                using (HttpResponseMessage response = await _httpClient.PostAsJsonAsync("UserAccount/user", userAccount))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        outcome.ResponseStatus = ResponseStatus.Success;
                        outcome.Entity = await response.Content.ReadFromJsonAsync<UserAccount>();
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

    }
}
