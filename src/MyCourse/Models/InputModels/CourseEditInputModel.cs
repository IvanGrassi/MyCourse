using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Controllers;
using MyCourse.Models.Entities;
using MyCourse.Models.Enums;
using MyCourse.Models.ValueTypes;

namespace MyCourse.Models.InputModels
{
    public class CourseEditInputModel : IValidatableObject  
    {
        //Classe che verrà utilizzata per modificare i dati del corso
        //IValidatableObject: usata per valori complessi soggetti a validazione (es: prezzo d'acquisto <= prezzo intero e stessa valuta)

        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Il titolo è obbligatorio"),
         MinLength(10, ErrorMessage = "Il titolo dev'essere di almeno {1} caratteri"),
         MaxLength(100, ErrorMessage = "Il titolo dev'essere di al massimo {1} caratteri"),
         RegularExpression(@"^[\w\s\.']+$", ErrorMessage = "Titolo non valido"),
         Remote(action: nameof(CoursesController.IsTitleAvailable), controller: "Courses", ErrorMessage = "Il titolo esiste già", AdditionalFields = "Id"),
         Display(Name = "Titolo")]
         //Display indica l'etichetta (spiega l'utilizzo della casella, é posta sopra il form)       
        public string Title { get; set; }
        
        
        [MinLength(10, ErrorMessage = "La descrizione dev'essere di almeno {1} caratteri"),
         MaxLength(4000, ErrorMessage = "La descrizione dev'essere di massimo {1} caratteri"),
         Display(Name = "Descrizione")]       
        public string Description { get; set; }
        

        [Display(Name = "Immagine rappresentativa")]
        public string ImagePath { get; set; }


        [Required(ErrorMessage = "L'email di contatto è obbligatoria"),
         EmailAddress(ErrorMessage = "Devi inserire un indirizzo email"),
         Display(Name = "Email di contatto")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Il prezzo intero è obbligatorio"),
         Display(Name = "Prezzo intero")]
        public Money FullPrice { get; set; }


        [Required(ErrorMessage = "Il prezzo corrente è obbligatorio"),
         Display(Name = "Prezzo corrente")]      
        public Money CurrentPrice { get; set; }


        [Display(Name = "Nuova immagine...")]
        public IFormFile Image { get; set; }
        public string RowVersion { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //Restituisce un IEnumerable di ValidationResult (cioé l'elenco dei problemi verificatisi)
            
            //yield return crea automaticamente un enumeratore (oggetto di una collezione)

            if(FullPrice.Currency != CurrentPrice.Currency) //verifica che le valute siano uguali, dopo la virgola mostro l'elenco delle proprietà coinvolte
            {
                yield return new ValidationResult("Il prezzo intero deve avere la stessa valuta del prezzo corrente", new [] { nameof(FullPrice) });
            }
            else if(FullPrice.Amount < CurrentPrice.Amount) //verifica che prezzo intero sia maggiore a prezzo scontato
            {
                yield return new ValidationResult("Il prezzo intero non può essere inferiore al prezzo corrente", new [] { nameof(FullPrice) });
            }
        }

        //per AdoNet
        //permette di mappare tutti i valori trovati nel DataRow, all'interno di un istanza di CourseEditInputModel
        public static CourseEditInputModel FromDataRow(DataRow courseRow)
        {
            //qui genero un istanza di CourseEditInputModel, assegno ogni proprietà con i dati ottenuti dal DataRow
            var courseEditInputModel = new CourseEditInputModel
            {
                Title = Convert.ToString(courseRow["Title"]),
                Description = Convert.ToString(courseRow["Description"]),
                ImagePath = Convert.ToString(courseRow["ImagePath"]),
                Email = Convert.ToString(courseRow["Email"]),
                FullPrice = new Money(
                    Enum.Parse<Currency>(Convert.ToString(courseRow["FullPrice_Currency"])),
                    Convert.ToDecimal(courseRow["FullPrice_Amount"])
                ),
                CurrentPrice = new Money(
                    Enum.Parse<Currency>(Convert.ToString(courseRow["CurrentPrice_Currency"])),
                    Convert.ToDecimal(courseRow["CurrentPrice_Amount"])
                ),
                Id = Convert.ToInt32(courseRow["Id"]),
                RowVersion = Convert.ToString(courseRow["RowVersion"])
            };
            return courseEditInputModel;
        }

        //per EFCore
        //permette di mappare tutti i valori trovati nell'entità course
        public static CourseEditInputModel FromEntity(Course course)
        {
            return new CourseEditInputModel {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Email = course.Email,
                ImagePath = course.ImagePath,
                CurrentPrice = course.CurrentPrice,
                FullPrice = course.FullPrice,
                RowVersion = course.RowVersion
            };
        }

    }

}