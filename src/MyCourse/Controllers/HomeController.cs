using Microsoft.AspNetCore.Mvc;

namespace MyCourse.Controllers
{

    //l'attributo avrebbe effetto su tutte le action del controller
    //[ResponseCache(CacheProfileName = "Home")]

    public class HomeController : Controller
    {
        //permetterà la visualizzazione dei corsi maggiormente piaciuti

        [ResponseCache(CacheProfileName = "Home")]         //l'output della view può essere messo in cache ma lo limitiamo solo al browser (.Client)
        public IActionResult Index()
        {
            ViewData["Title"] = "Benvenuto su MyCourse";
            return View();
        }
    }
}