using System.Collections.Generic;
using MyCourse.Models.InputModels;

namespace MyCourse.Models.ViewModels
{
    public class CourseListViewModel : IPaginationInfo
    {
        //classe che permette di incapsulare le due proprietà senza dover inserire un altro model nelle pagine cshtml
        //CourseListViewModel verrà poi utilizzato nelle classi .cshtml

        //fornisce l'elenco dei corsi
        public ListViewModel<CourseViewModel> Courses { get; set; }

        //fornisce l'input dell'utente
        public CourseListInputModel Input { get; set; }

        #region Implementazione esplicita IPaginationInfo
        //membri implementati ESPLICITAMENTE dall'interfaccia IPaginationInfo
        int IPaginationInfo.CurrentPage => Input.Page;

        int IPaginationInfo.TotalResults => Courses.TotalCount;

        int IPaginationInfo.ResultsPerPage => Input.Limit;

        string IPaginationInfo.Search => Input.Search;

        string IPaginationInfo.OrderBy => Input.OrderBy;

        bool IPaginationInfo.Ascending => Input.Ascending;
        #endregion
    }
}