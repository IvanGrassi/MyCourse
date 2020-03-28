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
            ViewData["Title"] = "Catalogo dei corsi";   //titolo statico

            //creiamo un istanza di CourseService, invochiamo il suo metodo GetCourses per ottenere l'elenco dei corsi
            var courseService = new CourseService();
            List<CourseViewModel> courses = courseService.GetCourses();
            return View(courses);      //va a cercare una view chiamata Index.cshtml in courses e gli passa i dati ottenuti tramite metodo
        }

        public IActionResult Detail(int id)         //Detail/5
        {
            var courseService = new CourseService();
            CourseDetailViewModel viewModel = courseService.GetCourse(id);          //attenzione, con questo metodo ottengo il dettaglio di un solo corso

            ViewData["Title"] = viewModel.Title;    //Action: viewModel.Title = 5 (Esempio) e carico il suo titolo
            return View(viewModel);
        }
    }
}