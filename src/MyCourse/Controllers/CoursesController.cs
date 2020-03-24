using Microsoft.AspNetCore.Mvc;

namespace MyCourse.Controllers
{
    public class CoursesController : Controller
    {
        public IActionResult Index()
        {
            return Content("Sono index");
        }

        public IActionResult Detail(string id)
        {
            return Content($"Sono detail, ho ricevuto l'id {id}");
        }
    }
}