using System;

namespace DownloadData.TituloPublico
{
    public class CotacaoTituloPublicoBCB
    {
        public DateTime DataCotacao { get; set; }
        public string CodigoTitulo { get; set; }
        public string TipoTitulo { get; set; }
        public DateTime Vencimento { get; set; }
        public double TaxaCompra { get; set; }
        public double TaxaVenda { get; set; }
        public double PrecoUnitarioCompra { get; set; }
        public double PrecoUnitarioVenda { get; set; }
        public double? PrecoUnitarioBase { get; set; }
        
    }
}
