using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using webBinh.Models;

namespace webBinh.LoginReponse
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public nguoiDung User { get; set; }
    }
}