using System;
using System.Collections.Generic;
using System.Linq;

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
    }
}