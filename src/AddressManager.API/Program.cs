using AddressManager.API.Filters;
using AddressManager.Infra.IoC;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;

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
        Description = "Esta API permite a consulta de informações de endereços a partir de um CEP.  \n\nEla se integra ao serviço da ViaCEP para buscar os dados de logradouro, bairro, cidade, estado e região, e em seguida, salva essas informações em um banco de dados local.  \n\nO principal objetivo deste projeto é demonstrar a habilidade de realizar integrações com APIs de terceiros de forma robusta e eficiente, utilizando o serviço da ViaCEP para enriquecer os dados e fornecer um serviço centralizado para gerenciamento de endereços.",
        Contact = new OpenApiContact { Name = "Renan Moreira", Email = "renan.h.s.moreira@gmail.com", Url = new Uri("https://github.com/renansantosm") },
    });

    var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));

}); 

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddMemoryCache();

builder.Services.AddMvc(options =>
{
    options.Filters.Add(new ExceptionFilter());
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


app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();