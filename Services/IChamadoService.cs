using DeskFlow.API.Models.Entities;

namespace DeskFlow.API.Services;

public interface IChamadoService
{
    Task<Chamados> CriarAsync(Chamados chamado);
    Task<Chamados> IniciarAsync(int id);
    Task<Chamados> FecharAsync(int id, string solucao);
}