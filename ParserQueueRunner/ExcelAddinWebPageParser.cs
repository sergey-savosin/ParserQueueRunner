using Microsoft.Win32;
//using Excel = Microsoft.Office.Interop.Excel;
using NetOffice;
using Excel = NetOffice.ExcelApi;
using ParserQueueRunner.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ParserQueueRunner
{
    public class ExcelAddinWebPageParser : IWebPageParser
    {
        public string GetParserPath()
        {
            const string regRoot = @"HKEY_CURRENT_USER\Software\VB and VBA Program Settings\Parser\Setup";
            string s = (string)Registry.GetValue(regRoot, "AddinPath", "") ?? "";

            return s;
        }

        public WebParserResult ParserCheck(WebParserConfig parserConfig)
        {
            string fileNameParser = this.GetParserPath();

            WebParserResult result = new WebParserResult()
            {
                ParserStatus = "Ok",
                ParserError = ""
            };

            Excel.Application xlApp = null;

            try
            {
                Console.WriteLine("Parser test starting. Please wait...");
                
                // Application
                xlApp = new Excel.Application();

                // открываем Excel файл
                Excel.Workbook xlWb1 = xlApp.Workbooks.Open(fileNameParser);

                xlApp.Run(@"ParserAddinTest");
                //xlWb1.Close(false); // закрываем книгу без сохранения
                xlApp.DisposeChildInstances();

            }
            catch (Exception exc)
            {
                result.ParserStatus = "Error";
                result.ParserError = exc.Message;
            }
            finally
            {
                if (xlApp != null)
                {
                    xlApp.Quit(); //закрываем Excel
                    xlApp.Dispose();
                }
            }

            return result;
        }

        public WebParserResult ParseWebSite(WebParserConfig parserConfig, WebParserParams parserParam)
        {
            string fileNameAddin = this.GetParserPath();

            string parserConfigName = parserConfig.AddinConfigName;

            WebParserResult parserResult = new WebParserResult()
            {
                ParserStatus = "Ok",
                ParserError = ""
            };

            Excel.Application xlApp = null;
            Excel.Workbook xlWb1 = null;
            Excel.Workbook xlWb2 = null;
            Excel.Worksheet xlSht = null;

            try
            {
                // Запустить Application
                xlApp = new Excel.Application();

                // Открыть надстройку
                xlWb1 = xlApp.Workbooks.Open(fileNameAddin);
                
                // Создать лист для ввода данных
                // Excel.Workbook xlWb2 = xlApp.Workbooks.Open(fileNameExcelWorkbook);
                xlWb2 = xlApp.Workbooks.Add();

                //Excel.Worksheet xlSht = xlWb2.Sheets["Лист1"]; //имя листа в файле
                xlSht = xlWb2.Sheets[1] as Excel.Worksheet; //имя листа в файле

                //xlApp.Visible = true;
                //xlApp.DisplayAlerts = false;

                int startRowNumber = parserConfig.StartRowNumber;

                // Вывод заголовка
                if (startRowNumber > 1)
                {
                    xlSht.Cells[1, parserConfig.DealNumberColumn].Value = "Номер документа";
                    xlSht.Cells[1, parserConfig.IsTrackColumn].Value = "Обрабатывать";
                }

                // Заполнение исходных данных на листе
                xlSht.Cells[startRowNumber, parserConfig.DealNumberColumn].Value = parserParam.DocumentNumber;
                xlSht.Cells[startRowNumber, parserConfig.IsTrackColumn].Value = "Да";

                // Запуск парсера
                var res = xlApp.Run(@"StartParser", parserConfigName);
                if (!String.IsNullOrEmpty((string)res))
                {
                    throw new Exception((string)res);
                }

                object row1 = xlSht.Cells[startRowNumber, parserConfig.ResultNumberColumn].Value;
                if (row1 == null)
                {
                    parserResult.ParserStatus = "Not found";
                }
                else
                {
                    string col2HyperlinkAddress = xlSht.Cells[startRowNumber, parserConfig.DealNumberHyperlinkColumn].Hyperlinks[1].Address;
                    string col3 = xlSht.Cells[startRowNumber, parserConfig.DocumentPdfFolderNameColumn].Value.ToString();
                    string col4 = xlSht.Cells[startRowNumber, parserConfig.DocumentPdfUrlColumn].Value.ToString();
                    DateTime col5 = (DateTime)xlSht.Cells[startRowNumber, parserConfig.LastDealDateColumn].Value;

                    parserResult.CardUrl = col2HyperlinkAddress;
                    parserResult.LastDealDate = col5;
                    parserResult.DocumentPdfUrl = col4;
                    parserResult.DocumentPdfFolderName = col3;
                    parserResult.HasAttachment = string.IsNullOrEmpty(col4) ? false : true;
                    parserResult.DocumentPfdPath = GetDocumentFullPath(parserConfig, parserResult, fileNameAddin);
                }
            }
            catch (Exception exc)
            {
                parserResult.ParserStatus = "Error";
                parserResult.ParserError = exc.Message;
            }
            finally
            {
                if (xlWb2 != null)
                {
                    xlWb2.Saved = true;
                }

                xlApp.DisposeChildInstances();

                if (xlApp != null)
                {
                    xlApp.Quit(); //закрываем Excel
                    xlApp.Dispose();
                }
            }

            return parserResult;
        }

        private string GetDocumentFullPath(WebParserConfig parserConfig, WebParserResult parserResult, string addinFullPath)
        {
            string downloadsFolderName = "Downloads";
            string addinConfigName = parserConfig.AddinConfigName;
            string parserDir = Path.GetDirectoryName(addinFullPath);
            string dealName = parserResult.DocumentPdfFolderName;
            string pdfDocumentName = Path.GetFileName(parserResult.DocumentPdfUrl);
            return Path.Combine(parserDir, downloadsFolderName, addinConfigName, dealName, pdfDocumentName);
        }

        private static void NAR(object o)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(o);
            }
            catch { }
            finally
            {
                o = null;
            }
        }

    }
}
