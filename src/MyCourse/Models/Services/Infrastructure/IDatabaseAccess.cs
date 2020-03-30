using System.Data;

namespace MyCourse.Models.Services.Infrastructure
{
    public interface IDatabaseAccess
    {
        DataSet ExecuteQuery(string query);
    }
}