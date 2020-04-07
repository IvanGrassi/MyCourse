using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MyCourse.Models.Options;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Services.Application
{
    public class AdoNetCourseServices : ICourseService
    {

        private readonly IDatabaseAccess db;
        private readonly IOptionsMonitor<CoursesOptions> CoursesOptions;

        public AdoNetCourseServices(IDatabaseAccess db, IOptionsMonitor<CoursesOptions> CoursesOptions) //dipende dai due servizi infrastrutturali
        {
            this.CoursesOptions = CoursesOptions;
            this.db = db;
        }

        public async Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
            //la prima query carica il corso scelto al rispettivo id, la seconda invece carica tutte le lezioni legate all'id di quel corso
            //FormattableString separa la parte fissa dai suoi parametri
            FormattableString query = $@"SELECT Id, Title, Description, ImagePath, Author, Rating, FullPrice_Amount, FullPrice_Currency, CurrentPrice_Amount, CurrentPrice_Currency 
            from Courses WHERE Id = {id}; 
            SELECT Id, Title, Description, Duration 
            FROM Lessons WHERE CourseID = {id}";

            DataSet dataSet = await db.ExecuteQueryAsync(query);

            //Course: 
            //estrae i dati dalla prima datatable, se Rows >1 allora c'é un errore in quanto ci può essere solo 1 Row (evita la SqlInjection)
            var courseTable = dataSet.Tables[0];
            if (courseTable.Rows.Count != 1)
            {
                throw new InvalidOperationException($"Did not return exactly 1 row for Course {id}");
            }
            var courseRow = courseTable.Rows[0];            //leggiamo la riga (courseRow), la passiamo a FromDataRow che farà la mappatura tra il datarow restituendo un oggetto di tipo courseDetailViewModel
            var courseDetailViewModel = CourseDetailViewModel.FromDataRow(courseRow);

            //Lessons
            var lessonDataTable = dataSet.Tables[1];

            foreach (DataRow lessonRow in lessonDataTable.Rows)
            {
                LessonViewModel lessonViewModel = LessonViewModel.FromDataRow(lessonRow);
                courseDetailViewModel.Lessons.Add(lessonViewModel);     //ogni oggetto di LessonViewModel trovato viene aggiunto alla lista
            }
            return courseDetailViewModel;
        }

        public async Task<List<CourseViewModel>> GetCoursesAsync()
        {
            //quali informazioni estrarre nei confronti di un database? eseguo una query
            FormattableString query = $"SELECT Id, Title, ImagePath, Author, Rating, FullPrice_Amount, FullPrice_Currency, CurrentPrice_Amount, CurrentPrice_Currency from Courses";
            DataSet dataset = await db.ExecuteQueryAsync(query);
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