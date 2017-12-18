using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfigSitios.Models
{
    public static class Servidores
    {
        public static Dictionary<string, string> ListaServidores;

        public static Dictionary<string, string> ObtenerServidores()
        {
            if(ListaServidores == null)
            {
                ListaServidores = new Dictionary<string, string>();

                ListaServidores.Add("rack1", "Profe109");
                ListaServidores.Add("rack2", "Profe109");
                ListaServidores.Add("rack3", "Profe109");
                ListaServidores.Add("desarrollo", "Program2016");
            }

            return ListaServidores;
        }
    }
}