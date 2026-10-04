using DeskFlow.API.Models;
namespace DeskFlow.API.Dtos;

public class ChamadoResponseDto
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public Prioridade Prioridade { get; set; }
    public StatusChamado Status { get; set; }
    public string SolicitanteNome { get; set; }
    public DateTime DataAbertura { get; set; }
    public DateTime DataFechamento { get; set; }
    public string Solucao { get; set; }
    public string CategoriaNome { get; set; }
    public List<InteracaoDto> Interacoes { get; set; }
}