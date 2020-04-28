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

        Task<ListViewModel<CourseViewModel>> GetCoursesAsync(CourseListInputModel model);
        Task<CourseDetailViewModel> GetCourseAsync(int id);

        Task<List<CourseViewModel>> GetMostRecentCoursesAsync();
        Task<List<CourseViewModel>> GetBestRatingCoursesAsync();

        //----------------------------Inserimento corso--------------------------
        Task<CourseDetailViewModel> CreateCourseAsync(CourseCreateInputModel inputModel);
        Task<bool> IsTitleAvailableAsync(string title); //riceve un parametro di tipo stringa e restituisce un task di bool (true/false)
    }
}