using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyCourse.Models.Exceptions.Infrastructure;
using MyCourse.Models.Options;
using MyCourse.Models.ValueTypes;

namespace MyCourse.Models.Services.Infrastructure
{
    public class SqlLiteDatabaseAccess : IDatabaseAccess
    {
        private readonly ILogger<SqlLiteDatabaseAccess> logger;
        private readonly IOptionsMonitor<ConnectionStringsOptions> connectionStringsOptions;

        public SqlLiteDatabaseAccess(ILogger<SqlLiteDatabaseAccess> logger, IOptionsMonitor<ConnectionStringsOptions> connectionStringsOptions) //Ricevo il servizio dal costruttore
        {
            this.logger = logger;
            this.connectionStringsOptions = connectionStringsOptions;       //Conservo il riferimento al servizio su un campo privato...
        }

        public async Task<int> CommandAsync(FormattableString formattableCommand)
        {
            try
            {
            using SqliteConnection conn = await GetOpenedConnection();
            using SqliteCommand cmd = GetCommand(formattableCommand, conn);
            int affectedRows = await cmd.ExecuteNonQueryAsync();
            return affectedRows;
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                throw new ConstraintViolationException(ex);
            }

        }

        public async Task<T> ExecuteQueryScalarAsync<T>(FormattableString formattableQuery)
        {
            using SqliteConnection conn = await GetOpenedConnection();
            using SqliteCommand cmd = GetCommand(formattableQuery, conn);
            object result = await cmd.ExecuteScalarAsync();
            return (T)Convert.ChangeType(result, typeof(T));        //change type permette di fornire un argomento di tipo object (result) e indicare il tipo verso il quale convertirlo
        }

        //implementazione completa del servizio infrastrutturale (IDatabaseAccess)
        public async Task<DataSet> ExecuteQueryAsync(FormattableString formattableQuery)       //oggetto in grado di conservare in memoria una o piu tabelle di risultati che arrivano da un db relazionale
        {
            logger.LogInformation(formattableQuery.Format, formattableQuery.GetArguments());

            using SqliteConnection conn = await GetOpenedConnection();
            using SqliteCommand cmd = GetCommand(formattableQuery, conn);

            //Inviamo la query al database e otteniamo un SqLiteDataReader
            //per leggere i risultati

             using var reader = await cmd.ExecuteReaderAsync();

             var dataSet = new DataSet();                        //invio dei dati dal db strutturali al servizio applicativo 
             //dataSet.EnforceConstraints = false;                 //Fix per evitare un bug del provider Microsoft.Data.Sqlite

             //creaiamo un datatable per ogni table che abbiamo finché il reader non si chiude
             do
             {
                 var dataTable = new DataTable();
                 dataSet.Tables.Add(dataTable);
                 dataTable.Load(reader);                 //il datatable legge i dati trovati dal reader che esegue la query
             } while (!reader.IsClosed);
             return dataSet;
        }

        private static SqliteCommand GetCommand(FormattableString formattableQuery, SqliteConnection conn)
        {
            //Creiamo dei SqliteParameter a partire dalla FormattableString
            var queryArguments = formattableQuery.GetArguments();
            var sqliteParameters = new List<SqliteParameter>();
            for (var i = 0; i < queryArguments.Length; i++)
            {
                if (queryArguments[i] is Sql)
                {
                    continue;
                }
                //se il valore é null, restituisco null tramite operatore ??
                var parameter = new SqliteParameter(name: i.ToString(), value: queryArguments[i] ?? DBNull.Value); 
                sqliteParameters.Add(parameter);
                queryArguments[i] = "@" + i;
            }
            string query = formattableQuery.ToString();

            var cmd = new SqliteCommand(query, conn);
            //Aggiungiamo i SqliteParameters al SqliteCommand
            cmd.Parameters.AddRange(sqliteParameters);
            return cmd;
        }

        private async Task<SqliteConnection> GetOpenedConnection()
        {
            //Colleghiamoci al db Sqlite, inviamo la query e leggiamo i risultati
            //NON dobbiamo più scrivere la stringa, ma usiamo Default
            var conn = new SqliteConnection(connectionStringsOptions.CurrentValue.Default);
            await conn.OpenAsync();
            return conn;
        }
    }
}