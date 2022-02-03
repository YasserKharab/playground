using Couchbase.Query;
using GraphQl.Models;
using HotChocolate.PreProcessingExtensions;

namespace GraphQl.Persistence.CB
{
    public interface IFarmService
    {
        Task<IQueryable<Farm>> GetAllFarms(IParamsContext contextParams);
        Task<Farm> GetFarmById(int id, IParamsContext contextParams);
        Task<Farm> UpdateFarm(int id, Dictionary<string, string> keyValueUpdate, IParamsContext graphQLParams);
    }

    public class FarmService : BaseService, IFarmService
    {
        private readonly ICouchbaseService _couchbaseService;

        public FarmService(ICouchbaseService couchbaseService)
        {
            _couchbaseService = couchbaseService;
        }

        public async Task<IQueryable<Farm>> GetAllFarms(IParamsContext contextParams)
        {
            var fields = GetFieldsSelection(contextParams, "farm_data");

            var query = $"SELECT DISTINCT(farm_data), {fields} FROM Feedlot AS farms UNNEST farms.Farms AS farm_data";

            var usersResult = await _couchbaseService.Cluster.QueryAsync<Farm>(query);

            if (usersResult.MetaData.Status != QueryStatus.Success)
            {
                Console.WriteLine(usersResult.Errors.OfType<string>());
                return null;
            }

            var farms = await usersResult.Rows.ToListAsync();

            return farms.AsQueryable();
        }

        public async Task<Farm> GetFarmById(int id, IParamsContext contextParams)
        {
            var fields = GetFieldsSelection(contextParams, "farm_data");

            var query = $"SELECT DISTINCT(farm_data), {fields} FROM Feedlot AS farms UNNEST farms.Farms AS farm_data WHERE farm_data.Id = {id}";

            var usersResult = await _couchbaseService.Cluster.QueryAsync<Farm>(query);

            if (usersResult.MetaData.Status != QueryStatus.Success)
            {
                Console.WriteLine(usersResult.Errors.OfType<string>());
                return null;
            }

            var farms = await usersResult.Rows.FirstOrDefaultAsync();

            return farms;
        }

        public async Task<Farm> UpdateFarm(int id, Dictionary<string, string> keyValueUpdate, IParamsContext graphQLParams)
        {
            var fieldQuery = "";

            foreach (var field in keyValueUpdate)
            {
                fieldQuery += $"f.{field.Key} = '{field.Value}' ";
            }

            var query = $"UPDATE Feedlot SET {fieldQuery} FOR f IN Farms WHEN f.Id = {id} END";

            var usersResult = await _couchbaseService.Cluster.QueryAsync<int>(query);

            if (usersResult.MetaData.Status != QueryStatus.Success)
            {
                Console.WriteLine(usersResult.Errors.OfType<string>());
                return null;
            }

            return await GetFarmById(id, graphQLParams);
        }
    }
}
