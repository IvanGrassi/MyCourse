using MyCourse.Models.ValueTypes;

namespace MyCourse.Models.ViewModels
{
    public class CourseViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImagePath { get; set; }
        public string Author { get; set; }
        public double Rating { get; set; }
        public Money FullPrice { get; set; }
        public Money CurrentPrice { get; set; }

        //i prezzi non sono in double o altro poiché un corso deve essere venduto in multivaluta
    }
}