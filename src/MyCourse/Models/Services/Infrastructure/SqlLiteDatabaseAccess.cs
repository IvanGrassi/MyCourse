using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace MyCourse.Models.Services.Infrastructure
{
    public class SqlLiteDatabaseAccess : IDatabaseAccess
    {
        //implementazione completa del servizio infrastrutturale (IDatabaseAccess)
        public async Task<DataSet> ExecuteQueryAsync(FormattableString formattableQuery)       //oggetto in grado di conservare in memoria una o piu tabelle di risultati che arrivano da un db relazionale
        {
            var queryArguments = formattableQuery.GetArguments();             //otteniamo gli argomenti della query (l'id che viene usato sia per la tabella dei Courses che delle Lessons)
            var sqlLiteParameters = new List<SqliteParameter>();              //creiamo una lista di parametri (Sqliteparameters)
            for (var i = 0; i < queryArguments.Length; i++)                   //cicla tutti gli argomenti (i due id)
            {
                var parameter = new SqliteParameter(i.ToString(), queryArguments[i]);
                sqlLiteParameters.Add(parameter);
                queryArguments[i] = "@" + i;                                  //il param. viene aggiunto alla lista e davanti a ogni param. viene aggiunta la @ (quindi diventa id = @id. ad es: id = @5)
            }
            string query = formattableQuery.ToString();



            using (var conn = new SqliteConnection("Data source=Data/MyCourse.db"))
            {
                await conn.OpenAsync();
                using (var cmd = new SqliteCommand(query, conn))
                {    //invio query al db
                    cmd.Parameters.AddRange(sqlLiteParameters);              //Aggiungiamo tutti i parametri passati qui sopra
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var dataSet = new DataSet();                        //invio dei dati dal db strutturali al servizio applicativo 
                        dataSet.EnforceConstraints = false;

                        //creaiamo un datatable per ogni table che abbiamo finché il reader non si chiude
                        do
                        {
                            var dataTable = new DataTable();
                            dataSet.Tables.Add(dataTable);
                            dataTable.Load(reader);                 //il datatable legge i dati trovati dal reader che esegue la query
                        } while (!reader.IsClosed);
                        return dataSet;
                    }
                }
            }
        }
    }
}