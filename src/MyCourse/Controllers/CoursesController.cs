using Microsoft.AspNetCore.Mvc;

namespace MyCourse.Controllers
{
    public class CoursesController : Controller
    {
        public IActionResult Index()        //Courses
        {
            return View();
        }

        public IActionResult Detail(string id)  //Detail/5
        {
            return View();
        }
    }
}