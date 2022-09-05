using MassTransit;
using OutOfOrderDemo.Punctuation.Api;
using OutOfOrderDemo.Punctuation.Common;
using System.Reflection;

namespace OutOfOrderDemo.Punctuation.Listener;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddScoped<MongoConnection>(sp =>
                    new MongoConnection("mongodb://localhost:27017"));

                services.AddScoped<BaseRepository<BookExpanded>>(sp =>
                {
                    return new BaseRepository<BookExpanded>(
                        sp.GetRequiredService<MongoConnection>()
                            .GetCollection<BookExpanded>("Books", "LibraryQuery"));
                });

                services.AddScoped<BaseRepository<BookAuthor>>(sp =>
                {
                    return new BaseRepository<BookAuthor>(
                        sp.GetRequiredService<MongoConnection>()
                            .GetCollection<BookAuthor>("Authors", "LibraryQuery"));
                });

                services.AddMassTransit(cfg =>
                {
                    cfg.AddConsumers(Assembly.GetExecutingAssembly());

                    cfg.UsingRabbitMq((context, r) =>
                    {
                        r.Host("localhost", 5672, "/", h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        });

                        //r.MessageTopology.SetEntityNameFormatter(r.MessageTopology.EntityNameFormatter);
                        r.ConfigureEndpoints(context);
                    });

                });
            });
}
