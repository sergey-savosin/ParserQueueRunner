using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;
using ParserQueueRunner.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            try
            {
                Console.WriteLine("Parser test starting. Please wait...");
                
                // Application
                Excel.Application xlApp = new Excel.Application();

                // открываем Excel файл
                Excel.Workbook xlWb1 = xlApp.Workbooks.Open(fileNameParser);

                xlApp.Run(@"ParserAddinTest");
                xlWb1.Close(false); // закрываем книгу без сохранения
                NAR(xlWb1);
                xlApp.Quit(); //закрываем Excel
                NAR(xlApp);
                GC.Collect();
            }
            catch (Exception exc)
            {
                result.ParserStatus = "Error";
                result.ParserError = exc.Message;
            }

            return result;
        }

        public WebParserResult ParseWebSite(WebParserConfig config, WebParserParams param)
        {
            throw new NotImplementedException();
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
