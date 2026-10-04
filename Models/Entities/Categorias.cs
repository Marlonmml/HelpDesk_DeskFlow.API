namespace DeskFlow.API.Models.Entities;

public class Categorias
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public ICollection<Chamados> Chamados { get; set; } = new List<Chamados>();
}