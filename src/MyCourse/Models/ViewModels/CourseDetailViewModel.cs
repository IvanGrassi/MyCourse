using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MyCourse.Models.Entities;
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

        //per AdoNet
        //permette di mappare tutti i valori trovati nel DataRow, all'interno di un istanza di CourseDetailViewModel
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

        //per EFCore
        //permette di mappare tutti i valori trovati nell'entità course
        public static new CourseDetailViewModel FromEntity(Course course)   //metodi in cui passo l'istanza
        {
            return new CourseDetailViewModel
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Author = course.Author,
                ImagePath = course.ImagePath,
                Rating = course.Rating,
                CurrentPrice = course.CurrentPrice,
                FullPrice = course.FullPrice,
                Lessons = course.Lessons
                                    .Select(lesson => LessonViewModel.FromEntity(lesson))
                                    .ToList()
            };
        }
    }
}