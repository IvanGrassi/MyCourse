using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Models.Exceptions;

namespace MyCourse.Controllers
{
    public class ErrorController : Controller
    {
        //controller che imposta un titolo (Errore) e restituisce una View (in Views/Error/Index.cshtml)

        public IActionResult Index()
        {
            //ottenere info sull'eccezione
            //Feature avrà due proprietà: Error (da cui si ottiene l'eccezione sollevata) e Path (/Course/Detail/5000 ad esempio)
            var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            switch (feature.Error)
            {
                case CourseNotFoundException exc:             //se l'eccezione é di questo titolo
                    ViewData["Title"] = "Corso non trovato";    //allora visualizzo un titolo specifico
                    Response.StatusCode = 404;
                    return View("CourseNotFound");              //e restituisco una view specifica

                //in tutti gli altri casi
                default:
                    ViewData["Title"] = "Errore";
                    return View();
            }
        }
    }
}