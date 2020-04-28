using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Models.Exceptions;
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

        //-----------------------------------------Inserimento corso---------------------------------------------------

        //mostra il form
        public IActionResult Create()
        {
            ViewData ["Title"] = "Nuovo corso";
            var inputModel = new CourseCreateInputModel();

            return View(inputModel);      //ritorno una view contenente il form di creazione
        }


        //metodo che permette al model binding di riversare dentro l'istanza dell'oggetto i valori ricevuti dal form
        
        [HttpPost]  //eseguiamo il metodo SOLO quando la richiesta é post (in questo caso quando invio i dati)
        public async Task<IActionResult> Create(CourseCreateInputModel inputModel)
        {
            //verifica della validità dell'input (in base alle regole definite nel CourseCreateInputModel)
            if(ModelState.IsValid)
            {
                //titolo valido: creiamo il corso e se tutto va bene, reindirizziamo l'utente alla pagina di elenco
                try
                {
                    CourseDetailViewModel course = await courseService.CreateCourseAsync(inputModel);  //passo il titolo (ricevuto nell'oggetto CourseCreateInputModel)
                    //se la validazione non é corretta, restituisco di nuovo il form di inserimento
                    return RedirectToAction(nameof(Index));             //dopo aver eseguito l'azione, viene indirizzato alla pagina Index
                }
                catch (CourseTitleUnavailableException)
                {
                    //se si verifica l'exception: aggiungiamo l'errore che riguarda la prop. Title e gli assegnamo il messaggio
                    ModelState.AddModelError(nameof(CourseDetailViewModel.Title), "Questo titolo é già esistente e in uso");
                }
                
            }
            ViewData["Title"] = "Nuovo corso";
            return View(inputModel); 
        }

        public async Task<IActionResult> IsTitleAvailable(string title)
        {
            //il parametro title contiene ciò che ha digitato l'utente e lo fornisco al metodo IsTitleAvailableAsync
            bool result = await courseService.IsTitleAvailableAsync(title);
            
            //true se il titolo é disponibile (non esiste ancora nel db) altrimenti false
            return Json(result);    //il ritorno json di un booleano é true o false
        }
    }
}