using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RazorLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RazorLibrary
{
    public partial class Login
    {
        [Inject] NavigationManager Navigation { get; set; } 
        private string? username;
        private string? password;
        private string errorMessage;
        private bool isSubmitting = false;

        private string displayLoader;
        private string Redirect { get; set; }
        private bool IsUnauthorizedRedirect { get; set; }
        protected override void OnInitialized()
        {

            var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
       
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
    
            }
        }


        private async Task SubmitForm()
        {
            try
            {
                await AuthService.LoginAsync(username, password);
                Navigation.NavigateTo("/user");
            }
            catch (Exception ex)
            {
                Navigation.NavigateTo("/user");
            }
        }

        private bool showPassword = false;

        private void TogglePasswordVisibility()
        {
            showPassword = !showPassword;
        }


        private void ToggleClose()
        {
            errorMessage = "";
        }
    }
}
