using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Services.Application
{
    public class MemoryCacheCourseService : ICachedCourseService
    {

        public ICourseService courseService { get; }    //usa per ottenere gli oggetti dal db
        public IMemoryCache memoryCache { get; }        //per ottenerli dalla cache

        public MemoryCacheCourseService(ICourseService courseService, IMemoryCache memoryCache)
        {
            this.courseService = courseService;
            this.memoryCache = memoryCache;
        }

        //TODO: Ricordati di usare memoryCache.Remove($"Course{id}"); quando aggiorni il corso

        public Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
            //con GetOrCreateAsync chiedo se esiste in cache un oggetto calcolato sull'id e lo recupero
            return memoryCache.GetOrCreateAsync($"Course{id}", cacheEntry =>
            {
                //inseriamo in cache un CourseDetailViewModel, 1 conta come capacità sul massimo di 1000 (impostato nel appsettings.json)
                cacheEntry.SetSize(1);

                //e SE l'oggetto NON esiste, con questa lambda lo recupero dal db
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
                return courseService.GetCourseAsync(id);    //recupero l'oggetto dal db usando ICourseService
            });
        }

        public Task<List<CourseViewModel>> GetCoursesAsync()
        {
            return memoryCache.GetOrCreateAsync($"Courses", cacheEntry =>
            {
                cacheEntry.SetSize(1);
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
                return courseService.GetCoursesAsync();
            });
        }
    }
}