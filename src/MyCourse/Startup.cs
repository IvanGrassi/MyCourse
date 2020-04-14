using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyCourse.Models.Options;
using MyCourse.Models.Services.Application;
using MyCourse.Models.Services.Infrastructure;
using Westwind.AspNetCore.LiveReload;

namespace MyCourse
{
    public class Startup
    {
        public IConfiguration Configuration { get; }    //in sola lettura

        public Startup(IConfiguration configuration)    //per andare a recuperare la stringa di connessione dal file appsettings.json
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)          //lega le interfacce ad implementazioni
        {
            services.AddResponseCaching();              //permette il ResponseCaching

            services.AddControllersWithViews(options =>
            {
                var homeProfile = new CacheProfile();
                //homeProfile.Duration = Configuration.GetValue<int>("ResponseCache:Home:Duration");
                //homeProfile.Location = Configuration.GetValue<ResponseCacheLocation>("ResponseCache:Home:Location");  //sia duration che location li recupero tramite appsettings.json

                //homeProfile.VaryByQueryKeys = new string[]{"page"};       //la cache varia in base al valore della chiave "page", lo recupero dall'appsettings.json
                Configuration.Bind("ResponseCache:Home", homeProfile);      //sezione da cui traggo i valori (appsettings.json) e l'istanza dell'oggetto che voglio popolare con i valori di configurazione partendo da home in giu

                options.CacheProfiles.Add("Home", homeProfile);
            }).AddRazorRuntimeCompilation();

            //services.AddTransient<ICourseService, AdoNetCourseServices>();                  //ogni volta che un componente ha una dipendenza da ICourseService, in realtà la sostituisce e coustruisce un AdoNetCourseServices
            services.AddTransient<ICourseService, EfCoreCourseService>();                 //ogni volta che un componente ha una dipendenza da ICourseService, verrà fornita un istanza di EfCoreCourseService
            services.AddTransient<IDatabaseAccess, SqlLiteDatabaseAccess>();                //ogni volta che un componente ha una dipendenza da IDatabaseAccess, dotnetcore inietterà un istanza di SqlLiteDatabaseAccess
            services.AddTransient<ICachedCourseService, MemoryCacheCourseService>();        //ogni volta che un componente ha una dipendenza da ICachedCourseService, dotnetcore inietterà un istanza di MemoryCacheCourseService


            //services.AddScoped<MyCourseDbContext>();                                      //permette di avere al massimo un istanza per ogni richiesta HTTP
            //services.AddDbContext<MyCourseDbContext>();                                   //permette di fare in modo che ogni query eseguita, viene messo in un file di log il comando sql

            services.AddDbContextPool<MyCourseDbContext>(optionsBuilder =>
            {
                //recupera dalla sezione "ConnectionStrings" di appsettings.json da cui recupero "Default" (che é una stringa)
                string connectionString = Configuration.GetSection("ConnectionStrings").GetValue<string>("Default");
                optionsBuilder.UseSqlite(connectionString);
            });

            #region Configurazione del servizio di cache distribuita

            //Se vogliamo usare Redis, ecco le istruzioni per installarlo: https://docs.microsoft.com/it-it/aspnet/core/performance/caching/distributed?view=aspnetcore-2.2#distributed-redis-cache
            //Bisogna anche installare il pacchetto NuGet: Microsoft.Extensions.Caching.StackExchangeRedis
            services.AddStackExchangeRedisCache(options =>
            {
                Configuration.Bind("DistributedCache:Redis", options);                      //Recupero i valori di configurazione dall'appsettings.json nell'oggetto options
            });

            //Se vogliamo usare Sql Server, ecco le istruzioni per preparare la tabella usata per la cache: https://docs.microsoft.com/it-it/aspnet/core/performance/caching/distributed?view=aspnetcore-2.2#distributed-sql-server-cache
            /*services.AddDistributedSqlServerCache(options => 
            {
                Configuration.Bind("DistributedCache:SqlServer", options);
            });*/

            //Se vogliamo usare la memoria, mentre siamo in sviluppo
            //services.AddDistributedMemoryCache();
            #endregion

            //Opzioni (tipizzazione forte)
            services.Configure<ConnectionStringsOptions>(Configuration.GetSection("ConnectionStrings"));        //i valori vengono recuperati dal file appsettings.json, sezione ConnectionStrings
            services.Configure<CoursesOptions>(Configuration.GetSection("Courses"));                            //recupera le opzioni dalla classe dal file appsettings.json, sezione Courses
            services.Configure<MemoryCacheOptions>(Configuration.GetSection("MemoryCache"));                    //utilizza la classe preimpostata "MemoryCacheOptions", contiene i valori di configurazione per il caching, estraggo i valori da MemoryCache
#if DEBUG
            services.AddLiveReload();
#endif

        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //l'ambiente (production o development) viene definito nel launch.json
            if (env.IsDevelopment())                                        //il middleware viene usato solo quando siamo in ambiente development
            {
                app.UseDeveloperExceptionPage();                            //primo middleware, SEMPRE per primo, produce una pagina informativa in caso di errore
            }

#if DEBUG
            app.UseLiveReload();
#endif

            if (env.IsStaging() || env.IsProduction())                                   //in tutti gli altri casi (es: Production)
            {
                app.UseExceptionHandler("/Error");  //il parametro fornito (es: /Courses/Details/5000) verrà sostituito da /Error comprendendo le info dell'eccezione
            }


            app.UseStaticFiles();                   //middleware file statici (immagini ad esempio)

            app.UseRouting();

            app.UseResponseCaching();               //per il response caching

            app.UseEndpoints(endpoints =>
            {
                //app.UseMvcWithDefaultRoute();
                //configurato una route con il template controller, action e id. ora il middleware é in grado di estrapolare info dal percorso della richiesta utente

                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");  //settati i valori di default, id é opzionale //Courses/Detail/5 = Controller/Action/id
                //si rifà a quanto dichiarato nelle view: <a class="nav-link" asp-action="Index" asp-controller="Home">Home</a> 
            });
        }
    }
}
