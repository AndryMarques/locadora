using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using locadora.Data;
using locadora.Models;
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
            group.MapPost("/", async ([FromBody] CreateAluguel aluguelDTO, LocadoraDbContext db) =>
            {
                var novoAluguel = new Aluguel
                {
                    DataIni = aluguelDTO.DataIni,
                    DataFimPrev = aluguelDTO.DataFimPrev,
                    KmIni = aluguelDTO.KmIni,
                    ValorDia = aluguelDTO.ValorDia,
                    IdCliente = aluguelDTO.IdCliente,
                    IdVeiculo = aluguelDTO.IdVeiculo,
                    IdFuncionario = aluguelDTO.IdFuncionario
                };

                if (novoAluguel.DataFimPrev <= novoAluguel.DataIni)
                {
                    return Results.BadRequest("A data prevista de fim deve ser posterior à data de início.");
                }

                db.Alugueis.Add(novoAluguel);
                await db.SaveChangesAsync();

                return Results.Created($"/api/alugueis/{novoAluguel.IdAluguel}", novoAluguel);
            });

            // PUT /api/alugueis/{idAluguel}
            group.MapPut("/{idAluguel:int}", async (int idAluguel, [FromBody] UpdateAluguel aluguelAtualizado, LocadoraDbContext db) =>
            {
                var aluguelExistente = await db.Alugueis.FindAsync(idAluguel);
                if (aluguelExistente is null)
                {
                    return Results.NotFound("Aluguel não encontrado.");
                }

                // Atualiza as propriedades que podem ser modificadas
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