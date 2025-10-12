using Microsoft.EntityFrameworkCore;
using locadora.Data;
using locadora.EndPoints;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Configuração dos Serviços ---

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<LocadoraDbContext>(options =>
    options.UseSqlServer(connectionString));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "API Locadora", Version = "v1" });
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});


var app = builder.Build();

// --- 2. Configuração do Pipeline HTTP ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Locadora v1"));
}

// --- 3. Mapeamento dos Endpoints ---

app.MapVeiculoEndpoints();
app.MapFuncionarioEndpoints();
app.MapFabricanteEndpoints();
app.MapClienteEndpoints();
app.MapAluguelEndpoints();

// Endpoint raiz para verificar se a API está no ar
app.MapGet("/", () => "API Locadora está funcional. Acesse /swagger para testar os endpoints.")
    .ExcludeFromDescription(); 

app.Run();
