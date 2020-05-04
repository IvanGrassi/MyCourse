using System;
using System.Data;
using MyCourse.Models.Entities;

namespace MyCourse.Models.ViewModels
{
    public class LessonViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TimeSpan Duration { get; set; }

        //per AdoNet
        //permette di mappare tutti i valori trovati nel DataRow, all'interno di un istanza di LessonViewModel
        public static LessonViewModel FromDataRow(DataRow dataRow)
        {
            var lessonViewModel = new LessonViewModel
            {
                //qui genero un istanza di LessonViewModel, assegno ogni proprietà con i dati ottenuti dal DataRow
                Id = Convert.ToInt32(dataRow["Id"]),
                Title = Convert.ToString(dataRow["Title"]),
                Description = Convert.ToString(dataRow["Description"]),
                Duration = TimeSpan.Parse(Convert.ToString(dataRow["Duration"])),
            };
            return lessonViewModel;
        }

        //per EFCore
        //permette di mappare tutti i valori trovati nell'entità course
        public static LessonViewModel FromEntity(Lesson lesson)
        {
            return new LessonViewModel
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Duration = lesson.Duration,
                Description = lesson.Description
            };
        }
    }
}