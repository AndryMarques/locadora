using Microsoft.EntityFrameworkCore;
using locadora.Data;
using locadora.Models;
using Microsoft.AspNetCore.Mvc;
using locadora.DTOs;

namespace locadora.EndPoints
{
    public static class AluguelEndpoints
    {
        public static void MapAluguelEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/alugueis").WithTags("Aluguéis");

            // GET /api/alugueis
            group.MapGet("/", async (LocadoraDbContext db) =>
            {
                // Inclui dados do cliente, veículo (com fabricante) e funcionário
                var alugueis = await db.Alugueis
                    .Include(a => a.Cliente)
                    .Include(a => a.Veiculo)
                        .ThenInclude(v => v.Fabricante)
                    .Include(a => a.Funcionario)
                    .ToListAsync();
                return Results.Ok(alugueis);
            });

            // GET /api/alugueis/{idAluguel}
            group.MapGet("/{idAluguel:int}", async (int idAluguel, LocadoraDbContext db) =>
            {
                var aluguel = await db.Alugueis
                    .Include(a => a.Cliente)
                    .Include(a => a.Veiculo)
                        .ThenInclude(v => v.Fabricante)
                    .Include(a => a.Funcionario)
                    .FirstOrDefaultAsync(a => a.IdAluguel == idAluguel);

                return aluguel is not null ? Results.Ok(aluguel) : Results.NotFound("Aluguel não encontrado.");
            });

            // POST /api/alugueis
            group.MapPost("/", async ([FromBody] Aluguel aluguel, LocadoraDbContext db) =>
            {
                // Validação básica
                if (aluguel.IdCliente <= 0 || aluguel.IdVeiculo <= 0 || aluguel.IdFuncionario <= 0)
                {
                    return Results.BadRequest("IDs de Cliente, Veículo e Funcionário são obrigatórios.");
                }

                db.Alugueis.Add(aluguel);
                await db.SaveChangesAsync();

                return Results.Created($"/api/alugueis/{aluguel.IdAluguel}", aluguel);
            });

            // PUT /api/alugueis/{idAluguel}
            group.MapPut("/{idAluguel:int}", async (int idAluguel, [FromBody] Aluguel aluguelAtualizado, LocadoraDbContext db) =>
            {
                var aluguelExistente = await db.Alugueis.FindAsync(idAluguel);
                if (aluguelExistente is null)
                {
                    return Results.NotFound("Aluguel não encontrado.");
                }

                // Atualiza as propriedades que podem ser modificadas
                aluguelExistente.DataFimPrev = aluguelAtualizado.DataFimPrev;
                aluguelExistente.DataFimReal = aluguelAtualizado.DataFimReal;
                aluguelExistente.KmFim = aluguelAtualizado.KmFim;
                aluguelExistente.ValorFim = aluguelAtualizado.ValorFim;

                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            // DELETE /api/alugueis/{idAluguel}
            group.MapDelete("/{idAluguel:int}", async (int idAluguel, LocadoraDbContext db) =>
            {
                var aluguel = await db.Alugueis.FindAsync(idAluguel);
                if (aluguel is null)
                {
                    return Results.NotFound("Aluguel não encontrado.");
                }

                db.Alugueis.Remove(aluguel);
                await db.SaveChangesAsync();
                return Results.Ok("Aluguel deletado com sucesso.");
            });

            // --- ROTAS DE FILTRO COM JOINS ---

            // Este filtro busca todos os aluguéis que ainda não foram finalizados.
            app.MapGet("/api/alugueis/ativos", async (LocadoraDbContext db) => {
                var alugueisAtivos = await db.Alugueis
                    .Where(a => a.DataFimReal == null) // Filtra onde a data final não foi preenchida
                    .Include(a => a.Cliente)
                    .Include(a => a.Veiculo)
                    .ToListAsync();

                return Results.Ok(alugueisAtivos);
            }).WithTags("Filtros Especiais");
        }
    }
}