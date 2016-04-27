using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace DownloadData.TituloPublico.Tests
{
    [TestClass]
    public class LeitorExtratoContaTests
    {
        [TestMethod]
        public void TestarLeituraDeArquivo()
        {
            var leitor = new LeitorExtratoConta();
            var registrosTabela = leitor.LerArquivoParaTabela(@"c:\temp\7C57f09d51d3c2c084caf66393d193b.txt");
            var registrosLista = leitor.LerArquivo(@"c:\temp\7C57f09d51d3c2c084caf66393d193b.txt");
            PersistenciaConta.Salvar(registrosLista);
        }
    }
}
