﻿using Microsoft.Win32;
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
            string fileNameParser = parserConfig.AddinPath;

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
            string fileNameAddin = parserConfig.AddinPath;
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

                xlSht.Cells[1, "B"].Value = "Номер документа";
                xlSht.Cells[1, "F"].Value = "Обрабатывать";
                xlSht.Cells[2, "B"].Value = parserParam.DocumentNumber;
                xlSht.Cells[2, "F"].Value = "Да";
                // Заполнение данных на листе

                // Запуск парсера
                var res = xlApp.Run(@"StartParser", parserConfigName);
                if (!String.IsNullOrEmpty((string)res))
                {
                    throw new Exception((string)res);
                }

                object row1 = xlSht.Cells[2, "A"].Value;
                if (row1 == null)
                {
                    parserResult.ParserStatus = "Not found";
                }
                else
                {
                    string col2HyperlinkAddress = xlSht.Cells[2, "B"].Hyperlinks[1].Address;
                    string col3 = xlSht.Cells[2, "C"].Value.ToString();
                    string col4 = xlSht.Cells[2, "D"].Value.ToString();
                    DateTime col5 = (DateTime)xlSht.Cells[2, "E"].Value;

                    parserResult.CardUrl = col2HyperlinkAddress;
                    parserResult.LastDealDate = col5;
                    parserResult.DocumentPdfUrl = col4;
                    parserResult.DocumentPdfFolderName = col3;
                    parserResult.HasAttachment = string.IsNullOrEmpty(col4) ? false : true;
                    parserResult.DocumentPfdPath = GetDocumentFullPath(parserConfig, parserResult);
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

        private string GetDocumentFullPath(WebParserConfig parserConfig, WebParserResult parserResult)
        {
            string downloadsFolderName = "Downloads";
            string addinConfigName = parserConfig.AddinConfigName;
            string parserDir = Path.GetDirectoryName(parserConfig.AddinPath);
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
