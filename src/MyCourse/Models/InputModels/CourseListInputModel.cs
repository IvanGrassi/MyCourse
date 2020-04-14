using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Customization.ModelBinders;
using MyCourse.Models.Options;

namespace MyCourse.Models.InputModels
{
    //definizione del model binder personalizzato
    [ModelBinder(BinderType = typeof(CourseListInputModelBinder))]

    public class CourseListInputModel
    {
        //la classe viene utilizzata per sanitizzare i seguenti valori


        //il costruttore fornisce i dati grezzi che verranno sanitizzati
        public CourseListInputModel(string search, int page, string orderBy, bool ascending, CoursesOptions coursesOptions)
        {
            //Sanitizzazione
            var orderOptions = coursesOptions.Order;    //fornito dal model binder personalizzato
            if (!orderOptions.Allow.Contains(orderBy))  //se nell'oggetto orderOptions, la proprietà allow NON contiene order by (guardare appsettings.json)
            {
                orderBy = orderOptions.By; //altrimenti viene assegnato il valore di default della configurazione
                ascending = orderOptions.Ascending;
            }

            Search = search ?? ""; //null coalescensing opeator: si assicura che search non assuma valori nulli (search = search), se é nullo invece riporta ""
            Page = Math.Max(1, page); //mi dai il maggiore fra queti due numeri
            OrderBy = orderBy;
            Ascending = ascending;

            //---------------------------------

            Limit = coursesOptions.PerPage; //recupera la configurazione corrente (CurrentValue) dalla classe CoursesOptions
            Offset = (Page - 1) * Limit;
        }

        public string Search { get; }
        public int Page { get; }
        public string OrderBy { get; }
        public bool Ascending { get; }


        public int Limit { get; }
        public int Offset { get; }

        //togliamo il settere per evitare che il valore possa essere modificato
    }
}