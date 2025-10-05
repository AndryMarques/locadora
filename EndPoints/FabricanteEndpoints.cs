using Microsoft.EntityFrameworkCore;
using locadora.Data;
using locadora.Models;
using Microsoft.AspNetCore.Mvc;

namespace locadora.EndPoints
{
    public static class FabricanteEndpoints
    {
        public static void MapFabricanteEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/fabricantes").WithTags("Fabricantes");

            // GET /api/fabricantes
            group.MapGet("/", async (LocadoraDbContext db) =>
                await db.Fabricantes.ToListAsync());

            // GET /api/fabricantes/{idFabricante}
            group.MapGet("/{idFabricante:int}", async (int idFabricante, LocadoraDbContext db) =>
            {
                var fabricante = await db.Fabricantes.FindAsync(idFabricante);
                return fabricante is not null ? Results.Ok(fabricante) : Results.NotFound("Fabricante não encontrado.");
            });

            // POST /api/fabricantes
            group.MapPost("/", async ([FromBody] Fabricante fabricante, LocadoraDbContext db) =>
            {
                if (string.IsNullOrEmpty(fabricante.Nome))
                {
                    return Results.BadRequest("O nome do fabricante é obrigatório.");
                }

                db.Fabricantes.Add(fabricante);
                await db.SaveChangesAsync();

                return Results.Created($"/api/fabricantes/{fabricante.IdFabricante}", fabricante);
            });

            // PUT /api/fabricantes/{idFabricante}
            group.MapPut("/{idFabricante:int}", async (int idFabricante, [FromBody] Fabricante fabricanteAtualizado, LocadoraDbContext db) =>
            {
                var fabricanteExistente = await db.Fabricantes.FindAsync(idFabricante);
                if (fabricanteExistente is null)
                {
                    return Results.NotFound("Fabricante não encontrado.");
                }

                fabricanteExistente.Nome = fabricanteAtualizado.Nome;

                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            // DELETE /api/fabricantes/{idFabricante}
            group.MapDelete("/{idFabricante:int}", async (int idFabricante, LocadoraDbContext db) =>
            {
                var fabricante = await db.Fabricantes.FindAsync(idFabricante);
                if (fabricante is null)
                {
                    return Results.NotFound("Fabricante não encontrado.");
                }

                db.Fabricantes.Remove(fabricante);
                await db.SaveChangesAsync();
                return Results.Ok("Fabricante deletado com sucesso.");
            });

            // --- ROTAS DE FILTRO COM JOINS ---

            // INNER JOIN (realizado pelo .Include)
            // Este filtro busca um fabricante e já inclui todos os seus veículos na resposta.
            app.MapGet("/api/fabricantes/{idFabricante:int}/veiculos", async (int idFabricante, LocadoraDbContext db) => {
                var fabricanteComVeiculos = await db.Fabricantes
                    .Where(f => f.IdFabricante == idFabricante)
                    .Include(f => f.Veiculos) // Junta Fabricante com Veiculos
                    .FirstOrDefaultAsync();

                return fabricanteComVeiculos is not null
                    ? Results.Ok(fabricanteComVeiculos)
                    : Results.NotFound("Fabricante não encontrado.");
            }).WithTags("Filtros Especiais");
        }
    }
}