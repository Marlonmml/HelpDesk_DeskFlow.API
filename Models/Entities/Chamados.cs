namespace DeskFlow.API.Models.Entities;

public class Chamados
{
    public int Id { get; set; }
    public string Titulo { get; set; } 
    public string Descricao { get; set; }
    public string Prioridade { get; set; }
    public StatusChamado Status { get; set; } = StatusChamado.Aberto;
    public string SolicitanteNome { get; set; }
    public DateTime DataAbertura { get; set; }
    public DateTime DataFechamento { get; set; }
    public string Solucao { get; set; }
    public int CategoriaId { get; set; } 
    public Categorias Categoria { get; set; }
    public ICollection<Interacao> Interacoes { get; set; } = new List<Interacao>();
}