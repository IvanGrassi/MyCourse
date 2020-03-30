using System.Collections.Generic;
using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Services.Application
{
    public interface ICourseService
    {
        //a differenza di una classe NON contiene logica, ma si limita a definire un elenco
        //di proprietà, metodi ed eventi.

        List<CourseViewModel> GetCourses();
        CourseDetailViewModel GetCourse(int id);
    }
}