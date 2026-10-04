using DeskFlow.API.Models;

namespace DeskFlow.API.Dtos;
public class ChamadoDto
{
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public Prioridade Prioridade { get; set; }
    public string SolicitanteNome { get; set; }
    public int CategoriaId { get; set; }
}