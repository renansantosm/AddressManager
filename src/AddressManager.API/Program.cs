using AddressManager.API.Middlewares;
using AddressManager.Infra.Data.Extensions;
using AddressManager.Infra.IoC;
using Asp.Versioning;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddOpenApi(options =>
     options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0
);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Address Manager",
        Description = "Esta API permite o cadastro de endereços a partir de um CEP informado pelo cliente, com campos opcionais para complementar os dados (número, referência, complemento).  " +
        "\n\nEla se integra automaticamente ao serviço ViaCEP para buscar os dados base (logradouro, bairro, cidade, estado) e cria um novo registro completo no banco de dados." +
        "\n\nO projeto demonstra integração resiliente com APIs externas, implementando cache inteligente, tratamento de exceções e validações robustas para criar um serviço flexível de cadastro de endereços.",
        Contact = new OpenApiContact { Name = "Renan Moreira", Email = "renan.h.s.moreira@gmail.com", Url = new Uri("https://github.com/renansantosm") },
    });

    var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
;

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddMemoryCache();

builder.Host.UseSerilog((context, configuration) =>
{
    configuration
    .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} <s:{SourceContext}>{NewLine}{Exception}", 
            theme: AnsiConsoleTheme.Literate
    )
    .ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AddressManager API v1");
    });
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();  

DatabaseExtensions.AddDatabase(app);

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();