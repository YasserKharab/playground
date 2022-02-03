using Couchbase;
using Couchbase.KeyValue;

namespace GraphQl.Persistence.CB
{
    public interface ICouchbaseService
    {
        ICluster Cluster { get; }
        IBucket FeedlotSampleBucket { get; }
        ICouchbaseCollection FeedlotCollection { get; }

        public Task<ICouchbaseCollection> TenantColleciton(string tenant, string collection);
    }

    public class CouchbaseService : ICouchbaseService
    {
        public ICluster Cluster { get; private set; }

        public IBucket FeedlotSampleBucket { get; private set; }

        public ICouchbaseCollection FeedlotCollection { get; private set; }

        public CouchbaseService(string host, string user, string password)
        {
            try
            {
                var task = Task.Run(async () =>
                {
                    var options = new ClusterOptions()
                    .WithConnectionString("couchbase://localhost")
                    .WithCredentials(username: "Alice", password: "Pass123$")
                    .WithBuckets("travel-sample")
                    .WithLogging(LoggerFactory.Create(builder =>
                        {
                            builder.AddFilter("Couchbase", LogLevel.Debug)
                            .AddEventLog();
                        }));

                    var cluster = await Couchbase.Cluster.ConnectAsync(options);

                    Cluster = cluster;
                    FeedlotSampleBucket = await Cluster.BucketAsync("Feedlot");

                    var defaultScope = await FeedlotSampleBucket.ScopeAsync("_default");
                    FeedlotCollection = await defaultScope.CollectionAsync("_default");
                });

                task.Wait();
            }
            catch (AggregateException e)
            {
                e.Handle((x) => throw x);
            }
        }

        public async Task<ICouchbaseCollection> TenantColleciton(string tenant, string collection)
        {
            var tenantScope = await FeedlotSampleBucket.ScopeAsync(tenant);
            var tenantCollection = await tenantScope.CollectionAsync(collection);
            return tenantCollection;
        }
    }
}
