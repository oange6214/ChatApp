using System.Data;

namespace ChatApp.Data.Interfaces;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}