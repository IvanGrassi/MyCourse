using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Models.InputModels;
using MyCourse.Models.Services.Application;
using MyCourse.Models.ViewModels;

namespace MyCourse.Controllers
{
    public class CoursesController : Controller
    {
        //Gestisce le richieste degli utenti che vogliono visualizzare l'elenco o il dettaglio di un corso

        private readonly ICourseService courseService;
        public CoursesController(ICachedCourseService courseService) //dipendenza del CoursesController dal Course service (senza questo non potrebbe funzionare)
        {
            this.courseService = courseService;
        }
        public async Task<IActionResult> Index(CourseListInputModel input) //Courses
        {
            ViewData["Title"] = "Catalogo dei corsi";   //titolo statico

            //invochiamo il suo metodo GetCourses per ottenere l'elenco dei corsi dal CourseViewModel
            ListViewModel<CourseViewModel> courses = await courseService.GetCoursesAsync(input);

            CourseListViewModel viewModel = new CourseListViewModel
            {
                Courses = courses,                      //valorizziamo i dati come ci sono stati restituiti dal servizio applicativo (contiene i risultati)
                Input = input                           //l'input ricevuto dall'utente arriverà anche alla view
            };

            return View(viewModel); //va a cercare una view chiamata Index.cshtml in courses e gli passa i dati ottenuti tramite metodo
        }

        public async Task<IActionResult> DetailAsync(int id) //Detail/5
        {
            CourseDetailViewModel viewModel = await courseService.GetCourseAsync(id); //attenzione, con questo metodo ottengo il dettaglio di un solo corso

            ViewData["Title"] = viewModel.Title;        //Action: viewModel.Title = 5 (Esempio) e carico il suo titolo
            return View(viewModel);
        }
    }
}