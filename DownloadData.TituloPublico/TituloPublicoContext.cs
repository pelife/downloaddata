using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace DownloadData.TituloPublico
{
    class TituloPublicoContext : DbContext
    {
        public TituloPublicoContext(string nomeStringConexao) : base (nomeStringConexao)
        {
            // Turn off the Migrations, (NOT a code first Db)
            Database.SetInitializer<TituloPublicoContext>(null);
            Database.Log = s => System.Diagnostics.Debug.Write(s);            
        }

        public DbSet<CotacaoTituloPublicoBCB> CotacaoTituloPublicoBCB { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.Add(new CotacaoTituloPublicoBCBTypeConfiguration());
        }
    }

    internal class CotacaoTituloPublicoBCBTypeConfiguration : EntityTypeConfiguration<CotacaoTituloPublicoBCB>
    {
        public CotacaoTituloPublicoBCBTypeConfiguration()
        {
            //chave
            HasKey(k => new { k.DataCotacao, k.CodigoTitulo });

            //colunas
            Property(p => p.CodigoTitulo).HasColumnName("sg_titulo");
            Property(p => p.TipoTitulo).HasColumnName("tp_titulo");
            Property(p => p.DataCotacao).HasColumnName("dt_valor");
            Property(p => p.Vencimento).HasColumnName("dt_vencimento");
            Property(p => p.TaxaCompra).HasColumnName("vl_taxa_compra");
            Property(p => p.TaxaVenda).HasColumnName("vl_taxa_venda");
            Property(p => p.PrecoUnitarioCompra).HasColumnName("vl_pu_compra");
            Property(p => p.PrecoUnitarioVenda).HasColumnName("vl_pu_venda");
            Property(p => p.PrecoUnitarioBase).HasColumnName("vl_pu_base").IsOptional();
            

            //tabela
            ToTable("tb_valor_titulo_publico_bcb");
        }
    }
}

