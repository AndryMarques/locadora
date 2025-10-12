using locadora.Data;
using locadora.DTOs;
using locadora.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace locadora.EndPoints
{
    public static class ClienteEndpoints
    {
        public static void MapClienteEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/clientes").WithTags("Clientes");

            // GET /api/clientes
            group.MapGet("/", async (LocadoraDbContext db) =>
                await db.Clientes.ToListAsync());

            // GET /api/clientes/{idCliente}
            group.MapGet("/{idCliente:int}", async (int idCliente, LocadoraDbContext db) =>
            {
                var cliente = await db.Clientes.FindAsync(idCliente);
                return cliente is not null ? Results.Ok(cliente) : Results.NotFound("Cliente não encontrado.");
            });

            // POST /api/clientes
            group.MapPost("/", async ([FromBody] CreateCliente clienteDTO, LocadoraDbContext db) =>
            {

                var novoCliente = new Cliente
                {
                    Nome = clienteDTO.Nome,
                    CPF = clienteDTO.CPF,
                    Email = clienteDTO.Email,
                    Telefone = clienteDTO.Telefone
                };

                if (string.IsNullOrEmpty(novoCliente.Nome) || string.IsNullOrEmpty(novoCliente.CPF) || string.IsNullOrEmpty(novoCliente.Email))
                {
                    return Results.BadRequest("Nome, CPF e Email são obrigatórios.");
                }

                db.Clientes.Add(novoCliente);
                await db.SaveChangesAsync();

                return Results.Created($"/api/clientes/{novoCliente.IdCliente}", novoCliente);
            });

            // PUT /api/clientes/{idCliente}
            group.MapPut("/{idCliente:int}", async (int idCliente, [FromBody] UpdateCliente clienteAtualizado, LocadoraDbContext db) =>
            {
                var clienteExistente = await db.Clientes.FindAsync(idCliente);
                if (clienteExistente is null)
                {
                    return Results.NotFound("Cliente não encontrado.");
                }

                clienteExistente.Nome = clienteAtualizado.Nome;
                clienteExistente.Email = clienteAtualizado.Email;
                clienteExistente.Telefone = clienteAtualizado.Telefone;

                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            // DELETE /api/clientes/{idCliente}
            group.MapDelete("/{idCliente:int}", async (int idCliente, LocadoraDbContext db) =>
            {
                var cliente = await db.Clientes.FindAsync(idCliente);
                if (cliente is null)
                {
                    return Results.NotFound("Cliente não encontrado.");
                }

                db.Clientes.Remove(cliente);
                await db.SaveChangesAsync();
                return Results.Ok("Cliente deletado com sucesso.");
            });

            // --- ROTAS DE FILTRO COM JOINS ---

            // INNER JOIN (realizado pelo .Include)
            // Este filtro busca todos os aluguéis de um cliente específico.
            app.MapGet("/api/clientes/{idCliente:int}/alugueis", async (int idCliente, LocadoraDbContext db) => {
                var clienteComAlugueis = await db.Clientes
                    .Where(c => c.IdCliente == idCliente)
                    .Include(c => c.Alugueis) // Junta Cliente com Alugueis
                        .ThenInclude(a => a.Veiculo) // Opcional: Junta Aluguel com Veiculo
                    .FirstOrDefaultAsync();

                return clienteComAlugueis is not null
                    ? Results.Ok(clienteComAlugueis)
                    : Results.NotFound("Cliente não encontrado.");
            }).WithTags("Filtros Especiais");
        }
    }
}
