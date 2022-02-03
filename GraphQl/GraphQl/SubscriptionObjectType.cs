using GraphQl.Models;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;

namespace GraphQl
{
    public class SubscriptionObjectType
    {

        [Subscribe(With = nameof(SubscribeToFarmChangedAsync))]
        public Farm SubscribeFarm([EventMessage] Farm farm, int subscriptionId) => farm;

        public async ValueTask<ISourceStream<Farm>> SubscribeToFarmChangedAsync(
            int subscriptionId,
            [Service] ITopicEventReceiver eventReceiver,
            CancellationToken cancellationToken) =>
            await eventReceiver.SubscribeAsync<string, Farm>(
                "FarmChanged_" + subscriptionId, cancellationToken);
    }
}
