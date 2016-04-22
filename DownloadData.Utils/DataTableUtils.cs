using System;
using System.Collections;
using System.Data;
using System.IO;

namespace DownloadData.Utils
{
    public class DataTableUtils
    {

        const string vbCr = @"\r";
        const string vbLf = @"\n";
        const string vbCrLf = @"\r\n";

        /// <summary>
        /// Imprime um data set no output
        /// </summary>
        /// <param name="vDts_DataSetToPrint"></param>
        /// <remarks></remarks>
        public static void PrintDataSet(DataSet vDts_DataSetToPrint)
        {
            foreach (DataTable lDtt_Table in vDts_DataSetToPrint.Tables)
            {
                PrintDataTable(lDtt_Table);
                System.Diagnostics.Debug.Write(Environment.NewLine);
            }
        }

        /// <summary>
        /// Gets a string representation of the <see cref="System.Data.DataTable" />.
        /// </summary>
        /// <remarks>The string representation should be displayed with a monospaced font.</remarks>
        /// <history>
        /// 	[almeidfa]	24/8/2010	Created
        /// </history>
        private static string GetStringRepresentation(DataTable dataTable)
        {
            if (dataTable == null)
            {
                throw new ArgumentNullException("'dataTable' cannot be null.");
            }

            StringWriter representationWriter = new StringWriter();

            // First, set the width of every column to the length of its largest element.
            int[] columnWidths = new int[dataTable.Columns.Count];
            for (int columnIndex = 0; columnIndex <= dataTable.Columns.Count - 1; columnIndex++)
            {
                int headerWidth = dataTable.Columns[columnIndex].ColumnName.Length;

                int longestElementWidth = 0;
                foreach (DataRow lDtr_Row in dataTable.Rows)
                {
                    if (!(lDtr_Row.RowState == DataRowState.Deleted))
                    {
                        if (lDtr_Row[columnIndex].ToString().Length > longestElementWidth)
                        {
                            longestElementWidth = lDtr_Row[columnIndex].ToString().Length;
                        }
                    }
                }
                columnWidths[columnIndex] = Math.Max(headerWidth, longestElementWidth);
            }

            // Next, write the table
            // Write a horizontal line.
            representationWriter.Write("+-");
            for (int columnIndex = 0; columnIndex <= dataTable.Columns.Count - 1; columnIndex++)
            {
                for (int i = 0; i <= columnWidths[columnIndex] - 1; i++)
                {
                    representationWriter.Write("-");
                }
                representationWriter.Write("-+-");
            }
            // Delete the extra "-"
            representationWriter.Write(Convert.ToChar(8));
            representationWriter.WriteLine(" ");
            // Print the headers
            representationWriter.Write("| ");
            for (int columnIndex = 0; columnIndex <= dataTable.Columns.Count - 1; columnIndex++)
            {
                string header = dataTable.Columns[columnIndex].ColumnName;
                representationWriter.Write(header);
                for (int blanks = columnWidths[columnIndex] - header.Length; blanks >= 1; blanks += -1)
                {
                    representationWriter.Write(" ");
                }
                representationWriter.Write(" | ");
            }
            representationWriter.WriteLine();
            // Print another horizontal line.
            representationWriter.Write("+-");
            for (int columnIndex = 0; columnIndex <= dataTable.Columns.Count - 1; columnIndex++)
            {
                for (int i = 0; i <= columnWidths[columnIndex] - 1; i++)
                {
                    representationWriter.Write("-");
                }
                representationWriter.Write("-+-");
            }
            // Delete the extra "-"
            representationWriter.Write(Convert.ToChar(8));
            representationWriter.WriteLine(" ");

            // Print the contents of the table.
            for (int row = 0; row <= dataTable.Rows.Count - 1; row++)
            {
                representationWriter.Write("| ");
                for (int column = 0; column <= dataTable.Columns.Count - 1; column++)
                {
                    representationWriter.Write(dataTable.Rows[row][column]);
                    for (int blanks = columnWidths[column] - dataTable.Rows[row][column].ToString().Length; blanks >= 1; blanks += -1)
                    {
                        representationWriter.Write(" ");
                    }
                    representationWriter.Write(" | ");
                }
                representationWriter.WriteLine();
            }

            // Print a final horizontal line.
            representationWriter.Write("+-");
            for (int column = 0; column <= dataTable.Columns.Count - 1; column++)
            {
                for (int i = 0; i <= columnWidths[column] - 1; i++)
                {
                    representationWriter.Write("-");
                }
                representationWriter.Write("-+-");
            }
            // Delete the extra "-"
            representationWriter.Write(Convert.ToChar(8));
            representationWriter.WriteLine(" ");

            return representationWriter.ToString();
        }

        /// <summary>
        /// Imprime um data table no output
        /// </summary>
        /// <param name="vDtt_DataTableToPrint"></param>
        /// <remarks></remarks>
        /// <history>
        /// 	[almeidfa]	24/8/2010	Created
        /// </history>
        public static void PrintDataTable(DataTable vDtt_DataTableToPrint)
        {
            if (vDtt_DataTableToPrint == null)
            {
                System.Diagnostics.Debug.Print("Null");
                return;
            }
            if (!string.IsNullOrEmpty(vDtt_DataTableToPrint.TableName))
            {
                System.Diagnostics.Debug.WriteLine(vDtt_DataTableToPrint.TableName);
            }
            System.Diagnostics.Debug.WriteLine(GetStringRepresentation(vDtt_DataTableToPrint));
        }

        /// <summary>
        /// Imprime um data row no output
        /// </summary>
        /// <param name="vDtr_DataRowToPrint"></param>
        /// <remarks></remarks>
        /// <history>
        /// 	[almeidfa]	24/8/2010	Created
        /// </history>
        public static void PrintDataRow(DataRow vDtr_DataRowToPrint)
        {
            PrintDataTable(DataRowArrayToDataTable(new DataRow[] { vDtr_DataRowToPrint }));
        }

        /// <summary>
        /// Função utilizada para fazer output para o console da estrutura da view
        /// </summary>
        /// <param name="vDtv_DataView">View para mostrar</param>

        public static void PrintDataView(DataView vDtv_DataView)
        {
            DataTable lDtt_DataTable = null;
            lDtt_DataTable = vDtv_DataView.Table.Clone();
            AddView(ref vDtv_DataView, ref lDtt_DataTable);

            PrintDataTable(lDtt_DataTable);
        }

        /// <summary>
        /// Função utilizada para fazer output para o console da estrutura da tabela de acordo com as colunas selecionadas
        /// </summary>
        /// <param name="vDtt_DataTableToPrint">Tabela para output</param>
        /// <param name="vStr_ColumnNames">Nome das colunas que se deseja printar</param>

        public static void PrintDataColumns(DataTable vDtt_DataTableToPrint, string[] vStr_ColumnNames)
        {
            DataTable lDtt_TableToPrint = null;
            lDtt_TableToPrint = CreateDataTableCustom(vDtt_DataTableToPrint, vStr_ColumnNames);

            PrintDataTable(lDtt_TableToPrint);
        }

        /// <summary>
        /// Função para exportar o conteudo de uma tabela p/ um arquivo CSV
        /// </summary>
        /// <param name="vDtt_DataTableToExport">Tabela para exportar</param>
        /// <param name="vStr_ColumnNames">Nome Colunas que se deseja exportar</param>
        /// <param name="vStr_FilePath">Caminho para salvar o arquivo</param>
        /// <remarks>
        /// Se o arquivo tiver aberto, o metodo gera uma exceção
        /// </remarks>
        public static void ExportTableToCSV(DataTable vDtt_DataTableToExport, string[] vStr_ColumnNames, string vStr_FilePath)
        {
            DataTable lDtt_TableToPrint = null;
            lDtt_TableToPrint = CreateDataTableCustom(vDtt_DataTableToExport, vStr_ColumnNames);
            ExportTableToCSV(lDtt_TableToPrint, vStr_FilePath);
        }

        /// <summary>
        /// Função para exportar o conteudo de uma tabela p/ um arquivo CSV
        /// </summary>
        /// <param name="vDtt_DataTableToExport">Tabela para exportar</param>
        /// <param name="vStr_FilePath">Caminho para salvar o arquivo</param>
        /// <remarks>
        /// Se o arquivo tiver aberto, o metodo gera uma exceção
        /// </remarks>
        public static void ExportTableToCSV(DataTable vDtt_DataTableToExport, string vStr_FilePath)
        {
            string _ConteudoExportacao = string.Empty;
            _ConteudoExportacao = DataTableUtils.ToCSV(vDtt_DataTableToExport);
            DataTableUtils.WriteToFile(_ConteudoExportacao, vStr_FilePath);
        }

        /// <summary>
        /// Função para exportar o conteudo de uma view p/ um arquivo CSV
        /// </summary>
        /// <param name="vDtt_DataViewToExport">View para exportar</param>
        /// <param name="vStr_FilePath">Caminho para salvar o arquivo</param>
        /// <remarks>
        /// Se o arquivo tiver aberto, o metodo gera uma exceção
        /// </remarks>
        public static void ExportViewToCSV(DataView vDtt_DataViewToExport, string vStr_FilePath)
        {
            DataTableUtils.ExportTableToCSV(vDtt_DataViewToExport.ToTable(), vStr_FilePath);
        }

        /// <summary>
        /// Função para exportar o conteudo de uma view p/ um arquivo CSV
        /// </summary>
        /// <param name="vDtt_DataViewToExport">View para exportar</param>
        /// <param name="vStr_FilePath">Caminho para salvar o arquivo</param>
        /// <remarks>
        /// Se o arquivo tiver aberto, o metodo gera uma exceção
        /// </remarks>

        public static void ExportViewToCSV(DataView vDtt_DataViewToExport, string[] vStr_ColumnNames, string vStr_FilePath)
        {
            DataTable lDtt_TableToPrint = null;
            lDtt_TableToPrint = CreateDataTableCustom(vDtt_DataViewToExport.ToTable(), vStr_ColumnNames);

            ExportTableToCSV(lDtt_TableToPrint, vStr_FilePath);
        }

        /// <summary>
        /// Cria um datatable ja com uma linha preparada para receber parametros
        /// </summary>
        /// <param name="vArr_Coluna">Array com o nome dos parametros que irao gerar as colunas</param>
        /// <returns>A primeira linha ja parte da table criada</returns>
        /// <remarks>
        /// Essa funcionalidade esta associada a correta utilizacao dos prefixos de padronizacao
        /// dos nomes de colunas, ou seja, uma coluna DT_... ira gerar uma coluna datetime, 
        /// uma coluna ID_... ira gerar uma coluna Integer, ...
        /// </remarks>
        protected static System.Data.DataRow CriarTabelaLinha(string[] vArr_Coluna)
        {
            System.Data.DataTable lDtt_Tabela = null;

            // cria a tabela
            lDtt_Tabela = CriarTabela(vArr_Coluna);

            // cria nova linha
            lDtt_Tabela.Rows.Add(lDtt_Tabela.NewRow());

            // retorna primeira linha
            return lDtt_Tabela.Rows[0];

        }

        /// <summary>
        /// Cria um datatable ja com uma linha preparada para receber parametros
        /// </summary>
        /// <param name="vArr_Coluna">Array com o nome dos parametros que irao gerar as colunas</param>
        /// <returns>A tabela criada</returns>
        /// <remarks>
        /// Essa funcionalidade esta associada a correta utilizacao dos prefixos de padronizacao
        /// dos nomes de colunas, ou seja, uma coluna DT_... ira gerar uma coluna datetime, 
        /// uma coluna ID_... ira gerar uma coluna Integer, ...
        /// </remarks>
        public static System.Data.DataTable CriarTabela(string[] vArr_Coluna)
        {
            System.Data.DataTable lDtt_Tabela = null;
            string lStr_Coluna = null;
            System.Type lTyp_Coluna = null;

            // cria table
            lDtt_Tabela = new System.Data.DataTable();

            // com a estrutura de colunas
            var _with1 = lDtt_Tabela.Columns;

            // percorre as colunas

            foreach (string lStr_Coluna_loopVariable in vArr_Coluna)
            {
                lStr_Coluna = lStr_Coluna_loopVariable;
                // ajusta o nome do campo
                lStr_Coluna = lStr_Coluna.Trim().ToUpper();

                // determina o tipo pelo prefixo
                switch (lStr_Coluna.Trim().ToUpper().Substring(0, 2))
                {

                    case "ID":
                        // long
                        lTyp_Coluna = typeof(System.Int64);

                        break;
                    case "NR":
                    case "DI":
                    case "ME":
                    case "AN":
                    case "DV":
                        //inteiros
                        lTyp_Coluna = typeof(System.Int32);

                        break;
                    case "CD":
                        //shorts
                        lTyp_Coluna = typeof(System.Int32);

                        break;
                    case "VL":
                    case "QT":
                    case "PE":
                    case "FT":
                    case "MD":
                        // valor
                        lTyp_Coluna = typeof(System.Double);

                        break;
                    case "NM":
                    case "DS":
                    case "TX":
                    case "TP":
                    case "SG":
                        // string
                        lTyp_Coluna = typeof(System.String);

                        break;
                    case "DT":
                    case "DH":
                        // data
                        lTyp_Coluna = typeof(System.DateTime);

                        break;
                    case "ES":
                        // string
                        lTyp_Coluna = typeof(System.Int16);

                        break;
                    default:
                        // qualquer outro
                        // string
                        lTyp_Coluna = typeof(System.String);

                        break;
                }
                // determina o tipo pelo prefixo

                // cria coluna na table
                _with1.Add(lStr_Coluna, lTyp_Coluna);

            }
            // percorre as colunas


            // nao tem coluna?
            if ((lDtt_Tabela.Columns.Count == 0))
            {
                // dispara excecao
                throw new ApplicationException("There are no columns created!");
            }
            // nao tem coluna?

            // comita table
            lDtt_Tabela.AcceptChanges();

            // retorna a tabela
            return lDtt_Tabela;

        }

        /// <summary>
        /// Transforma uma tabela em conteudo CSV
        /// </summary>
        /// <param name="TabelaParaTransformar">Tabela para transformar em CSV</param>
        /// <returns>
        /// Conteúdo da tabela formatado em CSV
        /// </returns>
        private static string ToCSV(DataTable TabelaParaTransformar)
        {

            System.Text.StringBuilder _ResultadoConversao = new System.Text.StringBuilder();
            int _IndiceColunas = 0;
            DataRow _LinhaTabelaConversao = null;

            for (_IndiceColunas = 0; _IndiceColunas <= TabelaParaTransformar.Columns.Count - 1; _IndiceColunas++)
            {
                _ResultadoConversao.Append(TabelaParaTransformar.Columns[_IndiceColunas].ColumnName);
                _ResultadoConversao.Append(_IndiceColunas == TabelaParaTransformar.Columns.Count - 1 ? vbLf : ",");
            }

            foreach (DataRow _LinhaTabelaConversao_loopVariable in TabelaParaTransformar.Rows)
            {
                _LinhaTabelaConversao = _LinhaTabelaConversao_loopVariable;
                for (_IndiceColunas = 0; _IndiceColunas <= TabelaParaTransformar.Columns.Count - 1; _IndiceColunas++)
                {
                    if (!Convert.IsDBNull(_LinhaTabelaConversao[_IndiceColunas]))
                    {
                        _ResultadoConversao.Append(_LinhaTabelaConversao[_IndiceColunas].ToString().Replace(",", ""));
                    }
                    else
                    {
                        _ResultadoConversao.Append("NULL");
                    }
                    _ResultadoConversao.Append(_IndiceColunas == TabelaParaTransformar.Columns.Count - 1 ? vbLf : ",");
                }
            }

            return _ResultadoConversao.ToString();

        }

        /// <summary>
        /// Escreve o conteudo texto em um arquivo
        /// </summary>
        /// <param name="vStr_Conteudo">Conteúdo a ser escrito</param>
        /// <param name="vStr_CaminhoArquivo">Caminho do arquivo</param>
        /// <remarks>
        /// Se o arquivo tiver aberto, o metodo gera uma exceção
        /// </remarks>
        private static void WriteToFile(string vStr_Conteudo, string vStr_CaminhoArquivo)
        {
            System.IO.File.WriteAllText(vStr_CaminhoArquivo, vStr_Conteudo);
        }

        private static DataTable CreateDataTableCustom(DataTable DataTableOriginal, string[] CustomColumns)
        {
            DataTable functionReturnValue = null;
            DataTable lDtt_TableToPrint = null;
            DataRow lDtt_NewRowToPrint = null;
            DataRow lDtt_RowToPrint = null;

            if (DataTableOriginal == null)
            {
                return functionReturnValue;
            }

            if (CustomColumns == null || CustomColumns.Length == 0)
            {
                return functionReturnValue;
            }

            lDtt_TableToPrint = CriarTabela(CustomColumns);

            foreach (DataRow lDtt_RowToPrint_loopVariable in DataTableOriginal.Rows)
            {
                lDtt_RowToPrint = lDtt_RowToPrint_loopVariable;
                lDtt_NewRowToPrint = lDtt_TableToPrint.NewRow();

                CopyRowValues(lDtt_RowToPrint, ref lDtt_NewRowToPrint);
                lDtt_TableToPrint.Rows.Add(lDtt_NewRowToPrint);
            }

            return lDtt_TableToPrint;
        }

        private static DataTable DataRowArrayToDataTable(DataRow[] vArr_Rows)
        {
            DataTable lDtt_Data = null;

            if ((vArr_Rows == null || vArr_Rows.Length == 0))
            {
                return new DataTable();
            }
            else
            {
                lDtt_Data = vArr_Rows[0].Table.Clone();
                AddRows(vArr_Rows, ref lDtt_Data);
                return lDtt_Data;
            }
        }

        public static void AddView(ref System.Data.DataView rDtv_Source, ref System.Data.DataTable rDtt_Destination)
        {
            System.Data.DataRowView lDrv_Sorce = null;

            // inicia insercao
            rDtt_Destination.BeginLoadData();

            // percorre e importa
            foreach (DataRowView lDrv_Sorce_loopVariable in rDtv_Source)
            {
                lDrv_Sorce = lDrv_Sorce_loopVariable;
                // copia a linha atual
                AddRowView(lDrv_Sorce, ref rDtt_Destination);
            }

            // fim
            rDtt_Destination.EndLoadData();

        }

        public static System.Data.DataRow AddRowView(System.Data.DataRowView vDrv_Source, ref System.Data.DataTable rDtt_Destination)
        {
            System.Data.DataRow lDtr_Destination = null;
            System.Data.DataColumn lDtc_Temp = null;
            bool lBln_Ok = false;

            // cria uma estrutura para nova linha no destino
            lDtr_Destination = rDtt_Destination.NewRow();

            // percorre as colunas da origem

            foreach (DataColumn lDtc_Temp_loopVariable in vDrv_Source.DataView.Table.Columns)
            {
                lDtc_Temp = lDtc_Temp_loopVariable;
                // tem no destino?

                if ((rDtt_Destination.Columns.Contains(lDtc_Temp.ColumnName) == true))
                {
                    // marca flag para indicar que pelo menos uma coluna foi copiada
                    lBln_Ok = true;

                    // copia
                    lDtr_Destination[lDtc_Temp.ColumnName] = vDrv_Source[lDtc_Temp.ColumnName];

                }

            }

            // nova linha valida?

            if ((lBln_Ok == true))
            {
                // passa para o destino
                rDtt_Destination.Rows.Add(lDtr_Destination);

                // retorna a linha incluida
                return lDtr_Destination;
            }
            else
            {
                return null;
            }

        }

        public static System.Data.DataRow[] AddRows(System.Data.DataRow[] vDtr_Source, ref System.Data.DataTable rDtt_Destination)
        {
            System.Data.DataRow lDtr_Destination = null;
            System.Data.DataColumn lDtc_Temp = null;
            bool lBln_Ok = false;
            DataRow lDtr_Source = null;
            ArrayList lObj_NewRows = new ArrayList();


            foreach (DataRow lDtr_Source_loopVariable in vDtr_Source)
            {
                lDtr_Source = lDtr_Source_loopVariable;
                // cria uma estrutura para nova linha no destino
                lDtr_Destination = rDtt_Destination.NewRow();

                // percorre as colunas da origem

                foreach (DataColumn lDtc_Temp_loopVariable in lDtr_Source.Table.Columns)
                {
                    lDtc_Temp = lDtc_Temp_loopVariable;
                    // tem no destino?

                    if ((rDtt_Destination.Columns.Contains(lDtc_Temp.ColumnName) == true))
                    {
                        // marca flag para indicar que pelo menos uma coluna foi copiada
                        lBln_Ok = true;

                        // copia
                        lDtr_Destination[lDtc_Temp.ColumnName] = lDtr_Source[lDtc_Temp.ColumnName];

                    }

                }

                // nova linha valida?

                if ((lBln_Ok == true))
                {
                    // passa para o destino
                    rDtt_Destination.Rows.Add(lDtr_Destination);

                    lObj_NewRows.Add(lDtr_Destination);

                }
            }

            // retorna a linha incluida
            return (DataRow[])lObj_NewRows.ToArray(typeof(DataRow));

        }

        public static void CopyRowValues(System.Data.DataRow vDtr_Source, ref System.Data.DataRow rDtr_Destination)
        {
            System.Data.DataColumn lObj_Column = null;

            // percorre as colunas
            foreach (DataColumn lObj_Column_loopVariable in vDtr_Source.Table.Columns)
            {
                lObj_Column = lObj_Column_loopVariable;
                // destino possui essa coluna?
                if ((rDtr_Destination.Table.Columns.Contains(lObj_Column.ColumnName) == true))
                {
                    // copia valores
                    rDtr_Destination[lObj_Column.ColumnName] = vDtr_Source[lObj_Column.ColumnName];
                }
            }

        }

    }
}
