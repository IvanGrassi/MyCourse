using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyCourse.Models.Entities;
using MyCourse.Models.Exceptions;
using MyCourse.Models.InputModels;
using MyCourse.Models.Options;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Services.Application
{
    public class EfCoreCourseService : ICourseService
    {
        private readonly ILogger<EfCoreCourseService> logger;
        private readonly MyCourseDbContext dbContext;
        private readonly IOptionsMonitor<CoursesOptions> coursesOptions;

        public EfCoreCourseService(ILogger<EfCoreCourseService> logger, MyCourseDbContext dbContext, IOptionsMonitor<CoursesOptions> coursesOptions) //per esprimere la dipendenza del servizio applicativo dal servizio infrastutturale (MyCourseDbContext)
        {
            this.coursesOptions = coursesOptions;
            this.logger = logger;
            this.dbContext = dbContext;
        }

        public async Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
            IQueryable<CourseDetailViewModel> queryLinq = dbContext.Courses
                .AsNoTracking()
                .Include(course => course.Lessons)
                .Where(course => course.Id == id)
                .Select(course => CourseDetailViewModel.FromEntity(course)); //Usando metodi statici come FromEntity, la query potrebbe essere inefficiente. Mantenere il mapping nella lambda oppure usare un extension method personalizzato

            CourseDetailViewModel viewModel = await queryLinq.FirstOrDefaultAsync();
            //.FirstOrDefaultAsync(); //Restituisce null se l'elenco è vuoto e non solleva mai un'eccezione
            //.SingleOrDefaultAsync(); //Tollera il fatto che l'elenco sia vuoto e in quel caso restituisce null, oppure se l'elenco contiene più di 1 elemento, solleva un'eccezione
            //.FirstAsync(); //Restituisce il primo elemento, ma se l'elenco è vuoto solleva un'eccezione

            if (viewModel == null)
            {
                logger.LogWarning("Course {id} not found", id);
                throw new CourseNotFoundException(id);
            }

            return viewModel;
        }

        public async Task<List<CourseViewModel>> GetCoursesAsync(CourseListInputModel model)
        {

            IQueryable<Course> baseQuery = dbContext.Courses;

            switch (model.OrderBy)
            {
                case "Title":
                    if (model.Ascending)    //ascending = true e ordino ASC
                    {
                        baseQuery = baseQuery.OrderBy(course => course.Title);
                    }
                    else    //Altrimenti DESC
                    {
                        baseQuery = baseQuery.OrderByDescending(course => course.Title);
                    }
                    break;
                case "Rating":
                    if (model.Ascending)
                    {
                        baseQuery = baseQuery.OrderBy(course => course.Rating);
                    }
                    else
                    {
                        baseQuery = baseQuery.OrderByDescending(course => course.Rating);
                    }
                    break;
                case "CurrentPrice":
                    if (model.Ascending)
                    {
                        baseQuery = baseQuery.OrderBy(course => (double)course.CurrentPrice.Amount);
                    }
                    else
                    {
                        baseQuery = baseQuery.OrderByDescending(course => (double)course.CurrentPrice.Amount);
                    }
                    break;
                case "Id":
                    if (model.Ascending)
                    {
                        baseQuery = baseQuery.OrderBy(course => course.Id);
                    }
                    else
                    {
                        baseQuery = baseQuery.OrderByDescending(course => course.Id);
                    }
                    break;
            }

            //per ogni proprietà trovata nel CourseViewModel, dobbiamo assegnare il valore trovato nell'entità course
            IQueryable<CourseViewModel> queryLinq = baseQuery
                .Where(course => course.Title.Contains(model.Search)) //che contiene il valore di search (ciò che cerca l'utente)
                .Skip(model.Offset)
                .Take(model.Limit)
                .AsNoTracking()
                .Select(course => CourseViewModel.FromEntity(course));

            //qui avviene l'esecuzione della query
            List<CourseViewModel> courses = await queryLinq.ToListAsync(); //invoco IQueryable ad una List di CourseViewModel, EFC apre la connessione con il Db per inviare la query 

            return courses;
        }
    }
}