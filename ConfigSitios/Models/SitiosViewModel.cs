using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfigSitios.Models
{
    public class SitiosViewModel
    {
        public IEnumerable<string> Sitios { get; set; }

        public string sitio { get; set; }

        public Sitio DatosSitio { get; set; }
    }
}