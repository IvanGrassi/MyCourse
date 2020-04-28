using System;

namespace MyCourse.Models.Exceptions
{
    public class CourseTitleUnavailableException : Exception
    {
        //exception personalizzata che viene chiamata dall'AdoNetCourseService
        public CourseTitleUnavailableException(string title, Exception innerException) : base($"Course title '{title}' existed", innerException)
        {
        }
    }
}