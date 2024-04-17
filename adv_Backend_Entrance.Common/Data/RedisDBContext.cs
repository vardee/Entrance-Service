using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.Data
{
    public class RedisDBContext
    {
        private readonly IDatabase _db;

        public RedisDBContext(string connectionString)
        {
            var connection = ConnectionMultiplexer.Connect(connectionString);
            _db = connection.GetDatabase();
        }

        public async Task AddToken(string token)
        {
            await _db.StringSetAsync(token, "blacklisted", TimeSpan.FromMinutes(10));
        }

        public async Task<bool> IsBlackToken(string token)
        {
            return await _db.KeyExistsAsync(token);
        }
    }
}
