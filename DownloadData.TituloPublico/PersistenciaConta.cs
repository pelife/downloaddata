using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadData.TituloPublico
{
    public class PersistenciaConta
    {
        public static void Salvar(IList<ExtratoConta> movimentacoesSalvar)
        {
            try
            {
                using (var dbContext = new TituloPublicoContext("financeDB"))
                {
                    dbContext.ExtratoConta.AddRange(movimentacoesSalvar);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
