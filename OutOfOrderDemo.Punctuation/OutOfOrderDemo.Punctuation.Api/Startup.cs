using MassTransit;
using OutOfOrderDemo.Punctuation.Common;

namespace OutOfOrderDemo.Punctuation.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddScoped<MongoConnection>(sp =>
            new MongoConnection("mongodb://localhost:27017"));

        services.AddScoped<BaseRepository<Author>>(sp =>
        {
            return new BaseRepository<Author>(
                sp.GetRequiredService<MongoConnection>()
                    .GetCollection<Author>("Authors", "Library"));
        });

        services.AddScoped<BaseRepository<Book>>(sp =>
        {
            return new BaseRepository<Book>(
                sp.GetRequiredService<MongoConnection>()
                    .GetCollection<Book>("Books", "Library"));
        });

        // add mass transit to publish events
        services.AddMassTransit(cfg =>
        {
            cfg.UsingRabbitMq((context, r) =>
            {
                r.Host("localhost", 5672, "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                r.ConfigureEndpoints(context);
            });
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
