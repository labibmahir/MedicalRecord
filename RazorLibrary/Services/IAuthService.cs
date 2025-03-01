﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorLibrary.Services
{
    public interface IAuthService
    {
        Task LoginAsync(string username, string password);
        Task LogoutAsync();
        Task<string> GetTokenAsync();
    }
}
