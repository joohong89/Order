using Cart.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Orders.Models;

namespace Orders.Services
{
    public class OrderService
    {
        private readonly IMongoCollection<OrderMaster> _booksCollection;

        public OrderService(
            IOptions<OrderSetting> orderSetting)
        {

            //   HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
            //   Port = Convert.ToInt32(Environment.GetEnvironmentVariable("RABBITMQ_PORT"))

            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") == null ? Environment.GetEnvironmentVariable("CONNECTION_STRING") : orderSetting.Value.ConnectionString;

            var mongoClient = new MongoClient(
                orderSetting.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                orderSetting.Value.DatabaseName);

            _booksCollection = mongoDatabase.GetCollection<OrderMaster>(
                orderSetting.Value.OrdersCollectionName);
        }

        public async Task<List<OrderMaster>> GetAsync() =>
            await _booksCollection.Find(_ => true).ToListAsync();

        public async Task<OrderMaster?> GetAsync(string id) =>
            await _booksCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(OrderMaster order) =>
            await _booksCollection.InsertOneAsync(order);

        public async Task UpdateAsync(string id, OrderMaster order) =>
            await _booksCollection.ReplaceOneAsync(x => x.Id == id, order);

        public async Task RemoveAsync(string id) =>
            await _booksCollection.DeleteOneAsync(x => x.Id == id);

    }
}
