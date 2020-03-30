using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Models.Services.Application;
using MyCourse.Models.ViewModels;

namespace MyCourse.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ICourseServices courseService;
        public CoursesController(ICourseServices courseService)   //dipendenza del CoursesController dal Course service (senza questo non potrebbe funzionare)
        {
            this.courseService = courseService;
        }
        public IActionResult Index()        //Courses
        {
            ViewData["Title"] = "Catalogo dei corsi";   //titolo statico

            //invochiamo il suo metodo GetCourses per ottenere l'elenco dei corsi
            List<CourseViewModel> courses = courseService.GetCourses();
            return View(courses);      //va a cercare una view chiamata Index.cshtml in courses e gli passa i dati ottenuti tramite metodo
        }

        public IActionResult Detail(int id)         //Detail/5
        {
            CourseDetailViewModel viewModel = courseService.GetCourse(id);          //attenzione, con questo metodo ottengo il dettaglio di un solo corso

            ViewData["Title"] = viewModel.Title;    //Action: viewModel.Title = 5 (Esempio) e carico il suo titolo
            return View(viewModel);
        }
    }
}