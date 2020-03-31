using System;
using System.Data;
using System.Threading.Tasks;

namespace MyCourse.Models.Services.Infrastructure
{
    public interface IDatabaseAccess
    {
        Task<DataSet> ExecuteQueryAsync(FormattableString query);
    }
}