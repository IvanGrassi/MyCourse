using System.Collections.Generic;

namespace MyCourse.Models.ViewModels
{
    public class ListViewModel<T>
    {
        //Raccoglie al massimo 10 corsi per pagina
        public List<T> Results { get; set; }

        //Numero totale dei corsi corrispondenti all'input dell'utente (es: se fa una ricerca)
        public int TotalCount { get; set; }
    }
}