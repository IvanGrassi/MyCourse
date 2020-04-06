using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Services.Application
{
    public class EfCoreCourseService : ICourseService
    {
        private readonly MyCourseDbContext dbContext;

        public EfCoreCourseService(MyCourseDbContext dbContext)    //per esprimere la dipendenza del servizio applicativo dal servizio infrastutturale (MyCourseDbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
            CourseDetailViewModel viewModel = await dbContext.Courses
                .Where(course => course.Id == id)
                .Select(course => new CourseDetailViewModel
                {
                    Id = course.Id,
                    Title = course.Title,
                    Description = course.Description,
                    Author = course.Author,
                    ImagePath = course.ImagePath,
                    Rating = course.Rating,
                    CurrentPrice = course.CurrentPrice,
                    FullPrice = course.FullPrice,
                    Lessons = course.Lessons.Select(lesson => new LessonViewModel
                    {
                        Id = lesson.Id,
                        Title = lesson.Title,
                        Description = lesson.Description,
                        Duration = lesson.Duration
                    }).ToList()
                })
                .AsNoTracking()
                .SingleAsync();      //restituisce il 1 elemento di un elenco, se é vuoto o più di uno, solleva un eccezione
            //.FirstAsync();    //Restituisce il primo elemento, ma se l'elenco é vuoto solleva un eccezione
            return viewModel;
        }

        public async Task<List<CourseViewModel>> GetCoursesAsync()
        {
            //per ogni proprietà trovata nel CourseViewModel, dobbiamo assegnare il valore trovato nell'entità course
            IQueryable<CourseViewModel> queryLinq = dbContext.Courses
            .AsNoTracking()
            .Select(course => CourseViewModel.FromEntity(course));

            //qui avviene l'esecuzione della query
            List<CourseViewModel> courses = await queryLinq.ToListAsync();      //invoco IQueryable ad una List di CourseViewModel, EFC apre la connessione con il Db per inviare la query 

            return courses;
        }
    }
}