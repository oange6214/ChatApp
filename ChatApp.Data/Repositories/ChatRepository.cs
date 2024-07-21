using ChatApp.Data.Interfaces;
using ChatApp.Domain.Models;
using Dapper;

namespace ChatApp.Data.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ChatRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> AddConversationAsync(ChatConversationEntity conversation)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(@"
                INSERT INTO conversations (ContactName, ReceivedMessage, MsgReceivedOn, SentMessage, MsgSentOn, IsMessageReceived)
                VALUES (@ContactName, @ReceivedMessage, @MsgReceivedOn, @SentMessage, @MsgSentOn, @IsMessageReceived)",
            conversation);
    }

    public async Task<bool> DeleteConversationAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        int affectedRows = await connection.ExecuteAsync(
            "DELETE FROM conversations WHERE Id = @Id", new { Id = id });
        return affectedRows > 0;
    }

    public async Task<IEnumerable<ChatConversationEntity>> GetAllConversationsAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<ChatConversationEntity>(
            "SELECT * FROM conversations ORDER BY MsgSentOn DESC");
    }

    public async Task<ChatConversationEntity> GetConversationByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<ChatConversationEntity>(
            "SELECT * FROM conversations WHERE Id = @Id", new { Id = id });
    }

    public async Task<IEnumerable<ChatConversationEntity>> GetConversationsByContactNameAsync(string contactName)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<ChatConversationEntity>(
            "SELECT * FROM conversations WHERE ContactName = @ContactName",
            new { ContactName = contactName });
    }

    public async Task<DateTime?> GetLastMessageTimeAsync(string contactName)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<DateTime?>(
            "SELECT TOP 1 MsgSentOn FROM conversations WHERE ContactName = @ContactName ORDER BY MsgSentOn DESC",
            new { ContactName = contactName });
    }

    public async Task<IEnumerable<ChatConversationEntity>> GetRecentConversationsAsync(int count)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<ChatConversationEntity>(
            "SELECT TOP (@Count) * FROM conversations ORDER BY MsgSentOn DESC",
            new { Count = count });
    }

    public async Task<int> GetTotalMessageCountAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM conversations");
    }

    public async Task<bool> UpdateConversationAsync(ChatConversationEntity conversation)
    {
        using var connection = _connectionFactory.CreateConnection();
        int affectedRows = await connection.ExecuteAsync(@"
                UPDATE conversations
                SET ContactName = @ContactName,
                    ReceivedMessage = @ReceivedMessage,
                    MsgReceivedOn = @MsgReceivedOn,
                    SentMessage = @SentMessage,
                    MsgSentOn = @MsgSentOn,
                    IsMessageReceived = @IsMessageReceived
                WHERE Id = @Id",
            conversation);
        return affectedRows > 0;
    }
}