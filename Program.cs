using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace iAR
{
    internal class Program
    {
        static void DebugLogEx(string debugMessage, Exception ex)
        {
            if(debugMessage != null)
            {
                if (ex != null)
                {
                    debugMessage = String.Concat(debugMessage,"\n\n", ex.Message.ToString());

                }
         
                Console.Write(debugMessage);
                
            }
            
            
        }
        static void Main(string[] args)
        {
        
            /*
             * vang de args 
             */
                   string sDownloadUrl = "https://github.com/senery/seneryservice/raw/main/sser.zip";
            if (DownloadAndSave(sDownloadUrl, "service.zip") == true)
            {
                DebugLogEx(sDownloadUrl, null);
            }
            else
            {
                DebugLogEx("Download failed", null);
            }
          
                        DebugLogEx("Geen args", null);
        }
        static void DoInstall()
        {

        }
        static string DownloadServiceZip(string serviceZipUrl)
        {
            string sPathZipFile = "";

            return sPathZipFile;
        }
        static bool DownloadAndSave(string url, string sSaveAs)
        {
            bool isSaved = false;
            WebClient web = new WebClient();
                       
            if (web != null)
            {
                try
                {
                    web.DownloadFile(url, sSaveAs);
                    DebugLogEx(url, null);
                    isSaved = true;
                }
                catch(Exception e)
                {
                    isSaved = false;
                    DebugLogEx("", e);
                }
                isSaved = true;
            }
            if (isSaved)
            {
                try
                {
                    ZipFile.ExtractToDirectory(sSaveAs, Path.GetTempPath());


                }
                catch (Exception e)
                {
                    DebugLogEx("", e);
                }
            }

            return isSaved; 
        }
    }
}
