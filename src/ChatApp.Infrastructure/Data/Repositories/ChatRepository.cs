using ChatApp.Core.Entities;
using ChatApp.Core.Interfaces;
using Dapper;
using Microsoft.Extensions.Logging;

namespace ChatApp.Infrastructure.Data.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<ChatRepository> _logger;

        public ChatRepository(IDbConnectionFactory connectionFactory, ILogger<ChatRepository> logger)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Conversation>> GetConversationsByContactNameAsync(string contactName)
        {
            if (string.IsNullOrWhiteSpace(contactName))
                throw new ArgumentException("Contact name cannot be null or empty", nameof(contactName));

            const string sql = @"
                SELECT *
                FROM conversations
                WHERE ContactName = @ContactName";

            var parameters = new
            {
                ContactName = contactName,
            };

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                return await connection.QueryAsync<Conversation>(sql, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching conversations for contact {ContactName}", contactName);
                throw;
            }
        }

        public async Task<IEnumerable<(Contact, Conversation)>> GetContactsWithLatestConversationsAsync()
        {
            const string sql = @"
                    SELECT *
                    FROM contacts p
                    LEFT JOIN
	                    (
		                    SELECT a.*,
			                    ROW_NUMBER() OVER(PARTITION BY a.contactname ORDER BY a.id DESC) AS seqnum
		                    FROM conversations a
	                    ) a ON a.ContactName = p.contactname AND a.seqnum = 1
                    ORDER BY a.Id DESC";

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.QueryAsync<Contact, Conversation, (Contact, Conversation)>(
                sql,
                    (contact, conversation) => (contact, conversation),
                    splitOn: "Id");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching contacts with latest conversations");
                throw;
            }
        }
    }
}