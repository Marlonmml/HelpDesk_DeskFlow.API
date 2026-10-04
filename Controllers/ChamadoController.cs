using DeskFlow.API.Models;
using DeskFlow.API.Models.Entities;
using DeskFlow.API.Services;
using Microsoft.AspNetCore.Mvc;
using DeskFlow.API.Dtos;

namespace DeskFlow.API.Controllers;

[ApiController]
[Route("api/chamados")]
public class ChamadoController : ControllerBase
{
    private readonly IChamadoService _chamadoService;

    public ChamadoController(IChamadoService chamadoService)
    {
        _chamadoService = chamadoService;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] ChamadoDto dto)
    {
        var novoChamado = await _chamadoService.CriarAsync(dto);
        return CreatedAtAction(nameof(BuscarPorId), new { id = novoChamado.Id }, novoChamado);
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] StatusChamado? status,
        [FromQuery] Prioridade? prioridade,
        [FromQuery] int? categoriaId)
    {
        var chamados = await _chamadoService.ListarAsync(status, prioridade, categoriaId);
        return Ok(chamados);
    }

    [HttpGet("{id}")]

    public async Task<IActionResult> BuscarPorId(int id)
    {
        var chamado = await _chamadoService.BuscarPorIdAsync(id);

        var resposta = new ChamadoResponseDto
        {
            Id = chamado.Id,
            Titulo = chamado.Titulo,
            Descricao = chamado.Descricao,
            Prioridade = chamado.Prioridade,
            Status = chamado.Status,
            SolicitanteNome = chamado.SolicitanteNome,
            DataAbertura = chamado.DataAbertura,
            DataFechamento = chamado.DataFechamento,
            Solucao = chamado.Solucao,
            CategoriaNome = chamado.Categoria?.Nome,
            Interacoes = chamado.Interacoes.Select(i => new InteracaoDto
            {
                Autor = i.Autor,
                Mensagem = i.Mensagem
            }).ToList()
        };

        return Ok(resposta);
    }
    [HttpPost("{id}/iniciar")]
    public async Task<IActionResult> Iniciar(int id)
    {
        var chamado = await _chamadoService.IniciarAsync(id);
        return Ok(chamado);
    }

    [HttpPost("{id}/encerrar")]
    public async Task<IActionResult> Encerrar(int id, [FromBody] FecharChamadoDto dto)
    {
        var chamado = await _chamadoService.EncerrarAsync(id, dto.Solucao);
        return Ok(chamado);
    }

    [HttpPost("{id}/interacoes")]
    public async Task<IActionResult> AdicionarInteracao(int id, [FromBody] InteracaoDto dto)
    {
        var interacao = await _chamadoService.AdicionarInteracaoAsync(id, dto.Autor, dto.Mensagem);
        return Ok(interacao);
    }
}