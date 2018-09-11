using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OdeToFood.Controllers
{
    [Route("company/[controller]/[action]")]
    public class AboutController
    {
        [Route("secret")]
        public string Phone()
        {
            return "+91 9876543210"; // accessible by going to url - company/about/phone/secret
        }
        public string Address()
        {
            return "+91 9876543210";
        }
    }
}
