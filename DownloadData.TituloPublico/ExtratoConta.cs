using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadData.TituloPublico
{
    public class ExtratoConta
    {
        public string Conta { get; set; }
        public DateTime DataTransacao { get; set; }
        public string DocumentoReferencia { get; set; }
        public string Historico { get; set; }
        public double ValorTransacao { get; set; }
        public string TipoTransacao { get; set; }
    }
}
