namespace DeskFlow.API.Repositories;

public interface IRepository<T> where T : class
{
    Task<T> ObterPorIdAsync(int id);
    Task<List<T>> ListarAsync();
    Task AdicionarAsync(T entity);
    Task AtualizarAsync(T entity);
    Task RemoverAsync(T entity);
}