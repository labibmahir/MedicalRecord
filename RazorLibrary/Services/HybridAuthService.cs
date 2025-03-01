using Domain.Dto;
using Domain.Entities;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json.Linq;
using RazorLibrary.HttpServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using System.Text.Json;
namespace RazorLibrary.Services
{
    public class HybridAuthService(LoginHttpService loginHttpService) : AuthenticationStateProvider, IAuthService
    {
        private readonly LoginHttpService _loginHttpService = loginHttpService;

        public async Task LoginAsync(string username, string password)
        {
            UserLoginDto userLoginDto = new UserLoginDto()
            {
                Username = username,
                Password = password,
            };
            UserAccount userDetails = new UserAccount();
            var responseOutCome = await loginHttpService.Login(userLoginDto);

            if (responseOutCome.ResponseStatus == ResponseStatus.Success)
            {
                userDetails = responseOutCome.Entity;
                userDetails.Username = userLoginDto.Username;
                string userDetailsJson = JsonSerializer.Serialize(userDetails);

                // Store JSON string in SecureStorage
                await SecureStorage.SetAsync("userDetails", userDetailsJson);
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

            }


        }

        public async Task LogoutAsync()
        {
            SecureStorage.Remove("userDetails");
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
        public async Task<UserAccount> GetUserDetailsAsync()
        {
            string userDetailsJson = await SecureStorage.GetAsync("userDetails");

            if (!string.IsNullOrEmpty(userDetailsJson))
            {
                return JsonSerializer.Deserialize<UserAccount>(userDetailsJson);
            }

            return null;
        }
        public async Task<string> GetTokenAsync()
        {
            return await SecureStorage.GetAsync("userDetails") ?? string.Empty;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await GetTokenAsync();
            var identity = string.IsNullOrEmpty(token) ? new ClaimsIdentity() : new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string token)
        {
            var payload = token.Split('.')[1];
            var jsonBytes = Convert.FromBase64String(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }
    }
}
