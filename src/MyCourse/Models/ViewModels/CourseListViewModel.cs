using System.Collections.Generic;
using MyCourse.Models.InputModels;

namespace MyCourse.Models.ViewModels
{
    public class CourseListViewModel
    {
        //classe che permette di incapsulare le due proprietà senza dover inserire un altro model nelle pagine cshtml
        //CourseListViewModel verrà poi utilizzato nelle classi .cshtml

        //fornisce l'elenco dei corsi
        public ListViewModel<CourseViewModel> Courses { get; set; }

        //fornisce l'input dell'utente
        public CourseListInputModel Input { get; set; }
    }
}