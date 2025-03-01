using Domain.Entities;
using Microsoft.AspNetCore.Components;
using RazorLibrary.HttpServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorLibrary
{
    public partial class UserAccounts
    {
        [Inject] UserAccountHttpService userService { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        public  UserAccount userAccount = new UserAccount();

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
        private async Task HandleValidSubmit()
        {
            await userService.CreateUserAccount(userAccount);
        }
    }
}
