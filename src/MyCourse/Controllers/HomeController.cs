using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Models.Services.Application;
using MyCourse.Models.ViewModels;

namespace MyCourse.Controllers {

    //l'attributo avrebbe effetto su tutte le action del controller
    //[ResponseCache(CacheProfileName = "Home")]

    public class HomeController : Controller {
        //permetterà la visualizzazione dei corsi maggiormente piaciuti

        //con [FromServices] permette di dare la giusta indicazione al model binder, che
        //l'istanza ICachedCourseService deve essere cercata tra i servizi registrati per la dependency injection
        public async Task<IActionResult> Index ([FromServices] ICachedCourseService courseService) {
            ViewData["Title"] = "Benvenuto su MyCourse";

            //otteniamo solo una lista, cioé l'elenco di tutti i corsi (ma limitato al dato inserito in appsettings.json)
            List<CourseViewModel> bestRatingCourses = await courseService.GetBestRatingCoursesAsync ();
            List<CourseViewModel> mostRecentCourses = await courseService.GetMostRecentCoursesAsync ();

            HomeViewModel viewModel = new HomeViewModel {
                BestRatingCourses = bestRatingCourses,
                MostRecentCourses = mostRecentCourses
            };

            return View (viewModel);
        }
    }
}