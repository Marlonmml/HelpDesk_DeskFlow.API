using DeskFlow.API.Models.Entities;
using DeskFlow.API.Services;
using Microsoft.AspNetCore.Mvc;
using DeskFlow.API.Models;
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
    public async Task<IActionResult> Criar([FromBody] Chamados chamado)
    {
        var novoChamado = await _chamadoService.CriarAsync(chamado);
        return Ok(novoChamado);
    }
    // Controllers/ChamadoController.cs (método novo)
    [HttpPost("{id}/interacoes")]
    public async Task<IActionResult> AdicionarInteracao(int id, [FromBody] InteracaoDto dto)
    {
        try
        {
            var interacao = await _chamadoService.AdicionarInteracaoAsync(id, dto.Autor, dto.Mensagem);
            return Ok(interacao);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        try
        {
            var chamado = await _chamadoService.BuscarPorIdAsync(id);
            return Ok(chamado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
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
}
