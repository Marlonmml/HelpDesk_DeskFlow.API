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
}