using Application;
using Configuration.Client;
using Configuration.Provider;
using Configuration.Provider.DiExtensions;
using Infrastructure.Persistence;
using Presentation.Grpc;
using Presentation.Kafka;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddCustomConfigurationProvider(builder.Services);
builder.Services
    .AddConfigurationClientRefit(builder.Configuration)
    .AddConfigurationUpdateService(builder.Configuration);

builder.Services
    .AddApplicationServices()
    .AddInfrastructurePersistence(builder.Configuration)
    .AddMigrations()
    .AddPresentationGrpc()
    .AddPresentationKafka(builder.Configuration);

WebApplication app = builder.Build();

IConfigurationUpdateService updateService = app.Services.GetRequiredService<IConfigurationUpdateService>();
await updateService.UpdateOnceAsync(CancellationToken.None);

app.MapPresentationGrpc();
await app.Services.RunMigrations();

app.Run();