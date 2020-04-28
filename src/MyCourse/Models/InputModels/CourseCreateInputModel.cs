using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Controllers;

namespace MyCourse.Models.InputModels
{
    public class CourseCreateInputModel
    {
        //Classe che verrà utilizzata per inserire il nuovo corso partendo dal titolo


        //le seguenti data annotations limitano il campo e allo stesso tempo lo rendono obbligatorio (non si può lasciarlo vuoto)
        //la regular expression: tutte le lettere, spazio e punto incluso, ad eccezione dei caratteri speciali
        //le quattro elencate sono chiamate "Data annotation"
        
        [Required (ErrorMessage = "Il titolo é obbligatorio"),
         MinLength(10, ErrorMessage = "Il titolo dev'essere di almeno {1} caratteri"),
         MaxLength(100, ErrorMessage = "Il titolo dev'essere di al massimo {1} caratteri"), 
         RegularExpression(@"^[\w\s\.]+$", ErrorMessage = "Titolo non valido"),
         
        //remote si occupa di inviare una richiesta ajax per eseguire una verifica lato server
        //il form non verrà inviato e la pagina non sarà ricaricata.
         Remote(action: nameof(CoursesController.IsTitleAvailable), controller: "Courses", ErrorMessage = "Il titolo esiste già")]
        public string Title { get; set; }
    }
}