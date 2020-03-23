using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MyCourse
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())    //il middleware viene usato solo quando siamo in ambiente development
            {
                app.UseDeveloperExceptionPage();    //primo middleware, SEMPRE per primo, produce una pagina informativa in caso di errore
            }

            //l'ambiente (production o development) viene definito nel launchsettings.json

            app.UseStaticFiles();               //middleware file statici (immagini ad esempio)

            app.UseRouting();

            app.UseEndpoints(endpoints => //secondo middleware
            {
                endpoints.MapGet("/", async context =>
                {
                    string nome = context.Request.Query["nome"];
                    await context.Response.WriteAsync($"Hello {nome}!");
                });
            });
        }
    }
}
