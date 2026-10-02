using DeskFlow.API.Models;           
using DeskFlow.API.Models.Entities; 
using DeskFlow.API.Data;
using Microsoft.EntityFrameworkCore;

namespace DeskFlow.API.Services;

public class ChamadoService : IChamadoService
{
    private readonly AppDbContext _context;

    public ChamadoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Chamados> CriarAsync(Chamados chamado)
    {
        var novoChamado = new Chamados
        {
            Titulo = chamado.Titulo,
            Descricao = chamado.Descricao,
            Prioridade = chamado.Prioridade,
            SolicitanteNome = chamado.SolicitanteNome,
            CategoriaId = chamado.CategoriaId,
            Status = StatusChamado.Aberto,
            DataAbertura = DateTime.Now
        };

        _context.Chamados.Add(novoChamado);
        await _context.SaveChangesAsync();

        return novoChamado;

    }
    public async Task<Chamados> IniciarAsync(int id)
    {
        var chamado = await _context.Chamados.FirstOrDefaultAsync(c => c.Id == id);
        if (chamado is null)
            throw new InvalidOperationException("Não existe chamados em aberto");

        chamado.Status = StatusChamado.EmAndamento;
        await _context.SaveChangesAsync();

        return chamado;
    }

    public async Task<Chamados> FecharAsync(int id, string solucao)
    {
        var chamado = await _context.Chamados.FirstOrDefaultAsync(c => c.Id == id);
        if (chamado is null)
            throw new InvalidOperationException("Não existe chamados em aberto");

        chamado.Solucao = solucao;
        chamado.DataFechamento = DateTime.Now;
        chamado.Status = StatusChamado.Finalizado;
        await _context.SaveChangesAsync();

        return chamado;
    }
     public async Task<Chamados> BuscarPorIdAsync(int id)
    {
        var chamado = await _context.Chamados
            .Include(c => c.Categoria)
            .Include(c => c.Interacoes)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (chamado is null)
            throw new KeyNotFoundException("Chamado não encontrado");

        return chamado;
    }
    public async Task<Interacao> AdicionarInteracaoAsync(int chamadoId, string autor, string mensagem)
        {
            var chamado = await _context.Chamados.FirstOrDefaultAsync(c => c.Id == chamadoId);
            if (chamado is null)
            throw new KeyNotFoundException("Chamado não encontrado");

            if (chamado.Status == StatusChamado.Finalizado) 
            throw new InvalidOperationException("Não é possível adicionar interações em um chamado fechado");

            var interacao = new Interacao
        {
            ChamadoId = chamadoId,
            Autor = autor,
            Mensagem = mensagem,
            DataRegistro = DateTime.Now
        };

        _context.Interacoes.Add(interacao);
        await _context.SaveChangesAsync();

    return interacao;
    }
    public async Task<List<Chamados>> ListarAsync(StatusChamado? status, string prioridade, int? categoriaId)
    {
        var query = _context.Chamados
            .Include(c => c.Categoria)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(prioridade))
            query = query.Where(c => c.Prioridade == prioridade);

        if (categoriaId.HasValue)
            query = query.Where(c => c.CategoriaId == categoriaId.Value);

        return await query.ToListAsync();
    }

}
