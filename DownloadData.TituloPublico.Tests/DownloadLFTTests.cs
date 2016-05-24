using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DownloadData.TituloPublico.Tests
{
    [TestClass]
    public class DownloadLFTTests
    {
        [TestMethod]
        public void DownloadLFTSFiles()
        {
            var lftDownloader = new DownloadTituloPublicoBCB();
            
            var localtoSaveFile = @"c:\temp\download_bcb\{$ano}\{$tipoTituloPublico}\";
            
            lftDownloader.DownloadYear(2016, localtoSaveFile, TipoTituloPublicoBCB.LFT);
            lftDownloader.DownloadYear(2016, localtoSaveFile, TipoTituloPublicoBCB.LTN);
            lftDownloader.DownloadYear(2016, localtoSaveFile, TipoTituloPublicoBCB.NTNC);
            lftDownloader.DownloadYear(2016, localtoSaveFile, TipoTituloPublicoBCB.NTNB);
            lftDownloader.DownloadYear(2016, localtoSaveFile, TipoTituloPublicoBCB.NTNBPrincipal);
            lftDownloader.DownloadYear(2016, localtoSaveFile, TipoTituloPublicoBCB.NTNF);

            lftDownloader.DownloadYear(2015, localtoSaveFile, TipoTituloPublicoBCB.LFT);
            lftDownloader.DownloadYear(2015, localtoSaveFile, TipoTituloPublicoBCB.LTN);
            lftDownloader.DownloadYear(2015, localtoSaveFile, TipoTituloPublicoBCB.NTNC);
            lftDownloader.DownloadYear(2015, localtoSaveFile, TipoTituloPublicoBCB.NTNB);
            lftDownloader.DownloadYear(2015, localtoSaveFile, TipoTituloPublicoBCB.NTNBPrincipal);
            lftDownloader.DownloadYear(2015, localtoSaveFile, TipoTituloPublicoBCB.NTNF);

            lftDownloader.DownloadYear(2014, localtoSaveFile, TipoTituloPublicoBCB.LFT);
            lftDownloader.DownloadYear(2014, localtoSaveFile, TipoTituloPublicoBCB.LTN);
            lftDownloader.DownloadYear(2014, localtoSaveFile, TipoTituloPublicoBCB.NTNC);
            lftDownloader.DownloadYear(2014, localtoSaveFile, TipoTituloPublicoBCB.NTNB);
            lftDownloader.DownloadYear(2014, localtoSaveFile, TipoTituloPublicoBCB.NTNBPrincipal);
            lftDownloader.DownloadYear(2014, localtoSaveFile, TipoTituloPublicoBCB.NTNF);

            lftDownloader.DownloadYear(2013, localtoSaveFile, TipoTituloPublicoBCB.LFT);
            lftDownloader.DownloadYear(2013, localtoSaveFile, TipoTituloPublicoBCB.LTN);
            lftDownloader.DownloadYear(2013, localtoSaveFile, TipoTituloPublicoBCB.NTNC);
            lftDownloader.DownloadYear(2013, localtoSaveFile, TipoTituloPublicoBCB.NTNB);
            lftDownloader.DownloadYear(2013, localtoSaveFile, TipoTituloPublicoBCB.NTNBPrincipal);
            lftDownloader.DownloadYear(2013, localtoSaveFile, TipoTituloPublicoBCB.NTNF);

            lftDownloader.DownloadYear(2012, localtoSaveFile, TipoTituloPublicoBCB.LFT);
            lftDownloader.DownloadYear(2012, localtoSaveFile, TipoTituloPublicoBCB.LTN);
            lftDownloader.DownloadYear(2012, localtoSaveFile, TipoTituloPublicoBCB.NTNC);
            lftDownloader.DownloadYear(2012, localtoSaveFile, TipoTituloPublicoBCB.NTNB);
            lftDownloader.DownloadYear(2012, localtoSaveFile, TipoTituloPublicoBCB.NTNBPrincipal);
            lftDownloader.DownloadYear(2012, localtoSaveFile, TipoTituloPublicoBCB.NTNF);

            lftDownloader.DownloadYear(2011, localtoSaveFile, TipoTituloPublicoBCB.LFT);
            lftDownloader.DownloadYear(2011, localtoSaveFile, TipoTituloPublicoBCB.LTN);
            lftDownloader.DownloadYear(2011, localtoSaveFile, TipoTituloPublicoBCB.NTNC);
            lftDownloader.DownloadYear(2011, localtoSaveFile, TipoTituloPublicoBCB.NTNB);
            lftDownloader.DownloadYear(2011, localtoSaveFile, TipoTituloPublicoBCB.NTNBPrincipal);
            lftDownloader.DownloadYear(2011, localtoSaveFile, TipoTituloPublicoBCB.NTNF);

            lftDownloader.DownloadYear(2010, localtoSaveFile, TipoTituloPublicoBCB.LFT);
            lftDownloader.DownloadYear(2010, localtoSaveFile, TipoTituloPublicoBCB.LTN);
            lftDownloader.DownloadYear(2010, localtoSaveFile, TipoTituloPublicoBCB.NTNC);
            lftDownloader.DownloadYear(2010, localtoSaveFile, TipoTituloPublicoBCB.NTNB);
            lftDownloader.DownloadYear(2010, localtoSaveFile, TipoTituloPublicoBCB.NTNBPrincipal);
            lftDownloader.DownloadYear(2010, localtoSaveFile, TipoTituloPublicoBCB.NTNF);
            
            lftDownloader.DownloadYear(2002, localtoSaveFile, TipoTituloPublicoBCB.LFT);
            lftDownloader.DownloadYear(2002, localtoSaveFile, TipoTituloPublicoBCB.LTN);
            lftDownloader.DownloadYear(2002, localtoSaveFile, TipoTituloPublicoBCB.NTNC);
            lftDownloader.DownloadYear(2002, localtoSaveFile, TipoTituloPublicoBCB.NTNB);
            lftDownloader.DownloadYear(2002, localtoSaveFile, TipoTituloPublicoBCB.NTNBPrincipal);
            lftDownloader.DownloadYear(2002, localtoSaveFile, TipoTituloPublicoBCB.NTNF);



        }
    }
}
