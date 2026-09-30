using DeskFlow.API.Data;
using DeskFlow.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeskFlow.API.Services;

public class CategoriaService : ICategoriaService
{
    private static readonly string[] CategoriasValidas =
        { "Hardware", "Software", "Redes", "Gestão de acesso" };

    private readonly AppDbContext _context;

    public CategoriaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Categorias> CriarAsync(string nome)
    {
        if (!CategoriasValidas.Contains(nome))
            throw new InvalidOperationException(
                $"Categoria inválida. Opções aceitas: {string.Join(", ", CategoriasValidas)}");

        var categoria = new Categorias { Nome = nome };

        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();

        return categoria;
    }

    public async Task<List<Categorias>> ListarAsync()
    {
        return await _context.Categorias.ToListAsync();
    }

    public async Task<Categorias> BuscarPorIdAsync(int id)
    {
        var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.Id == id);
        if (categoria is null)
            throw new KeyNotFoundException("Categoria não encontrada");

        return categoria;
    }

    public async Task<Categorias> AtualizarAsync(int id, string nome)
    {
        if (!CategoriasValidas.Contains(nome))
            throw new InvalidOperationException(
                $"Categoria inválida. Opções aceitas: {string.Join(", ", CategoriasValidas)}");

        var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.Id == id);
        if (categoria is null)
            throw new KeyNotFoundException("Categoria não encontrada");

        categoria.Nome = nome;
        await _context.SaveChangesAsync();

        return categoria;
    }

    public async Task RemoverAsync(int id)
    {
        var categoria = await _context.Categorias
            .Include(c => c.Chamados)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (categoria is null)
            throw new KeyNotFoundException("Categoria não encontrada");

        if (categoria.Chamados.Any())
            throw new InvalidOperationException(
                "Categoria possui chamados associados e não pode ser removida");

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();
    }
}