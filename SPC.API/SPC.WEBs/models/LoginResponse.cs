using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPC.Web.Models;

namespace SPC.WEBs.models
{
    public class LoginResponse
    {
        public string Message { get; set; }
        public Pharmacy Pharmacy { get; set; }
    }
}