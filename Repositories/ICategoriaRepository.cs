using DeskFlow.API.Models.Entities;

namespace DeskFlow.API.Repositories;

public interface ICategoriaRepository : IRepository<Categorias>
{
    Task<Categorias> ObterComChamadosAsync(int id);
}