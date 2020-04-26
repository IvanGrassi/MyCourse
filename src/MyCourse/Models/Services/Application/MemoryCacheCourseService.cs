using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using MyCourse.Models.InputModels;
using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Services.Application {
    public class MemoryCacheCourseService : ICachedCourseService {

        public ICourseService courseService { get; } //usa per ottenere gli oggetti dal db
        public IMemoryCache memoryCache { get; } //per ottenerli dalla cache

        public MemoryCacheCourseService (ICourseService courseService, IMemoryCache memoryCache) {
            this.courseService = courseService;
            this.memoryCache = memoryCache;
        }

        //TODO: Ricordati di usare memoryCache.Remove($"Course{id}"); quando aggiorni il corso

        public Task<CourseDetailViewModel> GetCourseAsync (int id) {
            //con GetOrCreateAsync chiedo se esiste in cache un oggetto calcolato sull'id e lo recupero
            return memoryCache.GetOrCreateAsync ($"Course{id}", cacheEntry => {
                //inseriamo in cache un CourseDetailViewModel, 1 conta come capacità sul massimo di 1000 (impostato nel appsettings.json)
                cacheEntry.SetSize (1);

                //e SE l'oggetto NON esiste, con questa lambda lo recupero dal db
                cacheEntry.SetAbsoluteExpiration (TimeSpan.FromSeconds (60));
                return courseService.GetCourseAsync (id); //recupero l'oggetto dal db usando ICourseService
            });
        }

        public Task<ListViewModel<CourseViewModel>> GetCoursesAsync (CourseListInputModel model) {
            //Importante: gestire le chiavi dei parametri come fatto nella riga sotto

            //Metto in cache i risultati solo per le prime 5 pagine del catalogo, che reputo essere
            //le più visitate dagli utenti, e che perciò mi permettono di avere il maggior beneficio dalla cache.
            //E inoltre, metto in cache i risultati solo se l'utente non ha cercato nulla.
            //In questo modo riduco drasticamente il consumo di memoria RAM
            bool canCache = model.Page <= 5 && string.IsNullOrEmpty (model.Search);

            //Se canCache è true, sfrutto il meccanismo di caching
            if (canCache) {
                return memoryCache.GetOrCreateAsync ($"Courses{model.Page}-{model.OrderBy}-{model.Ascending}", cacheEntry => {
                    cacheEntry.SetSize (1);
                    cacheEntry.SetAbsoluteExpiration (TimeSpan.FromSeconds (60));
                    return courseService.GetCoursesAsync (model);
                });
            }

            //Altrimenti uso il servizio applicativo sottostante, che recupererà sempre i valori dal database
            return courseService.GetCoursesAsync (model);
        }

        //----------------------------------------------------------------------

        public Task<List<CourseViewModel>> GetBestRatingCoursesAsync () {
            //usiamo una chiave specifica (BestRatingCourses) che viene assegnata con l'invocazioe di GetBestRatingCoursesAsync
            return memoryCache.GetOrCreateAsync ($"BestRatingCourses", cacheEntry => {
                cacheEntry.SetSize (1);
                cacheEntry.SetAbsoluteExpiration (TimeSpan.FromSeconds (60));
                return courseService.GetBestRatingCoursesAsync ();
            });
        }

        public Task<List<CourseViewModel>> GetMostRecentCoursesAsync () {
            return memoryCache.GetOrCreateAsync ($"MostRecentCourses", cacheEntry => {
                cacheEntry.SetSize (1);
                cacheEntry.SetAbsoluteExpiration (TimeSpan.FromSeconds (60));
                return courseService.GetMostRecentCoursesAsync ();
            });
        }

        //-------------------------------------------Inserimento corsi------------------------------------
        public Task<CourseDetailViewModel> CreateCourseAsync(CourseCreateInputModel inputModel)
        {
            //non viene eseguito il caching per le operazioni di scrittura
            return courseService.CreateCourseAsync(inputModel);
        }
    }
}