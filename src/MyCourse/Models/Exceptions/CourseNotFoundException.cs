using System;

namespace MyCourse.Models.Exceptions
{
    public class CourseNotFoundException : Exception
    {
        //costruttore che accetta l'id (che non é stato trovato)
        public CourseNotFoundException(int courseId) : base($"Course {courseId} not found")
        {
        }
    }
}

