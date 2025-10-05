using Microsoft.EntityFrameworkCore;
using locadora.Data;
using locadora.EndPoints; // Corrigido o namespace
using locadora.Models;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Configura��o dos Servi�os ---

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<LocadoraDbContext>(options =>
    options.UseSqlServer(connectionString));

// Adiciona os servi�os do Swagger para documenta��o e teste da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "API Locadora", Version = "v1" });
});


var app = builder.Build();

// --- 2. Configura��o do Pipeline HTTP ---

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

// Endpoint raiz para verificar se a API est� no ar
app.MapGet("/", () => "API Locadora est� funcional. Acesse /swagger para testar os endpoints.")
    .ExcludeFromDescription(); 

app.Run();
