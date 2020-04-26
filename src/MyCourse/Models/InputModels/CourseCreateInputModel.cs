using System.ComponentModel.DataAnnotations;

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
         RegularExpression(@"^[\w\s\.]+$", ErrorMessage = "Titolo non valido")]
        public string Title { get; set; }
    }
}