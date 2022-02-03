using Demo;
using GraphQl;
using GraphQl.Models;
using GraphQl.Persistence.CB;
using Microsoft.EntityFrameworkCore;
using GraphQL.PreProcessingExtensions;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

builder.Services.AddPooledDbContextFactory<GQDbContext>(options =>
      options.UseSqlServer(configuration.GetConnectionString("DBConnection")));

builder.Services.AddSingleton<ICouchbaseService>(new CouchbaseService("127.0.0.1", "alice", "Pass123$"));
builder.Services.AddSingleton<IUsersService, UsersService>();
builder.Services.AddSingleton<IFarmService, FarmService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "cors_specs",
                      builder =>
                      {
                          builder.AllowAnyOrigin();
                          builder.AllowAnyHeader();
                          builder.AllowAnyMethod();
                      });
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddInMemorySubscriptions()
    .AddSubscriptionType<SubscriptionObjectType>()
    .AddProjections()
    .AddFiltering()
    .AddSorting()
    .AddPreProcessedResultsExtensions();

var app = builder.Build();

var contextFactory = builder.Services.BuildServiceProvider().GetRequiredService<IDbContextFactory<GQDbContext>>();
var context = contextFactory.CreateDbContext();

context.Database.EnsureCreated();

if (!context.Users.Any())
{
    var user = new User { Firstname = "Yasser", Lastname = "Kharab", Username = "yshinee",/* DateOfBirth = new DateTime(1993, 06, 08)*/ };
    context.Users.Add(user);
    //context.UserProfiles.Add(new UserProfile { Bio = ".Net Backend Developer", followersCount = 5, User = user });
}

context.SaveChanges();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("cors_specs");

app.UseWebSockets();

app.UseRouting().UseEndpoints(endpoints =>
      {
          endpoints.MapGraphQL();
      });

app.Run();


