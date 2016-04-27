using GenericParsing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DownloadData.TituloPublico
{
    public class LeitorExtratoConta
    {

        public IList<ExtratoConta> LerArquivo(string caminhoParaLer)
        {
            IList<ExtratoConta> registros = null;

            var tabelaRegistros = LerArquivoCSV(caminhoParaLer);
            registros = ConverterRegistros(tabelaRegistros);

            return registros;
        }

        private IList<ExtratoConta> ConverterRegistros(DataTable tabelaRegistros)
        {
            IList<ExtratoConta> registrosConvertidos = null;
            if (tabelaRegistros == null) { return null; }

            registrosConvertidos = tabelaRegistros.AsEnumerable()                
                .Select(
                    dr => new ExtratoConta
                    {
                        Conta = dr.Field<string>("nr_conta"),
                        DataTransacao = dr.Field<DateTime>("dt_transacao"),
                        DocumentoReferencia = dr.Field<string>("nr_documento"),
                        Historico = dr.Field<string>("tx_descricao"),
                        ValorTransacao = dr.Field<double>("vl_montante"),
                        TipoTransacao = dr.Field<string>("tp_movimento")
                    }).ToList();

            return registrosConvertidos;
        }

        public DataTable LerArquivoParaTabela(string caminhoParaLer)
        {
            var tabelaRegistros = LerArquivoCSV(caminhoParaLer);
            return tabelaRegistros;
        }

        private DataTable LerArquivoCSV(string caminhoParaLer)
        {
            const string formato_data = @"yyyyMMdd";

            StringBuilder numeroContaConta = null;
            StringBuilder dataMovimentacao = null;
            StringBuilder numeroDocumento = null;
            StringBuilder historico = null;
            StringBuilder valorMovimentacao = null;
            StringBuilder tipoMovimentacao = null;

            DataTable tabelaRegistros = null;
            DataRow umRegistro = null;
            CultureInfo provider = null;
            DateTime dataSerieData;
            double valorSerieDouble = 0;

            numeroContaConta = new StringBuilder();
            dataMovimentacao = new StringBuilder();
            numeroDocumento = new StringBuilder();
            historico = new StringBuilder();
            valorMovimentacao = new StringBuilder();
            tipoMovimentacao = new StringBuilder();

            tabelaRegistros = CriarTabelaInputDoland();
            provider = CultureInfo.InvariantCulture;

            using (var parser = new GenericParser())
            {
                parser.SetDataSource(caminhoParaLer);

                parser.ColumnDelimiter = ';';
                parser.FirstRowHasHeader = true;
                parser.MaxBufferSize = 4096;
                parser.MaxRows = 500000;

                valorSerieDouble = 0;

                while (parser.Read())
                {
                    numeroContaConta.Clear();
                    dataMovimentacao.Clear();
                    numeroDocumento.Clear();
                    historico.Clear();
                    valorMovimentacao.Clear();
                    tipoMovimentacao.Clear();

                    numeroContaConta.Append(parser[0]);
                    dataMovimentacao.Append(parser[1]);
                    numeroDocumento.Append(parser[2]);
                    historico.Append(parser[3]);
                    valorMovimentacao.Append(parser[4]);
                    tipoMovimentacao.Append(parser[5]);

                    umRegistro = tabelaRegistros.NewRow();
                    umRegistro[0] = numeroContaConta.ToString();
                    DateTime.TryParseExact(dataMovimentacao.ToString(), formato_data, provider, DateTimeStyles.None, out dataSerieData);
                    umRegistro[1] = dataSerieData;
                    umRegistro[2] = numeroDocumento.ToString();
                    umRegistro[3] = historico.ToString();
                    double.TryParse(valorMovimentacao.ToString(), out valorSerieDouble);
                    umRegistro[4] = valorSerieDouble;
                    umRegistro[5] = tipoMovimentacao.ToString();

                    tabelaRegistros.Rows.Add(umRegistro);
                }
            }
            //Utils.DataTableUtils.PrintDataTable(tabelaRegistros);
            return tabelaRegistros;
        }

        private DataTable CriarTabelaInputDoland()
        {
            System.Data.DataTable tabelaDeRetorno = null;

            tabelaDeRetorno = new System.Data.DataTable("tb_extrato_conta");
            tabelaDeRetorno.Columns.Add(new DataColumn("nr_conta", typeof(string)));
            tabelaDeRetorno.Columns.Add(new DataColumn("dt_transacao", typeof(DateTime)));
            tabelaDeRetorno.Columns.Add(new DataColumn("nr_documento", typeof(string)));
            tabelaDeRetorno.Columns.Add(new DataColumn("tx_descricao", typeof(string)));
            tabelaDeRetorno.Columns.Add(new DataColumn("vl_montante", typeof(double)));
            tabelaDeRetorno.Columns.Add(new DataColumn("tp_movimento", typeof(string)));

            return tabelaDeRetorno;
        }
    }
}
