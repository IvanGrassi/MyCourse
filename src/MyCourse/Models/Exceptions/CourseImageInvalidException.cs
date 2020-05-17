using System;

namespace MyCourse.Models.Exceptions
{
    public class CourseImageInvalidException : Exception
    {
        //costruttore che riceve l'id e il testo originale dell'eccezione
        //innerException: Eccezione originale che si é verificata 
        //in base definisco il testo personalizzato dell'eccezione alla quale passo il courseId e l'eccezione originaria
        public CourseImageInvalidException(int courseId, Exception innerException) : base($"Image for course '{courseId}' is not valid", innerException)
        {
            
        }
    }
}