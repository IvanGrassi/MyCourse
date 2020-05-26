using System;
using System.Data;
using System.Threading.Tasks;

namespace MyCourse.Models.Services.Infrastructure
{
    public interface IDatabaseAccess
    {
        //servizio utilizzato per accedere al database (dialogare col db)

        Task<DataSet> ExecuteQueryAsync(FormattableString query);
        Task<T> ExecuteQueryScalarAsync<T>(FormattableString formattableQuery);
        Task<int> CommandAsync(FormattableString formattableCommand);
    }
}