using System;
using System.Collections.Generic;
using MyCourse.Models.Enums;
using MyCourse.Models.ValueTypes;

namespace MyCourse.Models.Entities
{
    public partial class Course
    {
        public Course(string title, string author)
        {
            if (string.IsNullOrWhiteSpace(title))   //se il campo title viene lasciato vuoto, sollevo un eccezione
            {
                throw new ArgumentException("The course must have a title");
            }
            if (string.IsNullOrWhiteSpace(author))
            {
                throw new ArgumentException("The course must have an author");
            }

            Title = title;      //questi due sono campi OBBLIGATORI
            Author = author;
            Lessons = new HashSet<Lesson>();
            CurrentPrice = new Money(Currency.EUR, 0);  //valuta di default (se non viene aggiunta): EUR con prezzo 0
            FullPrice = new Money(Currency.EUR, 0);
            ImagePath = "/Courses/default.png";         //immagine di default (se non viene aggiunta)
        }   

        //le proprietà sono private per evitare che dall'esterno si forniscano valori invalidi
        public int Id { get; private set; }    //PK
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public string Author { get; private set; }
        public string Email { get; private set; }
        public double Rating { get; private set; }
        public Money FullPrice { get; private set; }
        public Money CurrentPrice { get; private set; }

        //proprietà di navigazione: di un corso, ci permette di accedere alle sue lezioni

        //metodo che permette di gestire il nuovo titolo e validare il campo in modo che non sia vuoto
        public void ChangeTitle(string newTitle)
        {
            if (string.IsNullOrWhiteSpace(newTitle))
            {
                throw new ArgumentException("The course must have a title");
            }
            Title = newTitle;
        }

        //metodo che permette il cambiamento di entrambi i prezzi (dove il fullprice DEVE essere > al discountprice)
        public void ChangePrices(Money newFullPrice, Money newCurrentPrice)
        {
            if (newFullPrice == null || newCurrentPrice == null)       //verifica che nessuno dei due valori sia nullo
            {
                throw new ArgumentException("Prices can't be null");
            }
            if (newFullPrice.Currency != newCurrentPrice.Currency)     //verifica che sia la stessa valuta
            {
                throw new ArgumentException("Currencies don't match");
            }
            if (newFullPrice.Amount < newCurrentPrice.Amount)
            {
                throw new ArgumentException("Full price can't be less than the current price");
            }
            FullPrice = newFullPrice;
            CurrentPrice = newCurrentPrice;
        }

        public void ChangeDescription(string newDescription)
        {
            if (newDescription != null)
            {
                if (newDescription.Length < 20)
                {
                    throw new Exception("Description is too short");
                }
                else if (newDescription.Length > 4000)
                {
                    throw new Exception("Description is too long");
                }
            }
            Description = newDescription;
        }

        public void ChangeEmail(string newEmail)
        {
            if (string.IsNullOrEmpty(newEmail))
            {
                throw new ArgumentException("Email can't be empty");
            }
            Email = newEmail;
        }

        public void ChangeImagePath(string imagePath)
        {
            ImagePath = imagePath;
        }

        //Relazioni
        public virtual ICollection<Lesson> Lessons { get; private set; } //permette di passare dal corso verso le lezioni correlate ad esso
    }
}
