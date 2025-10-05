using Microsoft.EntityFrameworkCore;
using locadora.Data;
using locadora.Models;
using Microsoft.AspNetCore.Mvc;

namespace locadora.EndPoints
{
    public static class FuncionarioEndpoints
    {
        public static void MapFuncionarioEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/funcionarios").WithTags("Funcionários");

            // GET /api/funcionarios
            group.MapGet("/", async (LocadoraDbContext db) =>
                await db.Funcionarios.ToListAsync());

            // GET /api/funcionarios/{idFuncionario}
            group.MapGet("/{idFuncionario:int}", async (int idFuncionario, LocadoraDbContext db) =>
            {
                var funcionario = await db.Funcionarios.FindAsync(idFuncionario);
                return funcionario is not null ? Results.Ok(funcionario) : Results.NotFound("Funcionário não encontrado.");
            });

            // POST /api/funcionarios
            group.MapPost("/", async ([FromBody] Funcionario funcionario, LocadoraDbContext db) =>
            {
                if (string.IsNullOrEmpty(funcionario.Nome))
                {
                    return Results.BadRequest("O nome do funcionário é obrigatório.");
                }

                db.Funcionarios.Add(funcionario);
                await db.SaveChangesAsync();

                return Results.Created($"/api/funcionarios/{funcionario.IdFuncionario}", funcionario);
            });

            // PUT /api/funcionarios/{idFuncionario}
            group.MapPut("/{idFuncionario:int}", async (int idFuncionario, [FromBody] Funcionario funcionarioAtualizado, LocadoraDbContext db) =>
            {
                var funcionarioExistente = await db.Funcionarios.FindAsync(idFuncionario);
                if (funcionarioExistente is null)
                {
                    return Results.NotFound("Funcionário não encontrado.");
                }

                funcionarioExistente.Nome = funcionarioAtualizado.Nome;
                funcionarioExistente.Cargo = funcionarioAtualizado.Cargo;

                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            // DELETE /api/funcionarios/{idFuncionario}
            group.MapDelete("/{idFuncionario:int}", async (int idFuncionario, LocadoraDbContext db) =>
            {
                var funcionario = await db.Funcionarios.FindAsync(idFuncionario);
                if (funcionario is null)
                {
                    return Results.NotFound("Funcionário não encontrado.");
                }

                db.Funcionarios.Remove(funcionario);
                await db.SaveChangesAsync();
                return Results.Ok("Funcionário deletado com sucesso.");
            });

            // --- ROTAS DE FILTRO COM JOINS ---

            // INNER JOIN (realizado pelo .Where e .Include)
            app.MapGet("/api/funcionarios/{idFuncionario:int}/alugueis", async (int idFuncionario, LocadoraDbContext db) => {
                var alugueisDoFuncionario = await db.Alugueis
                    .Where(a => a.IdFuncionario == idFuncionario)
                    .Include(a => a.Cliente) // Opcional: incluir detalhes do cliente
                    .Include(a => a.Veiculo) // Opcional: incluir detalhes do veículo
                    .ToListAsync();

                return alugueisDoFuncionario.Any()
                    ? Results.Ok(alugueisDoFuncionario)
                    : Results.NotFound("Nenhum aluguel encontrado para este funcionário.");

            }).WithTags("Filtros Especiais");
        }
    }
}