using DeskFlow.API.Models.Entities;
using Microsoft.AspNetCore.Mvc;

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
}