namespace ImobiliariaAPI.Models
{
    public class TransacaoCompleta
    {
        public int IdTransacao { get; set; }
        public int CodigoImovel { get; set; }
        public DateTime DataTransacao { get; set; }
        public decimal Valor { get; set; }
        public string DescricaoImovel { get; set; } = string.Empty;
        public string TipoImovel { get; set; } = string.Empty;
    }
}