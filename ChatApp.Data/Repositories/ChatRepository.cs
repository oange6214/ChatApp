using ChatApp.Data.Entities;
using ChatApp.Data.Interfaces;
using ChatApp.Domain.Models;
using Dapper;

namespace ChatApp.Data.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly IDbConnectionFactory _connection;

    public ChatRepository(IDbConnectionFactory connectionFactory)
    {
        _connection = connectionFactory;
    }

    public async Task<IEnumerable<ChatConversationEntity>> GetConversationsByContactNameAsync(string contactName)
    {
        string sql = @"SELECT * FROM conversations WHERE ContactName = @ContactName";
        object args = new { ContactName = contactName };

        using var connection = _connection.CreateConnection();

        return await connection.QueryAsync<ChatConversationEntity>(sql, args);
    }

    //public async Task<IEnumerable<ChatListItem>> GetContactAsync()
    //{
    //    const string sql = @"
    //                SELECT *
    //                FROM contacts p
    //                LEFT JOIN
    //                 (
    //                  SELECT a.*, ROW_NUMBER()
    //                   OVER(PARTITION BY a.contactname ORDER BY a.id DESC) AS seqnum
    //                  FROM conversations a
    //                 ) a
    //                 ON a.ContactName = p.contactname AND a.seqnum = 1
    //                ORDER BY a.Id DESC
    //            ";

    //    using var connection = _connection.CreateConnection();
    //    connection.Query<ContactEntity, ChatConversationEntity>(sql);
    //}
}