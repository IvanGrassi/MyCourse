using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MyCourse.Models.Enums;
using MyCourse.Models.ValueTypes;

namespace MyCourse.Models.ViewModels
{
    //classe per il dettaglio del singolo corso
    public class CourseDetailViewModel : CourseViewModel  //CourseDetailViewModel eredita da CourseViewModel le sue proprietà
    {
        public string Description { get; set; }
        public List<LessonViewModel> Lessons { get; set; }  //tipo complesso con due proprietà (Titolo, durata)

        public TimeSpan TotalCourseDuration
        {
            //utilizza una query LINQ, prende le lezioni e le somma tutte le lezioni trovate nella lista (nel LessonViewModel) in secondi
            get => TimeSpan.FromSeconds(Lessons?.Sum(l => l.Duration.TotalSeconds) ?? 0);
        }

        public static new CourseDetailViewModel FromDataRow(DataRow courseRow)
        {
            var courseDetailViewModel = new CourseDetailViewModel
            {
                //qui genero un istanza di CourseDetailViewModel, assegno ogni proprietà con i dati ottenuti dal DataRow
                Title = Convert.ToString(courseRow["Title"]),
                Description = Convert.ToString(courseRow["Description"]),
                ImagePath = Convert.ToString(courseRow["ImagePath"]),
                Author = Convert.ToString(courseRow["Author"]),
                Rating = Convert.ToDouble(courseRow["Rating"]),
                //istazio un nuovo oggetto in Money dove definisco la valuta (Enum-Currency) e il prezzo completo (FullPrice) 
                FullPrice = new Money(
                    Enum.Parse<Currency>(Convert.ToString(courseRow["FullPrice_Currency"])),
                    Convert.ToDecimal(courseRow["FullPrice_Amount"])
                ),
                CurrentPrice = new Money(
                    Enum.Parse<Currency>(Convert.ToString(courseRow["CurrentPrice_Currency"])),
                    Convert.ToDecimal(courseRow["CurrentPrice_Amount"])
                ),
                Id = Convert.ToInt32(courseRow["Id"]),
                Lessons = new List<LessonViewModel>()
            };
            return courseDetailViewModel;
        }
    }
}