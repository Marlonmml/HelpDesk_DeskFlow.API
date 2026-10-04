using DeskFlow.API.Models.Entities;

namespace DeskFlow.API.Services;

public interface ICategoriaService
{
    Task<Categorias> CriarAsync(string nome);
    Task<List<Categorias>> ListarAsync();
    Task<Categorias> BuscarPorIdAsync(int id);
    Task<Categorias> AtualizarAsync(int id, string nome);

    Task RemoverAsync(int id);
}