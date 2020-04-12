using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using MyCourse.Models.ViewModels;
using Newtonsoft.Json;

namespace MyCourse.Models.Services.Application
{
    public class DistributedCacheCourseService : ICachedCourseService
    {
        //Implementa cio che é contenuto in ICachedCourseService

        public ICourseService courseService { get; }
        private readonly IDistributedCache distributedCache;

        //viene utilizzata nell'ambito della scalabilità orizzontale su più apparecchi

        public DistributedCacheCourseService(ICourseService courseService, IDistributedCache distributedCache)
        {
            //Riceviamo sia il servizio di cache distribuita (IDistributedCache),
            //che il servizio applicativo che ci serve per ottenere gli oggetti dal database (ICourseService)
            this.courseService = courseService;
            this.distributedCache = distributedCache;
        }


        public async Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
            //imposto la chiave
            string key = $"Course{id}";

            //Proviamo a recuperare l'oggetto serializzato dalla cache, uso la chiave con il metodo GetStringAsync
            string serializedObject = await distributedCache.GetStringAsync(key);

            //Se l'oggetto esisteva in cache (cioè se è diverso da null)
            if (serializedObject != null)
            {
                //Allora lo deserializzo e restituisco il dettaglio del corso
                return Deserialize<CourseDetailViewModel>(serializedObject);
            }

            //Se invece non esisteva, lo andiamo a recuperare dal database
            CourseDetailViewModel course = await courseService.GetCourseAsync(id);

            //Prima di restituire l'oggetto al chiamante, lo serializziamo.
            //Cioè ne creiamo una rappresentazione stringa o binaria
            serializedObject = Serialize(course);

            //Impostiamo la durata di permanenza in cache prima della scadenza
            var cacheOptions = new DistributedCacheEntryOptions();
            cacheOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(60));

            //Aggiungiamo in cache l'oggetto serializzato. Utilizzo SetStringAsync per scriverlo nella cache distribuita 
            await distributedCache.SetStringAsync(key, serializedObject, cacheOptions);

            //Lo restituisco
            return course;
        }

        public async Task<List<CourseViewModel>> GetCoursesAsync()
        {
            string key = $"Courses";
            string serializedObject = await distributedCache.GetStringAsync(key);

            if (serializedObject != null)
            {
                return Deserialize<List<CourseViewModel>>(serializedObject);
            }
            //Recupera la lista dal db, prima di restituirla però la serializzo
            List<CourseViewModel> courses = await courseService.GetCoursesAsync();
            serializedObject = Serialize(courses);


            var cacheOptions = new DistributedCacheEntryOptions();
            cacheOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(60));


            await distributedCache.SetStringAsync(key, serializedObject, cacheOptions);
            return courses;
        }

        private string Serialize(object obj)
        {
            //Convertiamo un oggetto in una stringa JSON
            return JsonConvert.SerializeObject(obj);
        }

        private T Deserialize<T>(string serializedObject)
        {
            //Riconvertiamo una stringa JSON in un oggetto
            return JsonConvert.DeserializeObject<T>(serializedObject);
        }
    }
}