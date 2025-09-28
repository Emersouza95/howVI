namespace ImobiliariaAPI.Models
{
    public class Imovel
    {
        public int CodigoImovel { get; set; }
        public string DescricaoImovel { get; set; } = string.Empty;
        public int IdTipoImovel { get; set; }
        // Remover navegação para evitar JOINs automáticos
        // public TipoImovel? TipoImovel { get; set; }
    }
}