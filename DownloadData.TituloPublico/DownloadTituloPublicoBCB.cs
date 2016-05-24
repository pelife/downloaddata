using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Text;
using System.Linq;

namespace DownloadData.TituloPublico
{
    public class DownloadTituloPublicoBCB
    {
        private const string CONST_URL_DOWNLOAD_HISTORICO_PREFIXO = "http://www3.tesouro.gov.br/tesouro_direto/download/historico/";
        private const string CONST_URL_DOWNLOAD_ATUAL_PREFIXO = "http://www.tesouro.fazenda.gov.br/documents/10180/137713/";
        const string formato_data = @"dd/MM/yyyy";


        public void DownloadYear(int year, string pathToSave, TipoTituloPublicoBCB tipoTituloPublico)
        {
            string fileName = string.Empty;
            string urlFile = string.Empty;
            string pathToSaveTratado = string.Empty;

            try
            {
                using (var dbContext = new TituloPublicoContext("financeDB"))
                {
                    if (string.IsNullOrEmpty(pathToSave))
                        throw new InvalidOperationException("In order to download files, it is necessary to setup de path");

                    pathToSaveTratado = SubstituirCamposPath(pathToSave, tipoTituloPublico, year);
                    FormatarNomeArquivoLFT(year, tipoTituloPublico, ref fileName, ref urlFile);

                    var CaminhoCompleto = string.Format(@"{0}{1}", pathToSaveTratado, fileName);
                    if (Downloader.DownloadFile(fileName, pathToSaveTratado, urlFile))
                    {
                        var dadosPlanilha = LerDadosSyncfusion(CaminhoCompleto);
                        var cotacoesNovas = TransformarCotacoes(dadosPlanilha);
                        var cotacoesAntigas = dbContext.CotacaoTituloPublicoTD;

                        var defaultDates = (from cotacaoNova in cotacoesNovas
                                            select new
                                            {
                                                CodigoTitulo = cotacaoNova.CodigoTitulo,
                                                MinDataCotacao = DateTime.Today,
                                                MaxDataCotacao = DateTime.Today
                                            }).ToList()
                                            .Distinct();

                        var maxCotacaoTitulo = (from x in cotacoesAntigas
                                                group x by x.CodigoTitulo into y
                                                orderby y.Key
                                                select new
                                                {
                                                    CodigoTitulo = y.Key,
                                                    MinDataCotacao = y.Min(z => z.DataCotacao),
                                                    MaxDataCotacao = y.Max(z => z.DataCotacao)
                                                }).ToList()
                                               .Distinct();

                        var dicionario = maxCotacaoTitulo.ToDictionary(p => p.CodigoTitulo);

                        foreach (var umDefault in defaultDates)
                            if (!dicionario.ContainsKey(umDefault.CodigoTitulo))
                                dicionario[umDefault.CodigoTitulo] = umDefault;

                        var listaTotal = dicionario.Values.ToList();

                        var queryFiltraPendentes = (from cotacaoNova in cotacoesNovas
                                                    from maxCotacao in listaTotal
                                                      .Where(maxCot => maxCot.CodigoTitulo == cotacaoNova.CodigoTitulo && cotacaoNova.DataCotacao < maxCot.MinDataCotacao)
                                                    select cotacaoNova);

                        var registrosInserir = queryFiltraPendentes.ToList();

                        cotacoesAntigas.AddRange(registrosInserir);
                    }
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex) { throw ex; }
        }

        private IList<CotacaoTituloPublicoTesouroDireto> TransformarCotacoes(DataSet tabelasCotacoes)
        {
            List<CotacaoTituloPublicoTesouroDireto> cotacoesTransformadas = null;

            if (tabelasCotacoes != null && tabelasCotacoes.Tables.Count > 0)
            {
                cotacoesTransformadas = new List<CotacaoTituloPublicoTesouroDireto>();
                foreach (DataTable umaTabelaCotacoes in tabelasCotacoes.Tables)
                    cotacoesTransformadas.AddRange(LerCotacoes(umaTabelaCotacoes));

            }


            return cotacoesTransformadas;
        }

        private IEnumerable<CotacaoTituloPublicoTesouroDireto> LerCotacoes(DataTable umaTabelaCotacoes)
        {
            CultureInfo provider = null;

            provider = CultureInfo.InvariantCulture;
            var cotacoes = umaTabelaCotacoes.AsEnumerable()
                .Skip(1)
                .Select(
                    dr => new CotacaoTituloPublicoTesouroDireto
                    {
                        DataCotacao = umaTabelaCotacoes.Columns["dt_valor"].DataType == typeof(string) ?
                            DateTime.ParseExact(dr["dt_valor"].ToString(), formato_data, provider, DateTimeStyles.None) :
                            dr.Field<DateTime>("dt_valor"),

                        CodigoTitulo = dr.Field<string>("sg_titulo"),
                        TipoTitulo = dr.Field<string>("tp_titulo"),

                        Vencimento = umaTabelaCotacoes.Columns["dt_vencimento"].DataType == typeof(string) ?
                                DateTime.ParseExact(dr["dt_vencimento"].ToString(), formato_data, provider, DateTimeStyles.None) :
                                dr.Field<DateTime>("dt_vencimento"),
                        TaxaCompra = umaTabelaCotacoes.Columns["vl_taxa_compra"].DataType == typeof(string) ?
                                double.Parse(dr["vl_taxa_compra"].ToString()) :
                                dr.Field<double>("vl_taxa_compra"),
                        TaxaVenda = umaTabelaCotacoes.Columns["vl_taxa_venda"].DataType == typeof(string) ?
                                double.Parse(dr["vl_taxa_venda"].ToString()) :
                                dr.Field<double>("vl_taxa_venda"),
                        PrecoUnitarioCompra = umaTabelaCotacoes.Columns["vl_pu_compra"].DataType == typeof(string) ?
                                double.Parse(dr["vl_pu_compra"].ToString()) :
                                dr.Field<double>("vl_pu_compra"),
                        PrecoUnitarioVenda = umaTabelaCotacoes.Columns["vl_pu_venda"].DataType == typeof(string) ?
                                double.Parse(dr["vl_pu_venda"].ToString()) :
                                dr.Field<double>("vl_pu_venda"),
                        PrecoUnitarioBase = umaTabelaCotacoes.Columns.Contains("vl_pu_base") && !Convert.IsDBNull(dr["vl_pu_base"]) ? dr.Field<double>("vl_pu_base") : null as double?

                    }).ToList();

            return cotacoes;
        }

        private string SubstituirCamposPath(string pathToSave, TipoTituloPublicoBCB tipoTituloPublico, int year)
        {
            StringBuilder construtorCaminho = new StringBuilder(pathToSave);
            construtorCaminho.Replace("{$tipoTituloPublico}", tipoTituloPublico.ToString());
            construtorCaminho.Replace("{$ano}", year.ToString());

            return construtorCaminho.ToString();
        }

        public DataSet LerDadosSyncfusion(string CaminhoArquivo)
        {

            CultureInfo provider = null;

            DataSet retorno = null;
            DataTable umConjuntoDados = null;
            DateTime vencimento;
            ExcelEngine excelEngine = null;
            IApplication application = null;
            IWorkbook workbook = null;

            string[] partesNomeTitulo = null;
            string nomeTipoTitulo = null;
            string vencimentoText = null;

            provider = CultureInfo.InvariantCulture;
            excelEngine = new ExcelEngine();
            application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel97to2003;
            workbook = application.Workbooks.Open(CaminhoArquivo);
            retorno = new DataSet();

            for (int i = 0; i < workbook.Worksheets.Count; i++)
            {

                var planilha = workbook.Worksheets[i];
                var nomePlanilha = planilha.Name;

                partesNomeTitulo = nomePlanilha.Split(' ');
                if (partesNomeTitulo != null && partesNomeTitulo.Length > 0)
                    nomeTipoTitulo = partesNomeTitulo[0];

                vencimentoText = planilha.Range["B1"].Cells[0].Value;
                DateTime.TryParseExact(vencimentoText, formato_data, provider, DateTimeStyles.None, out vencimento);

                planilha.UsedRangeIncludesFormatting = false;
                umConjuntoDados = planilha.ExportDataTable(planilha.UsedRange.Row + 1,
                                                            planilha.UsedRange.Column,
                                                            planilha.UsedRange.LastRow - 1,
                                                            planilha.UsedRange.LastColumn,
                                                            ExcelExportDataTableOptions.ColumnNames | ExcelExportDataTableOptions.DetectColumnTypes);

                umConjuntoDados.TableName = nomePlanilha.Replace(" Principal", "P").Replace(" ", "_");
                umConjuntoDados = DeletarRegistrosSemDados(umConjuntoDados);
                AjustarNomeColuna(umConjuntoDados);
                AdicionarSiglaTitulo(umConjuntoDados, umConjuntoDados.TableName);
                AdicionarDataVencimento(umConjuntoDados, vencimento);
                AdicionarTipoTitulo(umConjuntoDados, nomeTipoTitulo);
                retorno.Tables.Add(umConjuntoDados);
            }

            workbook.Close();
            excelEngine.Dispose();
            Utils.DataTableUtils.PrintDataSet(retorno);

            return retorno;
        }

        private void AdicionarDataVencimento(DataTable umConjuntoDados, DateTime vencimento)
        {
            if (umConjuntoDados == null)
                return;

            if (!umConjuntoDados.Columns.Contains("dt_vencimento"))
                umConjuntoDados.Columns.Add("dt_vencimento", typeof(DateTime));

            foreach (DataRow umItem in umConjuntoDados.Rows)
                umItem["dt_vencimento"] = vencimento;

            umConjuntoDados.AcceptChanges();
        }

        private void AdicionarSiglaTitulo(DataTable umConjuntoDados, string siglaTitulo)
        {
            if (umConjuntoDados == null)
                return;

            if (!umConjuntoDados.Columns.Contains("sg_titulo"))
                umConjuntoDados.Columns.Add("sg_titulo", typeof(string));

            foreach (DataRow umItem in umConjuntoDados.Rows)
                umItem["sg_titulo"] = siglaTitulo;

            umConjuntoDados.AcceptChanges();
        }

        private void AdicionarTipoTitulo(DataTable umConjuntoDados, string tipoTitulo)
        {
            if (umConjuntoDados == null)
                return;

            if (!umConjuntoDados.Columns.Contains("tp_titulo"))
                umConjuntoDados.Columns.Add("tp_titulo", typeof(string));

            foreach (DataRow umItem in umConjuntoDados.Rows)
                umItem["tp_titulo"] = tipoTitulo;

            umConjuntoDados.AcceptChanges();
        }

        private void AjustarNomeColuna(DataTable umConjuntoDados)
        {
            if (umConjuntoDados == null)
                return;

            if (umConjuntoDados.Columns.Count > 0)
                umConjuntoDados.Columns[0].ColumnName = "dt_valor";

            if (umConjuntoDados.Columns.Count > 1)
                umConjuntoDados.Columns[1].ColumnName = "vl_taxa_compra";

            if (umConjuntoDados.Columns.Count > 2)
                umConjuntoDados.Columns[2].ColumnName = "vl_taxa_venda";

            if (umConjuntoDados.Columns.Count > 3)
                umConjuntoDados.Columns[3].ColumnName = "vl_pu_compra";

            if (umConjuntoDados.Columns.Count > 4)
                umConjuntoDados.Columns[4].ColumnName = "vl_pu_venda";

            if (umConjuntoDados.Columns.Count > 5)
                umConjuntoDados.Columns[5].ColumnName = "vl_pu_base";

            umConjuntoDados.AcceptChanges();
        }

        private DataTable DeletarRegistrosSemDados(DataTable tabelaTratar)
        {
            bool deletarLinha = false;
            int indiceLinha = 0;
            double resultado = 0;

            if (tabelaTratar == null)
                return tabelaTratar;

            var linhasTabela = tabelaTratar.Rows.Count;
            while (indiceLinha < linhasTabela)
            {
                deletarLinha = true;
                for (int i = 1; i < tabelaTratar.Columns.Count; i++) //pula a primeira coluna porque é data
                {
                    var tabelaLinha = tabelaTratar.Rows[indiceLinha];
                    if (!Convert.IsDBNull(tabelaLinha[i]) &&
                            !string.IsNullOrEmpty(tabelaLinha[i].ToString().Trim()) &&
                            double.TryParse(tabelaLinha[i].ToString(), out resultado) &&
                            !double.IsNaN(resultado) &&
                            !double.IsInfinity(resultado))
                    {

                        deletarLinha = false;
                        break; //se a tabela tiver pelo menos 1 campo preenchido, va para a proxima iteração
                    }
                }
                if (deletarLinha)
                {
                    linhasTabela = linhasTabela - 1;
                    tabelaTratar.Rows.RemoveAt(indiceLinha);
                }
                else
                    indiceLinha = indiceLinha + 1;
            }

            tabelaTratar.AcceptChanges();
            return tabelaTratar;
        }

        private DataTable LerDados(string excelFile, string sheetName)
        {
            string ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0; data source=" + excelFile + "; Extended Properties=Excel 12.0;";

            OleDbConnection objConn = null;
            DataTable dt = null;
            //Create connection object by using the preceding connection string.
            objConn = new OleDbConnection(ConnectionString);
            objConn.Open();
            //Get the data table containg the schema guid.
            dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            string sql = string.Format("select * from [{0}$]", sheetName);
            var adapter = new OleDbDataAdapter(sql, ConnectionString);
            DataTable data = new DataTable();
            adapter.Fill(data);
            return data;
        }

        private void FormatarNomeArquivoLFT(int Ano, TipoTituloPublicoBCB tipoTituloPublico, ref string NomeArquivo, ref string URLDownload)
        {
            var prefixoTipoAtivo = ObterPrefixoTipoTitulo(tipoTituloPublico);
            var sufixoURL = CriarSufixoURL(Ano, prefixoTipoAtivo);

            if ((Ano <= 2011))
                URLDownload = string.Concat(CONST_URL_DOWNLOAD_HISTORICO_PREFIXO, sufixoURL);
            else
                URLDownload = string.Concat(CONST_URL_DOWNLOAD_ATUAL_PREFIXO, sufixoURL);

            NomeArquivo = string.Format("{2}{0}_{1:yyyyMMdd}.xls", Ano, DateTime.Today, prefixoTipoAtivo);
        }

        private string ObterPrefixoTipoTitulo(TipoTituloPublicoBCB tipoTituloPublico)
        {
            string prefixoTipoTitulo = string.Empty;

            switch (tipoTituloPublico)
            {
                case TipoTituloPublicoBCB.LFT:
                    prefixoTipoTitulo = "LFT";
                    break;
                case TipoTituloPublicoBCB.LTN:
                    prefixoTipoTitulo = "LTN";
                    break;
                case TipoTituloPublicoBCB.NTNC:
                    prefixoTipoTitulo = "NTN-C";
                    break;
                case TipoTituloPublicoBCB.NTNB:
                    prefixoTipoTitulo = "NTN-B";
                    break;
                case TipoTituloPublicoBCB.NTNBPrincipal:
                    prefixoTipoTitulo = "NTN-B_Principal";
                    break;
                case TipoTituloPublicoBCB.NTNF:
                    prefixoTipoTitulo = "NTN-F";
                    break;
                default:
                    prefixoTipoTitulo = "";
                    break;
            }

            return prefixoTipoTitulo;

        }

        private string CriarSufixoURL(int Ano, string PrefixoAtivo)
        {
            string sufixoURL = string.Empty;

            if (Ano <= 2011)
            {
                var prefixoHistorico = PrefixoAtivo.Replace("-", "").Replace("_", "");
                sufixoURL = string.Format("{0}/historico{1}_{0}.xls", Ano, prefixoHistorico);
            }
            else
                sufixoURL = string.Format("{1}_{0}.xls", Ano, PrefixoAtivo);

            return sufixoURL;
        }
    }
}
