using System;
using System.Data;

namespace MyCourse.Models.ViewModels
{
    public class LessonViewModel
    {
        public string Title { get; set; }
        public TimeSpan Duration { get; set; }      //durata temporale

        public static LessonViewModel FromDataRow(DataRow lessonRow)
        {
            var lessonViewModel = new LessonViewModel
            {
                //qui genero un istanza di LessonViewModel, assegno ogni proprietà con i dati ottenuti dal DataRow
                //Id = Convert.ToInt32(dataRow["Id"]),
                Title = Convert.ToString(lessonRow["Title"]),
                //Description = Convert.ToString(dataRow["Description"]),
                Duration = TimeSpan.Parse(Convert.ToString(lessonRow["Duration"])),
            };
            return lessonViewModel;
        }
    }
}