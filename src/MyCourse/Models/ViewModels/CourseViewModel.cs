using System;
using System.Data;
using MyCourse.Models.Entities;
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

        //per AdoNet
        //permette di mappare tutti i valori trovati nel DataRow, all'interno di un istanza di CourseViewModel
        public static CourseViewModel FromDataRow(DataRow courseRow)         
        {
            //qui genero un istanza di CourseViewModel, assegno ogni proprietà con i dati ottenuti dal DataRow
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

        //per EFCore
        //permette di mappare tutti i valori trovati nell'entità course
        public static CourseViewModel FromEntity(Course course)
        {
            return new CourseViewModel
            {
                Id = course.Id,
                Title = course.Title,
                ImagePath = course.ImagePath,
                Author = course.Author,
                Rating = course.Rating,
                CurrentPrice = course.CurrentPrice,
                FullPrice = course.FullPrice
            };
        }

        //i prezzi non sono in double o altro poiché un corso deve essere venduto in multivaluta
    }
}