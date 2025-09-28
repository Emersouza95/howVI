using Microsoft.EntityFrameworkCore;
using ImobiliariaAPI.Models;

namespace ImobiliariaAPI.Data
{
    public class ImobiliariaContext : DbContext
    {
        public ImobiliariaContext(DbContextOptions<ImobiliariaContext> options) : base(options)
        {
        }

        public DbSet<TipoImovel> TipoImoveis { get; set; }
        public DbSet<Imovel> Imoveis { get; set; }
        public DbSet<Transacao> Financeiro { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar TipoImovel
            modelBuilder.Entity<TipoImovel>(entity =>
            {
                entity.ToTable("tipo_imovel");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nome).HasColumnName("nome").HasMaxLength(50);
            });

            // Configurar Imovel
            modelBuilder.Entity<Imovel>(entity =>
            {
                entity.ToTable("imovel");
                entity.HasKey(e => e.CodigoImovel);
                entity.Property(e => e.CodigoImovel).HasColumnName("codigo_imovel");
                entity.Property(e => e.DescricaoImovel).HasColumnName("descricao_imovel");
                entity.Property(e => e.IdTipoImovel).HasColumnName("id_tipo_imovel");
            });

            // Configurar Transacao (tabela financeiro no banco)
            modelBuilder.Entity<Transacao>(entity =>
            {
                entity.ToTable("financeiro");
                entity.HasKey(e => e.IdVenda);
                entity.Property(e => e.IdVenda).HasColumnName("id_venda");
                entity.Property(e => e.DataDoPagamento).HasColumnName("data_do_pagamento");
                entity.Property(e => e.ValorDoPagamento).HasColumnName("valor_do_pagamento").HasColumnType("decimal(10,2)");
                entity.Property(e => e.CodigoImovel).HasColumnName("codigo_imovel");
            });
        }
    }
}