using System;
using System.Data;
using MyCourse.Models.Enums;
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

        public static CourseViewModel FromDataRow(DataRow courseRow) //fa il mapping di ogni datarow su istanze di CourseViewModel
        {
            //qui genero un istanza di CourseViewModel
            var courseViewModel = new CourseViewModel
            {
                Title = Convert.ToString(courseRow["Title"]),
                ImagePath = Convert.ToString(courseRow["ImagePath"]),
                Author = Convert.ToString(courseRow["Author"]),
                Rating = Convert.ToDouble(courseRow["Rating"]),
                //istazio un nuovo oggetto in Money dove definisco la valuta (Enum-Currency) e il prezzo completo (FullPrice) 
                FullPrice = new Money(
                    Enum.Parse<Currency>(Convert.ToString(courseRow["FullPrice_Currency"])),
                    Convert.ToDecimal(courseRow["FullPrice_Amount"])
                ),
                CurrentPrice = new Money(
                    Enum.Parse<Currency>(Convert.ToString(courseRow["CurrentPrice_Currency"])),
                    Convert.ToDecimal(courseRow["CurrentPrice_Amount"])
                ),
                Id = Convert.ToInt32(courseRow["Id"])
            };
            return courseViewModel;
        }

        //i prezzi non sono in double o altro poiché un corso deve essere venduto in multivaluta
    }
}