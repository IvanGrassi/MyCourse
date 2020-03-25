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
>>>>>>> 745fce1... Aggiunta view di contenuto e una view di layout
        }

        public IActionResult Detail(string id)  //Detail/5
        {
            return View();
        }
    }
}