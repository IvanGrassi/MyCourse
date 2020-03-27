using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Models.Services.Application;
using MyCourse.Models.ViewModels;

namespace MyCourse.Controllers
{
    public class CoursesController : Controller
    {
        public IActionResult Index()        //Courses
        {
            //creiamo un istanza di CourseService, invochiamo il suo metodo GetCourses per ottenere l'elenco dei corsi
            var courseService = new CourseService();
            List<CourseViewModel> courses = courseService.GetServices();
            return View(courses);      //va a cercare una view chiamata Index.cshtml in courses e gli passa i dati ottenuti tramite metodo
        }

        public IActionResult Detail(string id)  //Detail/5
        {
            return View();
        }
    }
}