using GraphQl;
using GraphQl.Models;
using GraphQl.Persistence.CB;
using HotChocolate.PreProcessingExtensions;
using HotChocolate.Subscriptions;

namespace Demo
{
    public class Mutation
    {
        public async Task<Farm> UpdateFarm(
            [Service] IFarmService service,
            [GraphQLParams] IParamsContext graphQLParams,
            [Service] ITopicEventSender eventSender,
            int farmId, 
            Dictionary<string, string> keyValueUpdate)
        {
            var result = await service.UpdateFarm(farmId, keyValueUpdate, graphQLParams);

            await eventSender.SendAsync("FarmChanged_" + farmId, result);

            return result;
        }
    }
}
