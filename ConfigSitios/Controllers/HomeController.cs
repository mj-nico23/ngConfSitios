using ConfigSitios.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ConfigSitios.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var ss = Sitio.RecuperarSitios();

            HttpContext.Session["sitios"] = ss;

            var sitios = new SitiosViewModel
            {
                Sitios = ss
            };

            return View(sitios);
        }

        [HttpPost]
        public ActionResult Index(SitiosViewModel model)
        {
            model.Sitios = HttpContext.Session["sitios"] as List<string>;

            if (!string.IsNullOrWhiteSpace(model.sitio))
                model.DatosSitio = new Sitio(model.sitio);

            return View("index", model);
        }

        [HttpPost]
        public ActionResult Arreglar(SitiosViewModel model)
        {
            model.Sitios = HttpContext.Session["sitios"] as List<string>;

            if (!string.IsNullOrWhiteSpace(model.sitio))
            {
                Sitio sitio_arregaldo = new Sitio(model.sitio);
                sitio_arregaldo.ArrelgarINI();

                model.DatosSitio = sitio_arregaldo;
            }

            return View("index", model);
        }

    }
}