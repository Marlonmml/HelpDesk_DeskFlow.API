using DeskFlow.API.Models.Entities;
using DeskFlow.API.Repositories;

namespace DeskFlow.API.Services;

public class CategoriaService : ICategoriaService
{
    private static readonly string[] CategoriasValidas =
        { "Hardware", "Software", "Redes", "Gestão de acesso" };

    private readonly ICategoriaRepository _categoriaRepository;

    public CategoriaService(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public async Task<Categorias> CriarAsync(string nome)
    {
        if (!CategoriasValidas.Contains(nome))
            throw new InvalidOperationException(
                $"Categoria inválida. Opções aceitas: {string.Join(", ", CategoriasValidas)}");

        var categoria = new Categorias { Nome = nome };
        await _categoriaRepository.AdicionarAsync(categoria);

        return categoria;
    }

    public async Task<List<Categorias>> ListarAsync()
    {
        return await _categoriaRepository.ListarAsync();
    }

    public async Task<Categorias> BuscarPorIdAsync(int id)
    {
        var categoria = await _categoriaRepository.ObterPorIdAsync(id);
        if (categoria is null)
            throw new KeyNotFoundException("Categoria não encontrada");

        return categoria;
    }

    public async Task<Categorias> AtualizarAsync(int id, string nome)
    {
        if (!CategoriasValidas.Contains(nome))
            throw new InvalidOperationException(
                $"Categoria inválida. Opções aceitas: {string.Join(", ", CategoriasValidas)}");

        var categoria = await _categoriaRepository.ObterPorIdAsync(id);
        if (categoria is null)
            throw new KeyNotFoundException("Categoria não encontrada");

        categoria.Nome = nome;
        await _categoriaRepository.AtualizarAsync(categoria);

        return categoria;
    }

    public async Task RemoverAsync(int id)
    {
        var categoria = await _categoriaRepository.ObterComChamadosAsync(id);
        if (categoria is null)
            throw new KeyNotFoundException("Categoria não encontrada");

        if (categoria.Chamados.Any())
            throw new InvalidOperationException(
                "Categoria possui chamados associados e não pode ser removida");

        await _categoriaRepository.RemoverAsync(categoria);
    }
}