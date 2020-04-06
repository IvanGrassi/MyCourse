using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyCourse.Models.Enums;
using MyCourse.Models.ValueTypes;
using MyCourse.Models.ViewModels;




namespace MyCourse.Models.Services.Application                  //FILE ESCLUSO DALLA COMPILAZIONE, NON VIENE PIU USATO
{
    public class CourseService : ICourseService                 //invocato dai controller per girare loro dei dati, implementata l'interfaccia
    {
        public List<CourseViewModel> GetCourses()
        {
            var courseList = new List<CourseViewModel>();       //crea una lista di CourseViewModel
            var random = new Random();
            for (int i = 1; i <= 20; i++)
            {
                var price = Convert.ToDecimal(random.NextDouble() * 10 + 10);
                var course = new CourseViewModel                //crea delle istanze di CourseViewModel
                {
                    Id = i,
                    Title = $"Corso {i}",
                    CurrentPrice = new Money(Currency.EUR, price),  //Currency = EUR, Amount = price
                    FullPrice = new Money(Currency.EUR, random.NextDouble() > 0.5 ? price : price - 1),
                    Author = "Nome cognome",
                    Rating = random.NextDouble() * 5.0,         //numero casuale tra 0.0 e 1.0 moltiplicato per 5
                    ImagePath = "/logo.svg"
                };
                courseList.Add(course);                         //aggiunge ogni corso alla lista
            }
            return courseList;
        }

        public CourseDetailViewModel GetCourse(int id)      //alla richiesta di un certo id, vengono caricate le informazioni del corso
        {
            var random = new Random();
            var price = Convert.ToDecimal(random.NextDouble() * 10 + 10);
            var course = new CourseDetailViewModel
            {
                Id = id,
                Title = $"Corso {id}",
                CurrentPrice = new Money(Currency.EUR, price),  //Currency = EUR, Amount = price
                FullPrice = new Money(Currency.EUR, random.NextDouble() > 0.5 ? price : price - 1),
                Author = "Nome cognome",
                Rating = random.Next(10, 50) / 10.0,
                ImagePath = "/logo.svg",
                Description = $"Descrizione {id}",
                Lessons = new List<LessonViewModel>()       //lista che conterrà tutte le lezioni del corso con id ...
            };

            for (int i = 1; i <= 5; i++)
            {
                var lesson = new LessonViewModel
                {
                    Title = $"lezione {i}",                 //lezione 1, ec...
                    Duration = TimeSpan.FromSeconds(random.Next(40, 90))
                };
                course.Lessons.Add(lesson);
            }
            return course;
        }

        public Task<List<CourseViewModel>> GetCoursesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}