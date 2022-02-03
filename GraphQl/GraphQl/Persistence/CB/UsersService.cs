using Couchbase.KeyValue;
using Couchbase.Query;
using GraphQl.Models;
using HotChocolate.PreProcessingExtensions;

namespace GraphQl.Persistence.CB
{
    public interface IUsersService
    {
        Task<IQueryable<User>> GetUsers(IParamsContext contextParams);
        Task<User> GetUserById(int id, IParamsContext contextParams);
        Task<User> GetUserByUsername(string username, IParamsContext contextParams);
        Task<IQueryable<Farm>> GetUserFarms(int userId);
    }

    public class UsersService : BaseService, IUsersService
    {
        private readonly ICouchbaseService _couchbaseService;

        public UsersService(ICouchbaseService couchbaseService)
        { 
            _couchbaseService = couchbaseService;
        }
        public async Task<IQueryable<Farm>> GetUserFarms(int userId)
        {
            var farmsResult = await _couchbaseService.FeedlotCollection.LookupInAsync(userId.ToString(), new List<LookupInSpec> { LookupInSpec.Get("Farms") });

            var farms = farmsResult.ContentAs<ICollection<Farm>>(0);

            return farms.AsQueryable();
        }

        public async Task<User> GetUserById(int id, IParamsContext contextParams)
        {
            var fields = GetFieldsSelection(contextParams, "Feedlot");

            var query = $"SELECT {fields} FROM Feedlot WHERE Id = {id}";

            var usersResult = await _couchbaseService.Cluster.QueryAsync<User>(query);

            if (usersResult.MetaData.Status != QueryStatus.Success)
            {
                Console.WriteLine(usersResult.Errors.OfType<string>());
                return null;
            }

            var user = await usersResult.Rows.FirstAsync();

            return user;

        }

        public async Task<User> GetUserByUsername(string username, IParamsContext contextParams)
        {
            var fields = GetFieldsSelection(contextParams, "Feedlot");

            var query = $"SELECT {fields} FROM Feedlot WHERE Username = '{username}'";

            var usersResult = await _couchbaseService.Cluster.QueryAsync<User>(query);

            if (usersResult.MetaData.Status != QueryStatus.Success)
            {
                Console.WriteLine(usersResult.Errors.OfType<string>());
                return null;
            }

            var user = await usersResult.Rows.FirstOrDefaultAsync();

            return user;

        }

        public async Task<IQueryable<User>> GetUsers(IParamsContext contextParams)
        {
            var fields = GetFieldsSelection(contextParams, "Feedlot");

            var query = $"SELECT {fields} FROM Feedlot";

            var usersResult = await _couchbaseService.Cluster.QueryAsync<User>(query);

            if (usersResult.MetaData.Status != QueryStatus.Success)
            {
                Console.WriteLine(usersResult.Errors.OfType<string>());
                return null;
            }

            var users = await usersResult.Rows.ToListAsync();

            Console.WriteLine($"N1QL query - scoped to inventory: {query}");

            return users.AsQueryable();
        }
    }
}
