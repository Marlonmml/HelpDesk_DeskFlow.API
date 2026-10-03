using DeskFlow.API.Data;
using DeskFlow.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeskFlow.API.Repositories;

public class CategoriaRepository : Repository<Categorias>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context) { }

    public async Task<Categorias> ObterComChamadosAsync(int id)
    {
        return await _context.Categorias
            .Include(c => c.Chamados)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}