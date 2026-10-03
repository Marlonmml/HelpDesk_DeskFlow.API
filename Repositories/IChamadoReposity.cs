using DeskFlow.API.Models;
using DeskFlow.API.Models.Entities;

namespace DeskFlow.API.Repositories;

public interface IChamadoRepository : IRepository<Chamados>
{
    Task<Chamados> ObterComDetalhesAsync(int id);
    Task<List<Chamados>> ListarComFiltrosAsync(StatusChamado? status, Prioridade? prioridade, int? categoriaId);
}