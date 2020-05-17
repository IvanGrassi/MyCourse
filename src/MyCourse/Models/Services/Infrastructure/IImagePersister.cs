using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MyCourse.Models.Services.Infrastructure
{
    public interface IImagePersister
    {
        //metodo che ha la responsabilità di salvare l'immagine: gli passo l'id e il riferimento al file da salvare
        //restituisce una stringa che rappresenta il percorso per raggiungere l'immagine 
        Task<string> SaveCourseImageAsync(int courseId, IFormFile formFile);
    } 

}