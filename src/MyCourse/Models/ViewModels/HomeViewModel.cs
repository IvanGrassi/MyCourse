using System.Collections.Generic;

namespace MyCourse.Models.ViewModels
{
    public class HomeViewModel : CourseViewModel
    {
        //ViewModel che perette di raccogliere i dati delle List<CourseViewModel>

        public List<CourseViewModel> MostRecentCourses { get; set; }
        public List<CourseViewModel> BestRatingCourses { get; set; }
    }
}