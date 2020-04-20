using Microsoft.AspNetCore.Mvc;
using MyCourse.Models.ViewModels;

namespace MyCourse.Customization.ViewComponents
{
    public class PaginationBarViewComponent : ViewComponent
    {
        //simile ad un controller ma con la differenza che qui deve esserci un metodo Invoke
        //si collega alla view di contenuto: Shared/Components/PaginationBar/Default.cshtml

        //definiamo un parametro di tipo object, il valore verrà fornito dalla view di contenuto (CourseListViewModel) che richiama il componente (model)


        //Non dipendendo da CourseListViewModel (ha due proprietà (Courses e Input), le i cui sottometodi comprendono del codice che NON SEMPRE é utile o viene utilizzato)
        //public IViewComponentResult Invoke(CourseListViewModel model)

        public IViewComponentResult Invoke(IPaginationInfo model)
        {

            //oggetto in grado di fornire il numero di pagina corrente
            //il numero di risultati totati
            // e il numero di risultati per pagina
            //Search, OrderBy e Ascending
            return View(model);
        }
    }
}