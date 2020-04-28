using System;

namespace MyCourse.Models.Exceptions
{
    public class CourseNotFoundException : Exception
    {
        //costruttore che accetta l'id (che non é stato trovato)

        //title: titolo che si é cercato di inserire
        //innerException: Eccezione originale che si é verificata (SqliteException)
        //costruttore base di Exception: definisco il testo dell'eccezzione e l'eccezione originale
        public CourseNotFoundException(int courseId) : base($"Course {courseId} not found")
        {
            
        }
    }
}

