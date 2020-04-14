using System.Collections.Generic;
using System.Threading.Tasks;
using MyCourse.Models.InputModels;
using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Services.Application
{
    public interface ICourseService
    {
        //a differenza di una classe NON contiene logica, ma si limita a definire un elenco
        //di proprietà, metodi ed eventi.

        Task<List<CourseViewModel>> GetCoursesAsync(CourseListInputModel model);

        Task<CourseDetailViewModel> GetCourseAsync(int id);
    }
}