using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyCourse.Models.Entities;
using MyCourse.Models.Exceptions;
using MyCourse.Models.Exceptions.Application;
using MyCourse.Models.InputModels;
using MyCourse.Models.Options;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Services.Application
{
    public class EfCoreCourseService : ICourseService
    {
        private readonly ILogger<EfCoreCourseService> logger;
        private readonly IImagePersister imagePersister;
        private readonly MyCourseDbContext dbContext;
        private readonly IOptionsMonitor<CoursesOptions> coursesOptions;

        public EfCoreCourseService(ILogger<EfCoreCourseService> logger, IImagePersister imagePersister, MyCourseDbContext dbContext, IOptionsMonitor<CoursesOptions> coursesOptions) //per esprimere la dipendenza del servizio applicativo dal servizio infrastutturale (MyCourseDbContext)
        {
            this.coursesOptions = coursesOptions;
            this.logger = logger;
            this.imagePersister = imagePersister;
            this.dbContext = dbContext;
        }

        public async Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
            IQueryable<CourseDetailViewModel> queryLinq = dbContext.Courses
                .AsNoTracking()
                .Include(course => course.Lessons)
                .Where(course => course.Id == id)
                .Select(course => CourseDetailViewModel.FromEntity(course)); //Usando metodi statici come FromEntity, la query potrebbe essere inefficiente. Mantenere il mapping nella lambda oppure usare un extension method personalizzato

            CourseDetailViewModel viewModel = await queryLinq.FirstOrDefaultAsync();
            //.FirstOrDefaultAsync(); //Restituisce null se l'elenco è vuoto e non solleva mai un'eccezione
            //.SingleOrDefaultAsync(); //Tollera il fatto che l'elenco sia vuoto e in quel caso restituisce null, oppure se l'elenco contiene più di 1 elemento, solleva un'eccezione
            //.FirstAsync(); //Restituisce il primo elemento, ma se l'elenco è vuoto solleva un'eccezione

            if (viewModel == null)
            {
                logger.LogWarning("Course {id} not found", id);
                throw new CourseNotFoundException(id);
            }

            return viewModel;
        }

        public async Task<ListViewModel<CourseViewModel>> GetCoursesAsync(CourseListInputModel model)
        {

            IQueryable<Course> baseQuery = dbContext.Courses;

            baseQuery = (model.OrderBy, model.Ascending) switch
            {
                ("Title", true) => baseQuery.OrderBy(course => course.Title),
                ("Title", false) => baseQuery.OrderByDescending(course => course.Title),               
                ("Rating", true) => baseQuery.OrderBy(course => course.Rating),
                ("Rating", false) => baseQuery.OrderByDescending(course => course.Rating),
                ("CurrentPrice", true) => baseQuery.OrderBy(course => course.CurrentPrice.Amount),
                ("CurrentPrice", false) => baseQuery.OrderByDescending(course => course.CurrentPrice.Amount),
                ("Id", true) => baseQuery.OrderBy(course => course.Id),
                ("Id", false) => baseQuery.OrderByDescending(course => course.Id),
                _ => baseQuery
            };

            //per ogni proprietà trovata nel CourseViewModel, dobbiamo assegnare il valore trovato nell'entità course
            IQueryable<Course> queryLinq = baseQuery
                .Where(course => course.Title.Contains(model.Search)) //che contiene il valore di search (ciò che cerca l'utente)
                .AsNoTracking();


            List<CourseViewModel> courses = await queryLinq     //vogliamo ottenere la lista dei corsi (skip e take agiscono qui)
                .Skip(model.Offset)
                .Take(model.Limit)
                .Select(course => CourseViewModel.FromEntity(course))
                /*.Select(course =>
                new CourseViewModel
                {
                    Id = course.Id,
                    Title = course.Title,
                    ImagePath = course.ImagePath,
                    Author = course.Author,
                    Rating = course.Rating,
                    CurrentPrice = course.CurrentPrice,
                    FullPrice = course.FullPrice
                })*/
                .ToListAsync(); //invoco IQueryable ad una List di CourseViewModel, EFC apre la connessione con il Db per inviare la query 


            int totalCount = await queryLinq.CountAsync();      //Conteggio di tutti i corsi esistenti

            //creo un istanza dell'oggetto
            ListViewModel<CourseViewModel> result = new ListViewModel<CourseViewModel>
            {
                Results = courses,          //contiene la lista dei corsi (paginata a 10 corsi per pagina)
                TotalCount = totalCount
            };

            return result;
        }

        //-----------------------------------------------------------------------------------
        //I due metodi rappresentano valori preimpostati e non manipolabili dall'utente
        public async Task<List<CourseViewModel>> GetBestRatingCoursesAsync()
        {
            CourseListInputModel inputModel = new CourseListInputModel(         //creo l'istanza
                search: "",
                page: 1,
                orderBy: "Rating",
                ascending: false,
                limit: coursesOptions.CurrentValue.InHome,  //rappresenta il totale dei corsi che verranno visualizzati (definito in appsettings.json)
                orderOptions: coursesOptions.CurrentValue.Order);

            //e la fornisco al metodo GetCourseAsync
            ListViewModel<CourseViewModel> result = await GetCoursesAsync(inputModel);
            return result.Results;
        }


        public async Task<List<CourseViewModel>> GetMostRecentCoursesAsync()
        {
            CourseListInputModel inputModel = new CourseListInputModel(
                search: "",
                page: 1,
                orderBy: "Id",  //sarebbe meglio per data di pubblicazione
                ascending: false,
                limit: coursesOptions.CurrentValue.InHome,
                orderOptions: coursesOptions.CurrentValue.Order);

            ListViewModel<CourseViewModel> result = await GetCoursesAsync(inputModel);
            return result.Results;
        }

        //-----------------------------------------Inserimento corsi----------------------------

        public async Task<CourseDetailViewModel> CreateCourseAsync(CourseCreateInputModel inputModel)
        {
            string title = inputModel.Title;
            string author = "Mario Verdi";

            var course = new Course(title, author);      //nuova istanza di course 
            dbContext.Add(course);                      //query di insert
            
            try
            {
                await dbContext.SaveChangesAsync();               //persiste la modifica in modo definito
            }
            //delle eccezioni del db, catturo solamnte la sqlitexception con codice 19 (i corsi sono unique)
            catch (DbUpdateException ex) when ((ex.InnerException as SqliteException)?.SqliteErrorCode == 19)
            {
                //eccezione personalizzata: creazione del corso fallita perché il titolo non era disponibile
                throw new CourseTitleUnavailableException(title, ex);
            }       
            //restituisco un istanza di CourseDetailVieModel tramite FromEntity
            return CourseDetailViewModel.FromEntity(course);
        }

    
        //verifica se il titolo é già in uso e inoltre viene passato l'id assegnato al corso
        public async Task<bool> IsTitleAvailableAsync(string title, int id)
        {
            //await dbContext.Courses.AnyAsyc(course => course.Title == title)
            
            //anyAsync restituisce true se esiste almeno una riga con il titolo digitato dall'utente
            //EF.Functions.Like: costrutto che consente di essere esplicito sul tipo di funzione (like) che voglio usare per comporre la query
            bool titleExists = await dbContext.Courses.AnyAsync(course => EF.Functions.Like(course.Title, title) && course.Id != id);
            //restituisco true SE il titolo é DISPONIBILE
            return !titleExists;
        }

        //-----------------------------------------Modifica corsi----------------------------

        public async Task<CourseEditInputModel> GetCourseForEditingAsync(int id)
        {
             IQueryable<CourseEditInputModel> queryLinq = dbContext.Courses
                .AsNoTracking()
                .Where(course => course.Id == id)
                .Select(course => CourseEditInputModel.FromEntity(course)); //Usando metodi statici come FromEntity, la query potrebbe essere inefficiente. Mantenere il mapping nella lambda oppure usare un extension method personalizzato

            CourseEditInputModel viewModel = await queryLinq.FirstOrDefaultAsync();

            if (viewModel == null)
            {
                logger.LogWarning("Course {id} not found", id);
                throw new CourseNotFoundException(id);
            }

            return viewModel;
        }

        public async Task<CourseDetailViewModel> EditCourseAsync(CourseEditInputModel inputModel)
        {
            //Recupero dell'entità Course
            //FindAsync: specializzato con le chiavi primarie, gli fornisco l'id
            Course course = await dbContext.Courses.FindAsync(inputModel.Id);

            if (course == null)
            {
                throw new CourseNotFoundException(inputModel.Id);
            }

            //...e lui ci recupererà il corso (collegato a quell'id) andando a richiamare i metodi presenti in Courses
            course.ChangeTitle(inputModel.Title);
            course.ChangePrices(inputModel.FullPrice, inputModel.CurrentPrice);
            course.ChangeDescription(inputModel.Description);
            course.ChangeEmail(inputModel.Email);

            //concorrenza ottimistica (prima di invocare il SaveAsync)
            //chiediamo il registro dell'entità (Entry(course) e la sua proprietà RowVersion a cui settiamo l'original value)
            //il valore fornito dall'input model é letto dal db
            dbContext.Entry(course).Property(course => course.RowVersion).OriginalValue = inputModel.RowVersion;

            if (inputModel.Image != null)
            {
                try{
                    string imagePath = await imagePersister.SaveCourseImageAsync(inputModel.Id, inputModel.Image);
                    //aggiorno solo l'image path con il nuovo percorso che é stato restituito
                    course.ChangeImagePath(imagePath);
                }
                catch(Exception ex) //immagine troppo grande!
                {
                    throw new CourseImageInvalidException(inputModel.Id, ex);
                }
                
            }

            //dbContext.Update(course); non necessario perché già l'entità course viene tracciata

            try
            {
                //SaveChangesAsync invia un comando update al database
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) 
            {
                throw new OptimisticConcurrencyException();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqliteException)?.SqliteErrorCode == 19)
            {
                throw new CourseTitleUnavailableException(inputModel.Title, ex);
            }

             return CourseDetailViewModel.FromEntity(course);
        }
    }
}