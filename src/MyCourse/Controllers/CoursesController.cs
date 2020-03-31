using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Models.Services.Application;
using MyCourse.Models.ViewModels;

namespace MyCourse.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ICourseService courseService;
        public CoursesController(ICourseService courseService)   //dipendenza del CoursesController dal Course service (senza questo non potrebbe funzionare)
        {
            this.courseService = courseService;
        }
        public async Task<IActionResult> Index()        //Courses
        {
            ViewData["Title"] = "Catalogo dei corsi";   //titolo statico

            //invochiamo il suo metodo GetCourses per ottenere l'elenco dei corsi
            List<CourseViewModel> courses = await courseService.GetCoursesAsync();
            return View(courses);      //va a cercare una view chiamata Index.cshtml in courses e gli passa i dati ottenuti tramite metodo
        }

        public async Task<IActionResult> DetailAsync(int id)         //Detail/5
        {
            CourseDetailViewModel viewModel = await courseService.GetCourseAsync(id);          //attenzione, con questo metodo ottengo il dettaglio di un solo corso

            ViewData["Title"] = viewModel.Title;    //Action: viewModel.Title = 5 (Esempio) e carico il suo titolo
            return View(viewModel);
        }
    }
}