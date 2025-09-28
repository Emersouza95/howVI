namespace ImobiliariaAPI.Models
{
    public class Transacao
    {
        public int IdVenda { get; set; }
        public DateTime DataDoPagamento { get; set; }
        public decimal ValorDoPagamento { get; set; }
        public int CodigoImovel { get; set; }
        // Remover navegação para evitar JOINs automáticos  
        // public Imovel? Imovel { get; set; }
    }
}