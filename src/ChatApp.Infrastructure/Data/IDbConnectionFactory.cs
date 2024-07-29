using System.Data;

namespace ChatApp.Infrastructure.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}