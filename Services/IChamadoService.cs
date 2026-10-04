using DeskFlow.API.Models.Entities;
using DeskFlow.API.Models;
using DeskFlow.API.Dtos;

namespace DeskFlow.API.Services;

public interface IChamadoService
{
    Task<Chamados> CriarAsync(ChamadoDto dto);
    Task<Chamados> IniciarAsync(int id);
    Task<Chamados> EncerrarAsync(int id, string solucao);
    Task<Interacao> AdicionarInteracaoAsync(int chamadoId, string autor, string mensagem);
    Task<Chamados> BuscarPorIdAsync(int id);
    Task<List<Chamados>> ListarAsync(StatusChamado? status, Prioridade? prioridade, int? categoriaId);
}