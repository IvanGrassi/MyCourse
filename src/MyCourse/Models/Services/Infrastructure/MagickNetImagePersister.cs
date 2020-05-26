using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MyCourse.Models.Exceptions.Infrastructure;

namespace MyCourse.Models.Services.Infrastructure
{
    public class MagickNetImagePersister : IImagePersister
    {
        //qua avviene la validazione dell'immagine e la sanitizzazione

        private readonly IWebHostEnvironment env;

        private readonly SemaphoreSlim semaphore;

        public MagickNetImagePersister(IWebHostEnvironment env)
        {
            //limitazione del consumo di RAM
            ResourceLimits.Height = 4000;   //massima altezza
            ResourceLimits.Width = 4000;    //massima larghezza
            //se superiore: Magick.NET la rifiuta e mostra un eccezione

            //Grazie a un SemaphoreSlim possiamo fare in modo che un blocco di codice critico, che potenzialmente è in grado di destabilizzare
            // l'applicazione, venga eseguito al massimo da un certo numero di thread contemporanei
            semaphore = new SemaphoreSlim(2);   //limita a un massimo di 2 il numero di thread in esecuzione
            this.env = env;
        }

        public async Task<string> SaveCourseImageAsync(int courseId, IFormFile formFile)
        {
            //Il metodo WaitAsync ha anche un overload che permette di passare un timeout
            //Ad esempio, se vogliamo aspettare al massimo 1 secondo:
            //await semaphore.AwaitAsync(TimeSpan.FromSeconds(1));
            //Se il timeout scade, il SemaphoreSlim solleverà un'eccezione (così almeno non resta in attesa all'infinito)
            await semaphore.WaitAsync();    //metodo che gestisce il passaggio dei thread
            try
            {
                //Salvare il file
                string path = $"/Courses/{courseId}.jpg";
                string physicalPath = Path.Combine(env.WebRootPath, "Courses", $"{courseId}.jpg");

                //OpenReadStream permette di avere accesso al flusso di byte che compongono il file caricato dall'utente
                using Stream inputStream = formFile.OpenReadStream();
                //l'inputStream lo passiamo all'image Magick
                using MagickImage image = new MagickImage(inputStream);

                //Manipolare l'immagine
                int width = 300; //settiamo una larghezza di 300px e una altezza di 300px  
                int height = 300;

                //ridimensionare mantenendo le proporzioni e tagliando l'eccesso
                MagickGeometry resizeGeometry = new MagickGeometry(width, height)
                {
                    FillArea = true
                };
                image.Resize(resizeGeometry);       //lato corto sui 300px mentre il lato lungo viene calcolato da ImageMagick per preservare la proporzione
                image.Crop(width, width, Gravity.Center);    //ritaglia l'immagine in modo che sia di 300x300

                image.Quality = 70;                             //valore che imposta la qualità dell'immagine al 70% (non troppo sgranata)
                image.Write(physicalPath, MagickFormat.Jpg);    //salviamo l'immagine ritagliata e con MagickFormat il formato dell'immagine che vogliamo scrivere (jpg)

                /*
                //Ridimensionare alterando le proporzioni dell'immagine
                using var stream = formFile.OpenReadStream();
                using var image = new MagickImage(stream);
                var resizeGeometry = new MagickGeometry(300,300)
                {
                    IgnoreAspectRatio = true;   //ImageMagick deforma l'immagine se necessario
                }
                image.Resize(resizeGeometry);
                */

                /*
                //Ridimensionare mantenendo le proporzioni e l'intera figura
                using var stream = formFile.OpenReadStream();
                using var image = new MagickImage(stream);
                var resizeGeometry = new MagickGeometry(300,300)
                {
                    FillArea = false;       
                }
                image.Resize(resizeGeometry);
                image.Extent(300,300, MagickColor.FromRgb(255,255,255)); //colore usato per riempire le fascie laterali
                */

                //Restituire il percorso al file
                return path;
            }
            catch(Exception ex)     //quando si verifica una qualsiasi eccezione, sollevo una ImagePersistenceException
            {    
                throw new ImagePersistenceException(ex);
            } 
            finally 
            {
                semaphore.Release();    //lavoro dei thred finito, semaforo verde per il prossimo thread
            }
        }
    }
}