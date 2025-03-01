using Blazored.LocalStorage;
using Domain.Dto;
using Domain.Entities;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using RazorLibrary.HttpServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RazorLibrary.Services
{
    public class WebAuthService(LoginHttpService loginHttpService, ILocalStorageService localStorage, IJSRuntime jsRuntime)
        : AuthenticationStateProvider, IAuthService
    {
        private readonly LoginHttpService _loginHttpService = loginHttpService;
        private readonly ILocalStorageService _localStorage = localStorage;
        private readonly IJSRuntime _jsRuntime = jsRuntime;

  
        public async Task<(bool isSuccess, string message)> Login(UserLoginDto userLoginDto)
        {
            UserAccount userDetails = new UserAccount();
            var responseOutCome = await loginHttpService.Login(userLoginDto);

            if (responseOutCome.ResponseStatus == ResponseStatus.Success)
            {
                userDetails = responseOutCome.Entity;
                userDetails.Username = userLoginDto.Username;
                await MarkUserAsAuthenticated(userDetails);

                return (true, responseOutCome.Message);
            }

            return (false, responseOutCome.Message);
        }
        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task<string> GetTokenAsync()
        {
            return await _localStorage.GetItemAsync<string>("authToken") ?? string.Empty;
        }
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
                await MarkUserAsAuthenticated(userDetails);

             }

        }
        private async Task StoreAuthenticationInformation(UserAccount userDetails)
        {
            await localStorage.SetItemAsync("userDetails", userDetails);
            await localStorage.SetItemAsync("lastActivityTime", DateTime.Now.ToString());
        }
        public async Task MarkUserAsAuthenticated(UserAccount userDetails)
        {
          
            await StoreAuthenticationInformation(userDetails);
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userDetails.Username), new Claim("FirstName", userDetails.FirstName)
                , new Claim("SurName", userDetails.Username), new Claim("Email", userDetails.Email), new Claim("Username", userDetails.Username.ToString()) }, "MRAuthentication");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var authenticationState = Task.FromResult(new AuthenticationState(claimsPrincipal));
            NotifyAuthenticationStateChanged(authenticationState);
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
