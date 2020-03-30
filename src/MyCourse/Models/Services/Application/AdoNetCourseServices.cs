using System.Collections.Generic;
using System.Data;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Services.Application
{
    public class AdoNetCourseServices : ICourseService
    {

        private readonly IDatabaseAccess db;

        public AdoNetCourseServices(IDatabaseAccess db) //esprime la dipendenza
        {
            this.db = db;
        }

        public CourseDetailViewModel GetCourse(int id)
        {
            throw new System.NotImplementedException();
        }

        public List<CourseViewModel> GetCourses()
        {
            //quali informazioni estrarre nei confronti di un database? eseguo una query
            string query = "SELECT Id, Title, ImagePath, Author, Rating, FullPrice_Amount, FullPrice_Currency, CurrentPrice_Amount, CurrentPrice_Currency from Courses";
            DataSet dataset = db.ExecuteQuery(query);
            var dataTable = dataset.Tables[0];                          //primo datatable
            var courseList = new List<CourseViewModel>();               //aggiungo CourseViewModel ad una lista
            foreach (DataRow courseRow in dataTable.Rows)               //il datatable contiene tutte le righe trovate e cicliamo tutte le Rows in un foreach
            {
                CourseViewModel course = CourseViewModel.FromDataRow(courseRow);    //il metodo FromDataRow conterrà tutti i risultati ciclati e viene richamato. Il tutto mi fa ottenere un oggetto di tipo CourseViewModel
                courseList.Add(course);                                 //ogni oggetto ciclato viene aggiunto alla lista                
            }
            return courseList;                                          //e alla fine ritorno il risultato completo
        }
    }
}