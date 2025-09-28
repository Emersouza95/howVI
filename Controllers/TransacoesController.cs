using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using ImobiliariaAPI.Models;

namespace ImobiliariaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransacoesController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TransacoesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string ConnectionString => _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string not found");

        [HttpGet("dados-completos")]
        public ActionResult<IEnumerable<TransacaoCompleta>> GetDadosCompletos()
        {
            try
            {
                var dados = CarregarTodosOsDados();
                return Ok(new
                {
                    total = dados.Count,
                    dados = dados
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }

        [HttpGet("grafico-barras")]
        public ActionResult<object> GetGraficoBarras()
        {
            try
            {
                var dados = CarregarTodosOsDados();

                // PROCESSAMENTO EM MEMÓRIA usando programação funcional
                var valorPorImovel = dados
                    .GroupBy(d => new { d.CodigoImovel, d.DescricaoImovel })
                    .Select(g => new
                    {
                        CodigoImovel = g.Key.CodigoImovel,
                        DescricaoImovel = g.Key.DescricaoImovel,
                        ValorAcumulado = g.Sum(x => x.Valor)
                    })
                    .OrderByDescending(x => x.ValorAcumulado)
                    .ToList();

                return Ok(new
                {
                    tipo = "barras",
                    titulo = "Valor Acumulado por Imóvel",
                    dados = valorPorImovel
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }

        /// <summary>
        /// Gráfico de linhas: Total de transações por mês/ano (processado em memória)
        /// </summary>
        [HttpGet("grafico-linhas")]
        public ActionResult<object> GetGraficoLinhas()
        {
            try
            {
                var dados = CarregarTodosOsDados();

                // PROCESSAMENTO EM MEMÓRIA
                var transacoesPorMes = dados
                    .GroupBy(d => new
                    {
                        Ano = d.DataTransacao.Year,
                        Mes = d.DataTransacao.Month
                    })
                    .Select(g => new
                    {
                        Periodo = $"{g.Key.Mes:00}/{g.Key.Ano}",
                        TotalTransacoes = g.Sum(x => x.Valor),
                        QuantidadeTransacoes = g.Count()
                    })
                    .OrderBy(x => x.Periodo)
                    .ToList();

                return Ok(new
                {
                    tipo = "linhas",
                    titulo = "Transações por Mês/Ano",
                    dados = transacoesPorMes
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }

        /// <summary>
        /// Gráfico de pizza: Percentual por tipo de imóvel (processado em memória)
        /// </summary>
        [HttpGet("grafico-pizza")]
        public ActionResult<object> GetGraficoPizza()
        {
            try
            {
                var dados = CarregarTodosOsDados();

                // PROCESSAMENTO EM MEMÓRIA
                var totalGeral = dados.Sum(d => d.Valor);

                var percentualPorTipo = dados
                    .GroupBy(d => d.TipoImovel)
                    .Select(g => new
                    {
                        TipoImovel = g.Key,
                        ValorTotal = g.Sum(x => x.Valor),
                        Percentual = Math.Round((double)(g.Sum(x => x.Valor) / totalGeral) * 100, 2)
                    })
                    .OrderByDescending(x => x.ValorTotal)
                    .ToList();

                return Ok(new
                {
                    tipo = "pizza",
                    titulo = "Percentual de Transações por Tipo de Imóvel",
                    totalGeral = totalGeral,
                    dados = percentualPorTipo
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }

        /// <summary>
        /// Método privado que carrega TODOS os dados sem filtros SQL
        /// </summary>
        private List<TransacaoCompleta> CarregarTodosOsDados()
        {
            var dados = new List<TransacaoCompleta>();

            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                // SQL simples com JOIN - SEM WHERE nem GROUP BY (conforme escopo)
                // AJUSTE OS NOMES DAS TABELAS E COLUNAS PARA SEU BANCO
                string sql = @"
                    SELECT 
                        f.id_venda as Id,
                        f.codigo_imovel as ImovelId,
                        f.data_do_pagamento as DataTransacao,
                        f.valor_do_pagamento as Valor,
                        i.descricao_imovel as Endereco,
                        tp.nome as TipoImovel
                    FROM financeiro f
                    INNER JOIN imovel i ON f.codigo_imovel = i.codigo_imovel  
                    INNER JOIN tipo_imovel tp ON i.id_tipo_imovel = tp.id
                    ORDER BY f.data_do_pagamento DESC";

                using (var command = new MySqlCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dados.Add(new TransacaoCompleta
                        {
                            IdTransacao = Convert.ToInt32(reader["Id"]),
                            CodigoImovel = Convert.ToInt32(reader["ImovelId"]),
                            DataTransacao = Convert.ToDateTime(reader["DataTransacao"]),
                            Valor = Convert.ToDecimal(reader["Valor"]),
                            DescricaoImovel = reader["Endereco"].ToString() ?? string.Empty,
                            TipoImovel = reader["TipoImovel"].ToString() ?? string.Empty
                        });
                    }
                }
            }

            return dados;
        }
    }
}