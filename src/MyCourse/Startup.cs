using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
#if DEBUG
            services.AddLiveReload();
#endif
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            //services.AddTransient<ICourseService, AdoNetCourseServices>();       //ogni volta che un componente ha una dipendenza da ICourseService, in realtà la sostituisce e coustruisce un AdoNetCourseServices
            services.AddTransient<ICourseService, EfCoreCourseService>();          //ogni volta che un componente ha una dipendenza da ICourseService, verrà fornita un istanza di EfCoreCourseService
            services.AddTransient<IDatabaseAccess, SqlLiteDatabaseAccess>();       //ogni volta che un componente ha una dipendenza da IDatabaseAccess, dotnetcore inietterà un istanza di SqlLiteDatabaseAccess

            //services.AddScoped<MyCourseDbContext>();        //permette di avere al massimo un istanza per ogni richiesta HTTP
            //services.AddDbContext<MyCourseDbContext>();     //permette di fare in modo che ogni query eseguita, viene messo in un file di log il comando sql
            services.AddDbContextPool<MyCourseDbContext>(optionsBuilder =>
            {
                //recupera dalla sezione "ConnectionStrings" di appsettings.json da cui recupero "Default" (che é una stringa)
                string connectionString = Configuration.GetSection("ConnectionStrings").GetValue<string>("Default");
                optionsBuilder.UseSqlite(connectionString);
            });


            //Opzioni (tipizzazione forte)
            services.Configure<ConnectionStringsOptions>(Configuration.GetSection("ConnectionStrings"));        //i valori vengono recuperati dal file appsettings.json, sezione ConnectionStrings
            services.Configure<CoursesOptions>(Configuration.GetSection("Courses"));                            //recupera le opzioni dalla classe dal file appsettings.json, sezione Courses
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())                                        //il middleware viene usato solo quando siamo in ambiente development
            {
                app.UseDeveloperExceptionPage();                            //primo middleware, SEMPRE per primo, produce una pagina informativa in caso di errore
            }

#if DEBUG
            app.UseLiveReload();
#endif

            //l'ambiente (production o development) viene definito nel launchsettings.json

            app.UseStaticFiles();                                           //middleware file statici (immagini ad esempio)

            app.UseRouting();

            app.UseEndpoints(endpoints => //secondo middleware
            {
                //app.UseMvcWithDefaultRoute();
                //configurato una route con il template controller, action e id. ora il middleware é in grado di estrapolare info dal percorso della richiesta utente

                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");  //settati i valori di default, id é opzionale
                //Courses/Detail/5 = Controller/Action/id
            });
        }
    }
}
