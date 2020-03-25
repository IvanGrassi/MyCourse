using Microsoft.AspNetCore.Mvc;

namespace MyCourse.Controllers
{
    public class CoursesController : Controller
    {
        public IActionResult Index()        //Courses
        {
            return Content("Sono index");
        }

        public IActionResult Detail(string id)  //Detail/5
        {
            return Content($"Sono detail, ho ricevuto l'id {id}");
        }
    }
}