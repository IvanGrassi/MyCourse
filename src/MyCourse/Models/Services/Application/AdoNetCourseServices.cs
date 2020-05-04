using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyCourse.Models.Exceptions;
using MyCourse.Models.InputModels;
using MyCourse.Models.Options;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.ValueTypes;
using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Services.Application
{
    public class AdoNetCourseServices : ICourseService
    {
        private readonly ILogger<AdoNetCourseServices> logger;
        private readonly IDatabaseAccess db;
        private readonly IOptionsMonitor<CoursesOptions> coursesOptions;

        public AdoNetCourseServices(ILogger<AdoNetCourseServices> logger, IDatabaseAccess db, IOptionsMonitor<CoursesOptions> coursesOptions) //dipende dai due servizi infrastrutturali
        {
            this.coursesOptions = coursesOptions;
            this.logger = logger;
            this.db = db;
        }

        public async Task<CourseDetailViewModel> GetCourseAsync(int id)
        {

            logger.LogInformation("Course {id} requested", id); //permette di filtrare i log in base al valore degli id

            //la prima query carica il corso scelto al rispettivo id, la seconda invece carica tutte le lezioni legate all'id di quel corso
            //FormattableString separa la parte fissa dai suoi parametri
            FormattableString query = $@"SELECT Id, Title, Description, ImagePath, Author, Rating, FullPrice_Amount, FullPrice_Currency, CurrentPrice_Amount, CurrentPrice_Currency FROM Courses WHERE Id={id} 
            ; SELECT Id, Title, Description, Duration FROM Lessons WHERE CourseID={id}";

            DataSet dataSet = await db.ExecuteQueryAsync(query);

            //Course: 
            //estrae i dati dalla prima datatable, se Rows >1 allora c'é un errore in quanto ci può essere solo 1 Row (evita la SqlInjection)
            var courseTable = dataSet.Tables[0];
            if (courseTable.Rows.Count != 1)
            {
                logger.LogWarning("Course {id} not found", id); //messaggio di log, problema non grave
                throw new CourseNotFoundException(id);
            }
            var courseRow = courseTable.Rows[0]; //leggiamo la riga (courseRow), la passiamo a FromDataRow che farà la mappatura tra il datarow restituendo un oggetto di tipo courseDetailViewModel
            var courseDetailViewModel = CourseDetailViewModel.FromDataRow(courseRow);

            //Lessons
            var lessonDataTable = dataSet.Tables[1];

            foreach (DataRow lessonRow in lessonDataTable.Rows)
            {
                LessonViewModel lessonViewModel = LessonViewModel.FromDataRow(lessonRow);
                courseDetailViewModel.Lessons.Add(lessonViewModel); //ogni oggetto di LessonViewModel trovato viene aggiunto alla lista
            }
            return courseDetailViewModel;
        }

        public async Task<ListViewModel<CourseViewModel>> GetCoursesAsync(CourseListInputModel model)
        {
            string orderBy = model.OrderBy == "CurrentPrice" ? "CurrentPrice_Amount" : model.OrderBy;
            string direction = model.Ascending ? "ASC" : "DESC";  //se é true restituisce ASC, se é false: DESC

            //quali informazioni estrarre nei confronti di un database? eseguo una query
            FormattableString query = $@"SELECT Id, Title, ImagePath, Author, Rating, FullPrice_Amount, FullPrice_Currency, CurrentPrice_Amount, CurrentPrice_Currency FROM Courses WHERE Title LIKE {"%" + model.Search + "%"} ORDER BY {(Sql) orderBy} {(Sql) direction} LIMIT {model.Limit} OFFSET {model.Offset};
            SELECT COUNT(*) FROM Courses WHERE Title LIKE {"%" + model.Search + "%"}";
            DataSet dataSet = await db.ExecuteQueryAsync(query);
            var dataTable = dataSet.Tables[0]; //primo datatable
            var courseList = new List<CourseViewModel>(); //aggiungo CourseViewModel ad una lista
            foreach (DataRow courseRow in dataTable.Rows) //il datatable contiene tutte le righe trovate e cicliamo tutte le Rows in un foreach
            {
                CourseViewModel courseViewModel = CourseViewModel.FromDataRow(courseRow); //il metodo FromDataRow conterrà tutti i risultati ciclati e viene richamato. Il tutto mi fa ottenere un oggetto di tipo CourseViewModel
                courseList.Add(courseViewModel); //ogni oggetto ciclato viene aggiunto alla lista                
            }

            //creo un istanza dell'oggetto
            ListViewModel<CourseViewModel> result = new ListViewModel<CourseViewModel>
            {
                Results = courseList,   //rappresenta l'elenco dei corsi (paginato a 10 corsi per pagina)
                TotalCount = Convert.ToInt32(dataSet.Tables[1].Rows[0][0])        //leggo il secondo dataset [1] dove viene eseguita la seconda query e della prima riga ottengo i valore della prima colonna, totale di tutti i corsi
            };

            return result;
        }

        //----------------------------------------------------------------------
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

        //--------------------------------Inserimento corso-----------------------------------------

        public async Task<CourseDetailViewModel> CreateCourseAsync(CourseCreateInputModel inputModel)
        {
            //uso il servizio infrastrutturare per rivolgergli la query
            string title = inputModel.Title;    //cio che l'utente inserisce
            string author = "Mario Rossi";

            
            try{
            //definisce alcuni valori predefiniti per evitare di inserire un campo vuoto
            var dataSet = await db.ExecuteQueryAsync($@"INSERT INTO Courses (Title, Author, ImagePath, CurrentPrice_Currency, CurrentPrice_Amount, FullPrice_Currency, FullPrice_Amount) VALUES ({title}, {author}, 'Courses/default.png', 'EUR', 0, 'EUR', 0);
                                                    SELECT last_insert_rowid();");
            
            int courseId = Convert.ToInt32(dataSet.Tables[0].Rows[0][0]);

            //fornisco l'id a una chiamata GetCourseAsync per ottenere tutto l'oggetto CourseDetailViewModel(Author, description, ecc...)
            CourseDetailViewModel course = await GetCourseAsync(courseId); 
            return course;

            //catturo solamnte la sqlitexception con codice 19 (i corsi sono unique)
            } catch(SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                //eccezione personalizzata: creazione del corso fallita perché il titolo é già in utilizzo da un altro corso
                throw new CourseTitleUnavailableException(title, ex);
            }

            
        }

        //verifica se il titolo é già in uso e inoltre viene passato l'id assegnato al corso
        public async Task<bool> IsTitleAvailableAsync(string title, int id)
        {
            //invio una query che conteggia tutte le righe che contengono lo stesso input dell'utente (per evitare i doppioni)
            //l'id dovrà essere diverso da quello fornito dall'esterno (verifico i restanti corsi ma non quello che sto modificando attualmente)

            DataSet result = await db.ExecuteQueryAsync($"SELECT COUNT(*) FROM Courses WHERE Title LIKE {title} AND id<>{id}");
            bool titleAvailable = Convert.ToInt32(result.Tables[0].Rows[0][0]) == 0; //titolo disponibile (non duplicato) se il conteggio é 0
            return titleAvailable;
        }

        //--------------------------------Modifica corso-----------------------------------------

        public async Task<CourseEditInputModel> GetCourseForEditingAsync(int id)
        {
            //seleziona tutti i campi modificabili
            FormattableString query = $@"SELECT Id, Title, Description, ImagePath, Email, FullPrice_Amount, FullPrice_Currency, CurrentPrice_Amount, CurrentPrice_Currency FROM Courses WHERE Id={id}";

            DataSet dataSet = await db.ExecuteQueryAsync(query);

            var courseTable = dataSet.Tables[0];
            if (courseTable.Rows.Count != 1)                //se vengono trovate 0 righe: corso ... non trovato
            {
                logger.LogWarning("Course {id} not found", id);
                throw new CourseNotFoundException(id);      //richiama la classe dove é stata dichiarata l'eccezione
            }
            var courseRow = courseTable.Rows[0];
            //mapping sul CourseEditInputModel
            var courseEditInputModel = CourseEditInputModel.FromDataRow(courseRow);
            return courseEditInputModel;
        }

        public async Task<CourseDetailViewModel> EditCourseAsync(CourseEditInputModel inputModel)
        {
            DataSet dataSet = await db.ExecuteQueryAsync($"SELECT COUNT(*) FROM Courses WHERE Id={inputModel.Id}");
            if (Convert.ToInt32(dataSet.Tables[0].Rows[0][0]) == 0)
            {
                throw new CourseNotFoundException(inputModel.Id);
            }

            try
            {
                dataSet = await db.ExecuteQueryAsync($"UPDATE Courses SET Title={inputModel.Title}, Description={inputModel.Description}, Email={inputModel.Email}, CurrentPrice_Currency={inputModel.CurrentPrice.Currency}, CurrentPrice_Amount={inputModel.CurrentPrice.Amount}, FullPrice_Currency={inputModel.FullPrice.Currency}, FullPrice_Amount={inputModel.FullPrice.Amount} WHERE Id={inputModel.Id}");
                CourseDetailViewModel course = await GetCourseAsync(inputModel.Id);     //restituisco l'istanza di CourseDetailViewModel DOPO l'aggionrnamento dei dati
                return course;
            }
            //catturo solamente la sqlitexception con codice 19 (i corsi sono unique)
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                //eccezione personalizzata: creazione del corso fallita perché il titolo é già in utilizzo da un altro corso
                throw new CourseTitleUnavailableException(inputModel.Title, ex);
            }
        }
    }
}