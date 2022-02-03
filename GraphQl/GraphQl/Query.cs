using GraphQl;
using GraphQl.Models;
using GraphQl.Persistence.CB;
using HotChocolate.PreProcessingExtensions;

namespace Demo
{
    public class Query
    {
        ///
        /// GraphQl Schema for EF
        ///

        //[UseDbContext(typeof(GQDbContext))]
        //public IQueryable<User> GetUsers([ScopedService] GQDbContext context) => context.Users.AsQueryable();
        //[UseDbContext(typeof(GQDbContext))]
        //public IQueryable<UserProfile> GetUserProfiles([ScopedService] GQDbContext context) => context.UserProfiles.AsQueryable();

        ///
        /// GraphQl Schema for CB
        ///

        public async Task<IQueryable<User>> GetUsers([Service] IUsersService service, [GraphQLParams] IParamsContext graphQLParams)
        {
            // Authorization goes here.

            return await service.GetUsers(graphQLParams);
        }
        public async Task<User> GetUserById(int userId, [Service] IUsersService service, [GraphQLParams] IParamsContext graphQLParams) => await service.GetUserById(userId, graphQLParams);
        // For fake sign in only
        public async Task<User> GetUserByUsername(string username, [Service] IUsersService service, [GraphQLParams] IParamsContext graphQLParams) => await service.GetUserByUsername(username, graphQLParams);
        public async Task<IQueryable<Farm>> GetUserFarms(int userId, [Service] IUsersService service) => await service.GetUserFarms(userId);
        public async Task<Farm> GetFarmById(int farmId, [Service] IFarmService service, [GraphQLParams] IParamsContext graphQLParams) => await service.GetFarmById(farmId, graphQLParams);
        public async Task<IQueryable<Farm>> GetAllFarms([Service] IFarmService service, [GraphQLParams] IParamsContext graphQLParams) => await service.GetAllFarms(graphQLParams);

    }
}
