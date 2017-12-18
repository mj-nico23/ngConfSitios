using IniParser;
using IniParser.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Data;

namespace ConfigSitios.Models
{
    public class Sitio
    {
        public string NombreSitio { get; set; }
        public string INI_Servidor { get; set; }
        public string INI_BD { get; set; }
        public string INI_Tmp { get; set; }

        public string ODBC_Name { get; set; }
        public string ODBC_Servidor { get; set; }
        public string ODBC_BD { get; set; }

        public string BD_Tmp { get; set; }
        public string BD_Virtual { get; set; }

        public bool INI { get; set; }
        public bool ODBC { get; set; }
        public bool BD { get; set; }

        IniData data;
        FileIniDataParser parser;
        string ini_name;

        public Sitio(string name)
        {
            NombreSitio = name;

            RecuperarDatosSitio();
        }

        private void RecuperarDatosSitio()
        {
            if (string.IsNullOrWhiteSpace(NombreSitio))
                return;

            LeerINI();

            LeerODBC();

            LeerBD();

            ValidarINI();

            ValidarODBC();

            ValidarBD();
        }

        private void LeerINI()
        {
            ini_name = NombreSitio;
            if (NombreSitio == "pgmv6")
                ini_name = "pgm";

            parser = new FileIniDataParser();
            data = parser.ReadFile($@"C:\Windows\{ini_name}.ini", System.Text.Encoding.UTF8);

            INI_Servidor = data["Config"]["SERVIDOR_SQL"].ToUpper();
            INI_BD = data["Config"]["BASE_SQL"].ToUpper();
            INI_Tmp = data["MSSQL"]["PATH_TXT"];
            ODBC_Name = data["DSNUSR"]["General"];
        }

        private void ValidarINI()
        {
            string[] tmp = INI_Tmp.Split("\\".ToCharArray());

            string SitioAux = tmp[tmp.Length - 2].Trim().ToLower();

            INI = NombreSitio == SitioAux;
        }

        private void ValidarODBC()
        {
            ODBC = INI_Servidor == ODBC_Servidor && INI_BD == ODBC_BD;
        }

        private void ValidarBD()
        {
            BD = INI_Tmp.ToLower().TrimEnd('\\') == BD_Tmp.ToLower().TrimEnd('\\');
        }

        private void LeerODBC()
        {
            if (string.IsNullOrWhiteSpace(ODBC_Name))
                return;

            Tuple<string, string> odbc = ObtenerDatosOdbc(ODBC_Name);
            ODBC_Servidor = odbc.Item1;
            ODBC_BD = odbc.Item2;
        }

        private void LeerBD()
        {
            if (string.IsNullOrWhiteSpace(INI_Servidor) || string.IsNullOrWhiteSpace(INI_BD))
                return;

            BD_Tmp = ObtenerConfigDB("XXPFTMP");
            BD_Virtual = ObtenerConfigDB("XXPFVIRT");
        }

        public void ArrelgarINI()
        {
            data["MSSQL"]["PATH_TXT"] = $@"C:\Inetpub\wwwroot\{NombreSitio}\tmp";
            parser.WriteFile($@"C:\Windows\{ini_name}.ini", data, System.Text.Encoding.UTF8);

            LeerINI();
            ValidarINI();
        }

        public static List<string> RecuperarSitios()
        {
            List<string> sitios = new List<string>();
            var sitios_dir = Directory.GetDirectories(@"C:\inetpub\wwwroot", "pgmv6*");

            foreach (var item in sitios_dir)
            {
                string[] aux = item.Split("\\".ToCharArray());

                sitios.Add(aux[3].ToLower());
            }

            return sitios;
        }

        private Tuple<string, string> ObtenerDatosOdbc(string dsn)
        {
            string s = "", b = "";

            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Software\ODBC\ODBC.INI\" + dsn);
            if (regKey != null)
            {
                foreach (string name in regKey.GetValueNames())
                {
                    if(name.ToLower() == "server")
                        s = regKey.GetValue(name, "").ToString().ToUpper().Trim();
                    if (name.ToLower() == "database")
                        b = regKey.GetValue(name, "").ToString().ToUpper().Trim();
                }
            }

            return new Tuple<string, string>(s, b);
        }

        private string ObtenerConfigDB(string codigo)
        {
            string clave = "";
            Servidores.ObtenerServidores().TryGetValue(INI_Servidor.ToLower(), out clave);

            string sql = $"SELECT CONCEPTO FROM dbo.GLOBAL WHERE CODIGO = '{codigo}'";
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection($"Server={INI_Servidor};Database={INI_BD};User Id=sa;Password={clave};"))
            {
                using(SqlDataAdapter adap = new SqlDataAdapter(sql, conn))
                {
                    adap.Fill(dt);

                    if (dt.Rows.Count > 0)
                        return dt.Rows[0][0].ToString().Trim();
                }
            }

            return "";
        }
    }
}