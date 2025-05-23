﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTAuthenticationManager
{
    public class AuthenticationResponse
    {
        public string Username { get; set; }

        public string JwtToken { get; set; }

        public int ExpireIn { get; set; }
    }
}
