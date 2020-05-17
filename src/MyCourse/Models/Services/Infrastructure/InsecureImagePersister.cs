using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace MyCourse.Models.Services.Infrastructure
{
    public class InsecureImagePersister : IImagePersister
    {
    private readonly IWebHostEnvironment env;

    //ottenere il percorso fisico di wwwroot
    //IWebHostEnvironment fornisce il percorso FISICO della directory wwwroot
    public InsecureImagePersister(IWebHostEnvironment env)
    {
        this.env = env;
    }

    public async Task<string> SaveCourseImageAsync(int courseId, IFormFile formFile)
    {
        //insecure nel senso che se l'utente carica un virus, con CopyToAsync viene scritto il virus sul disco
        //seguirà la validazione e sanitizzazione


        //CopyToAsync: Legge i byte dallo stream di input (RAM o file temp su disco)
        //e gli scrive sullo stream di output (file su disco) copiandoli TALI E QUALI

        string path = $"/Courses/{courseId}.jpg";    //percorso di destinazione dove verranno salvate le immagini

        //Path Combine in questo modo permette l'utilizzo anche su altri OS
        string physicalPath = Path.Combine(env.WebRootPath, "Courses", $"{courseId}.jpg");   //concateniamo il path fisico con il /Courses/...

        using FileStream fileStream = File.OpenWrite(physicalPath);   //apriamo uno stream per scrivere su file (OpenWrite a cui passo il percorso del file)
        
        await formFile.CopyToAsync(fileStream);                       //Copiamo il contenuto sul FileStream di destinazione

        //Restituire il percorso al file
        return path;
    }
}
}