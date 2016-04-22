using System;
using System.IO;
using System.Net;

namespace DownloadData.TituloPublico
{
    public class Downloader
    {
        public static bool DownloadFile(string fileName, string pathSaveTo, string uRLDownload)
        {
            var salvouComSucesso = false;
            var pathLocalFile = Path.Combine(pathSaveTo, fileName);

            if (File.Exists(pathLocalFile))
                File.Delete(pathLocalFile);

            var directoryPath = Path.GetDirectoryName(pathSaveTo);

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            using (var webClient = new WebClient())
            {
                try
                {
                    webClient.DownloadFile(uRLDownload, pathLocalFile);
                    salvouComSucesso = true;
                }
                catch (WebException ex)
                {
                    salvouComSucesso = false;
                    if (ex.Status == WebExceptionStatus.ProtocolError) { }
                }
                catch (Exception) { salvouComSucesso = false; }
            }

            return salvouComSucesso;
        }
    }
}
