using ImobiliariaAPI.Data;
using ImobiliariaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ImobiliariaAPI.Services
{
    public class TransacaoService
    {
        private readonly ImobiliariaContext _context;

        public TransacaoService(ImobiliariaContext context)
        {
            _context = context;
        }

        public async Task<List<TransacaoCompleta>> ObterTransacoesCompletas()
        {
            // Carregar TODOS os dados na memória (sem WHERE, sem JOIN no SQL)
            var todasTransacoes = await _context.Financeiro.ToListAsync();
            var todosImoveis = await _context.Imoveis.ToListAsync();
            var todosTipos = await _context.TipoImoveis.ToListAsync();

            // Usar programação funcional: map, filter, reduce, forEach
            var resultado = new List<TransacaoCompleta>();

            // Processar cada transação usando forEach
            todasTransacoes.ForEach(transacao =>
            {
                // Usar filter para encontrar o imóvel correspondente
                var imovel = todosImoveis.Where(i => i.CodigoImovel == transacao.CodigoImovel).FirstOrDefault();

                if (imovel != null)
                {
                    // Usar filter para encontrar o tipo do imóvel
                    var tipoImovel = todosTipos.Where(t => t.Id == imovel.IdTipoImovel).FirstOrDefault();

                    if (tipoImovel != null)
                    {
                        // Usar map (transformação) para criar TransacaoCompleta
                        var transacaoCompleta = new TransacaoCompleta
                        {
                            IdTransacao = transacao.IdVenda,
                            DataTransacao = transacao.DataDoPagamento,
                            Valor = transacao.ValorDoPagamento,
                            CodigoImovel = imovel.CodigoImovel,
                            DescricaoImovel = imovel.DescricaoImovel,
                            TipoImovel = tipoImovel.Nome
                        };

                        resultado.Add(transacaoCompleta);
                    }
                }
            });

            // Ordenar por data (processamento na memória)
            return resultado.OrderBy(t => t.DataTransacao).ToList();
        }
    }
}