using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPC.Web.Models;

namespace SPC.WEBs.models
{
    public class LoginResponseSupplier
    {
        public Supplier Supplier { get; set; }
        public string Message { get; set; }
    }
}