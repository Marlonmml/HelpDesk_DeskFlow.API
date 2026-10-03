using DeskFlow.API.Data;
using DeskFlow.API.Models;
using DeskFlow.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeskFlow.API.Repositories;

public class ChamadoRepository : Repository<Chamados>, IChamadoRepository
{
    public ChamadoRepository(AppDbContext context) : base(context) { }

    public async Task<Chamados> ObterComDetalhesAsync(int id)
    {
        return await _context.Chamados
            .Include(c => c.Categoria)
            .Include(c => c.Interacoes)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Chamados>> ListarComFiltrosAsync(StatusChamado? status, Prioridade? prioridade, int? categoriaId)
    {
        var query = _context.Chamados
            .Include(c => c.Categoria)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        if (prioridade.HasValue)
            query = query.Where(c => c.Prioridade == prioridade.Value);

        if (categoriaId.HasValue)
            query = query.Where(c => c.CategoriaId == categoriaId.Value);

        return await query.ToListAsync();
    }
}