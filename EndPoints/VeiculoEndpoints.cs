using locadora.Data;
using locadora.DTOs;
using locadora.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace locadora.EndPoints
{
    public static class VeiculoEndpoints
    {
        public static void MapVeiculoEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/veiculos").WithTags("Veículos");

            // GET /api/veiculos
            group.MapGet("/", async (LocadoraDbContext db) =>
                await db.Veiculos.Include(v => v.Fabricante).ToListAsync());

            // GET /api/veiculos/{id}
            group.MapGet("/{id:int}", async (int id, LocadoraDbContext db) =>
            {
                var veiculo = await db.Veiculos.Include(v => v.Fabricante).FirstOrDefaultAsync(v => v.IdVeiculo == id);
                return veiculo is not null ? Results.Ok(veiculo) : Results.NotFound("Veículo não encontrado.");
            });

            // POST /api/veiculos
            group.MapPost("/", async ([FromBody] CreateVeiculo veiculoDTO, LocadoraDbContext db) =>
            {

                var novoViculo = new Veiculo 
                {
                    Modelo = veiculoDTO.Modelo,
                    Ano = veiculoDTO.Ano,
                    KM = veiculoDTO.KM,
                    IdFabricante = veiculoDTO.IdFabricante
                };

                // Validação simples (pode ser expandida com bibliotecas como FluentValidation)
                if (string.IsNullOrEmpty(veiculoDTO.Modelo) || veiculoDTO.Ano <= 0)
                {
                    return Results.BadRequest("Modelo e Ano são obrigatórios e o ano deve ser um valor positivo.");
                }

                db.Veiculos.Add(novoViculo);
                await db.SaveChangesAsync();

                return Results.Created($"/api/veiculos/{novoViculo.IdVeiculo}", novoViculo);
            });

            // PUT /api/veiculos/{id}
            group.MapPut("/{idVeiculo:int}", async (int idVeiculo, [FromBody] UpdateVeiculo veiculoAtualizado, LocadoraDbContext db) =>
            {
                var veiculoExistente = await db.Veiculos.FindAsync(idVeiculo);
                if (veiculoExistente is null)
                {
                    return Results.NotFound("Veículo não encontrado.");
                }

                // Atualiza as propriedades
                veiculoExistente.Modelo = veiculoAtualizado.Modelo;
                veiculoExistente.Ano = veiculoAtualizado.Ano;
                veiculoExistente.KM = veiculoAtualizado.KM; 
                veiculoExistente.IdFabricante = veiculoAtualizado.IdFabricante; 

                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            // DELETE /api/veiculos/{id}
            group.MapDelete("/{id:int}", async (int id, LocadoraDbContext db) =>
            {
                var veiculo = await db.Veiculos.FindAsync(id);
                if (veiculo is null)
                {
                    return Results.NotFound("Veículo não encontrado.");
                }

                db.Veiculos.Remove(veiculo);
                await db.SaveChangesAsync();
                return Results.Ok("Veículo deletado com sucesso.");
            });

            // --- ROTAS DE FILTRO COM JOINS ---

            // LEFT JOIN (realizado pelo .Include)
            app.MapGet("/api/alugueis/detalhados", async (LocadoraDbContext db) => {
                var alugueis = await db.Alugueis
                    .Include(a => a.Cliente) 
                    .Include(a => a.Veiculo) 
                        .ThenInclude(v => v.Fabricante) 
                    .ToListAsync();

                return Results.Ok(alugueis);
            }).WithTags("Filtros Especiais");

            // INNER JOIN (realizado pelo .Where)
            app.MapGet("/api/veiculos/porfabricante/{idFabricante:int}", async (int idFabricante, LocadoraDbContext db) => {
                var veiculos = await db.Veiculos
                   .Where(v => v.IdFabricante == idFabricante)
                   .Include(v => v.Fabricante)
                   .ToListAsync();

                return veiculos.Any() ? Results.Ok(veiculos) : Results.NotFound("Nenhum veículo encontrado para este fabricante.");
            }).WithTags("Filtros Especiais");
        }
    }
}