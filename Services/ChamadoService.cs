using DeskFlow.API.Models;
using DeskFlow.API.Models.Entities;
using DeskFlow.API.Repositories;
using DeskFlow.API.Dtos;

namespace DeskFlow.API.Services;

public class ChamadoService : IChamadoService
{
    private readonly IChamadoRepository _chamadoRepository;
    private readonly ICategoriaRepository _categoriaRepository;

    public ChamadoService(IChamadoRepository chamadoRepository, ICategoriaRepository categoriaRepository)
    {
        _chamadoRepository = chamadoRepository;
        _categoriaRepository = categoriaRepository;
    }

    public async Task<Chamados> CriarAsync(ChamadoDto dto)
{
    var categoriaExiste = await _categoriaRepository.ObterPorIdAsync(dto.CategoriaId);
    if (categoriaExiste is null)
        throw new InvalidOperationException("Categoria informada não existe");

    var novoChamado = new Chamados
    {
        Titulo = dto.Titulo,
        Descricao = dto.Descricao,
        Prioridade = dto.Prioridade,
        SolicitanteNome = dto.SolicitanteNome,
        CategoriaId = dto.CategoriaId,
        Status = StatusChamado.Aberto,
        DataAbertura = DateTime.Now
    };

    await _chamadoRepository.AdicionarAsync(novoChamado);
    return novoChamado;
}

    public async Task<Chamados> IniciarAsync(int id)
    {
        var chamado = await _chamadoRepository.ObterPorIdAsync(id);
        if (chamado is null)
            throw new KeyNotFoundException("Chamado não encontrado");

        chamado.Status = StatusChamado.EmAndamento;
        await _chamadoRepository.AtualizarAsync(chamado);

        return chamado;
    }

    public async Task<Chamados> EncerrarAsync(int id, string solucao)
{
    var chamado = await _chamadoRepository.ObterPorIdAsync(id);
    if (chamado is null)
        throw new KeyNotFoundException("Chamado não encontrado");

    chamado.Solucao = solucao;
    chamado.DataFechamento = DateTime.Now;
    chamado.Status = StatusChamado.Fechado; // lembrando do ajuste Finalizado → Fechado
    await _chamadoRepository.AtualizarAsync(chamado);

    return chamado;
}

    public async Task<Interacao> AdicionarInteracaoAsync(int chamadoId, string autor, string mensagem)
    {
        var chamado = await _chamadoRepository.ObterPorIdAsync(chamadoId);
        if (chamado is null)
            throw new KeyNotFoundException("Chamado não encontrado");

        if (chamado.Status == StatusChamado.Fechado)
            throw new InvalidOperationException("Não é possível adicionar interações em um chamado fechado");

        var interacao = new Interacao
        {
            ChamadoId = chamadoId,
            Autor = autor,
            Mensagem = mensagem,
            DataRegistro = DateTime.Now
        };

        chamado.Interacoes.Add(interacao);
        await _chamadoRepository.AtualizarAsync(chamado);

        return interacao;
    }

    public async Task<Chamados> BuscarPorIdAsync(int id)
    {
        var chamado = await _chamadoRepository.ObterComDetalhesAsync(id);
        if (chamado is null)
            throw new KeyNotFoundException("Chamado não encontrado");

        return chamado;
    }

    public async Task<List<Chamados>> ListarAsync(StatusChamado? status, Prioridade? prioridade, int? categoriaId)
    {
        return await _chamadoRepository.ListarComFiltrosAsync(status, prioridade, categoriaId);
    }
}