using Microsoft.AspNetCore.Mvc;

namespace MyCourse.Controllers
{
    public class CoursesController : Controller
    {
        public IActionResult Index()        //Courses
        {
<<<<<<< HEAD
            return View();      //va a cercare una view chiamata Index.cshtml in courses
=======
            return View();
>>>>>>> Sezione07
        }

        public IActionResult Detail(string id)  //Detail/5
        {
            return View();
        }
    }
}