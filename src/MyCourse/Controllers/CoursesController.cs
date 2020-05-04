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

        public async Task<IActionResult> Detail(int id) //Detail/5
        {
            CourseDetailViewModel viewModel = await courseService.GetCourseAsync(id); //attenzione, con questo metodo ottengo il dettaglio di un solo corso

            ViewData["Title"] = viewModel.Title;        //Action: viewModel.Title = 5 (Esempio) e carico il suo titolo
            return View(viewModel);                      //riceve l'oggetto popolato con tutti i dati
        }

        //-----------------------------------------Inserimento corso---------------------------------------------------

        //mostra il form
        public IActionResult Create()       //visualizza il form
        {
            ViewData ["Title"] = "Nuovo corso";
            var inputModel = new CourseCreateInputModel();

            return View(inputModel);        //ritorno una view contenente l'istanza del form di creazione
        }


        //metodo che permette al model binding di riversare dentro l'istanza dell'oggetto i valori ricevuti dal form
        [HttpPost]  //eseguiamo il metodo SOLO quando la richiesta é post (quando invio i dati)
        public async Task<IActionResult> Create(CourseCreateInputModel inputModel)      //ricevo l'istanza riempita con tutti i dati
        {
            //verifica della validità dell'input (in base alle regole definite nel CourseCreateInputModel)
            if(ModelState.IsValid)
            {
                //titolo valido: creiamo il corso e se tutto va bene, reindirizziamo l'utente alla pagina di elenco
                try
                {
                    CourseDetailViewModel course = await courseService.CreateCourseAsync(inputModel);  //passo il titolo (ricevuto nell'oggetto CourseCreateInputModel)
                    TempData["ConfirmationMessage"] = "Il corso é stato creato correttamente, ora hai la possibilità di inserire gli altri dati";
                    //se la validazione non é corretta, restituisco di nuovo il form di inserimento
                    return RedirectToAction(nameof(Edit), new { id = course.Id });             
                    //dopo aver eseguito l'azione, viene indirizzato alla pagina Index
                }
                catch (CourseTitleUnavailableException)
                {
                    //se si verifica l'exception: aggiungiamo l'errore al ModelState e la segnaliamo al docente
                    ModelState.AddModelError(nameof(CourseDetailViewModel.Title), "Questo titolo é già esistente e in uso");
                }
                
            }
            ViewData["Title"] = "Nuovo corso";
            return View(inputModel);   //riceve l'oggetto popolato con tutti i dati
        }

        //attenzione! per la validazione uso anche l'id perché altrimenti il titolo verrebbe considerato duplicato anche se assegnato a quell'id
        //(se modifico un prezzo ad esempio e mantengo il titolo uguale, solo con il title mi darebbe erreore di duplicazione )
        public async Task<IActionResult> IsTitleAvailable(string title, int id = 0)
        {
            //il parametro title contiene ciò che ha digitato l'utente e lo fornisco al metodo IsTitleAvailableAsync
            //l'id rimane fisso e SEMPRE assegnato a quel corso
            bool result = await courseService.IsTitleAvailableAsync(title, id);
            
            //true se il titolo é disponibile (non esiste ancora nel db) altrimenti false
            return Json(result);    //il ritorno json di un booleano é true o false
        }


        //----------------------------------------Modifica corso-------------------------------------------------------
    
        public async Task<IActionResult> Edit(int id)       //visualizza il form
        {
            ViewData["Title"] = "Modifica corso";
            CourseEditInputModel inputModel = await courseService.GetCourseForEditingAsync(id); 
            //ritorno una view contenente l'istanza del form di modifica
            return View(inputModel); //riceve l'oggetto popolato con tutti i dati
        }


        [HttpPost] //eseguiamo il metodo SOLO quando la richiesta é post (quando invio i dati)
        public async Task<IActionResult> Edit(CourseEditInputModel inputModel)  //ricevo l'istanza riempita con tutti i dati
        {
            //verifica della validità dell'input (in base alle regole definite nel CourseEditInputModel)
            if(ModelState.IsValid){
                //titolo valido: creiamo il corso e se tutto va bene, reindirizziamo l'utente alla pagina di elenco
                try
                {
                    CourseDetailViewModel course = await courseService.EditCourseAsync(inputModel);  //passo il titolo (ricevuto nell'oggetto CourseCreateInputModel)
                    //imposto il messaggio di conferma
                    TempData["ConfirmationMessage"] = "I dati sono stati salvati con successo";
                    //redireziono alla pagina di dettaglio, creo un oggetto con proprietà id valorizzata con l'id del corso
                    return RedirectToAction(nameof(Detail), new {id = inputModel.Id});             //dopo aver eseguito l'azione, viene indirizzato alla pagina Index
                }
                catch (CourseTitleUnavailableException)
                {
                    //se si verifica l'exception: aggiungiamo l'errore al ModelState e la segnaliamo al docente
                    ModelState.AddModelError(nameof(CourseEditInputModel.Title), "Questo titolo é già esistente e in uso");
                }
            }

            ViewData["Title"] = "Modifica corso";
            return View(inputModel); //riceve l'oggetto popolato con tutti i dati
        }
    }
}