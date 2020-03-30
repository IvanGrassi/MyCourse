using System.Data;
using Microsoft.Data.Sqlite;

namespace MyCourse.Models.Services.Infrastructure
{
    public class SqlLiteDatabaseAccess : IDatabaseAccess
    {
        //implementazione completa del servizio infrastrutturale (IDatabaseAccess)
        public DataSet ExecuteQuery(string query)       //oggetto in grado di conservare in memoria una o piu tabelle di risultati che arrivano da un db relazionale
        {
            using (var conn = new SqliteConnection("Data source=Data/MyCourse.db"))
            {
                conn.Open();
                var cmd = new SqliteCommand(query, conn);       //invio query al db
                using (var reader = cmd.ExecuteReader())
                {
                    var dataSet = new DataSet();            //invio dei dati dal db strutturali al servizio applicativo 
                    dataSet.EnforceConstraints = false;
                    var dataTable = new DataTable();
                    dataSet.Tables.Add(dataTable);
                    dataTable.Load(reader);                 //il datatable legge i dati trovati dal reader che esegue la query
                    return dataSet;
                }

            }
        }
    }
}