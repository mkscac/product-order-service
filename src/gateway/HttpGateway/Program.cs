using HttpGateway.Clients;
using HttpGateway.DiExtensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPresentationGrpcClients(builder.Configuration)
    .AddProcessingGrpcClients(builder.Configuration)
    .AddSwaggerSettings()
    .AddMiddlewares()
    .AddControllers()
    .AddCustomJsonOptions();

WebApplication app = builder.Build();
app
    .UseSwaggerSettings()
    .UseMiddlewares();
app.MapControllers();

app.Run();