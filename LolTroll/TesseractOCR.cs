using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace LolTroll
{
    // usage:
    //
    // TesseractOCR ocr = TesseractOCR(@"C:\bin\tesseract.exe");
    // string result = ocr.OCRFromBitmap(bmp);
    // textBox1.Text = result;
    //


    public class TesseractOCR
    {
        private string commandpath;
        private string outpath;
        private string tmppath;

        public TesseractOCR(string commandpath)
        {
            this.commandpath = commandpath;
            tmppath = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\out.tif";
            outpath = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\out.txt";
        }
        public string analyze(string filename)
        {
            if (Environment.GetEnvironmentVariable("TESSDATA_PREFIX") == null)
                // If it doesn't exist, create it.
                Environment.SetEnvironmentVariable("TESSDATA_PREFIX", Directory.GetCurrentDirectory()+"/Tesseract-OCR/");
            else
            {
                Environment.SetEnvironmentVariable("TESSDATA_PREFIX", Directory.GetCurrentDirectory() + "/Tesseract-OCR/");
            }


            string args = filename + " " + outpath.Replace(".txt", "");
            ProcessStartInfo startinfo = new ProcessStartInfo(commandpath, args);

            startinfo.CreateNoWindow = true;
            startinfo.UseShellExecute = false;
            startinfo.RedirectStandardOutput = true;


            Process proc = Process.Start(startinfo);

            proc.WaitForExit();
            
            string ret = "";
            using (StreamReader r = new StreamReader(outpath))
            {
                string content = r.ReadToEnd();
                ret = content;
            }
            File.Delete(outpath);
            return ret;
        }
        public string OCRFromBitmap(Bitmap bmp)
        {
            bmp.Save(tmppath, System.Drawing.Imaging.ImageFormat.Tiff);
            string ret = analyze(tmppath);
            File.Delete(tmppath);
            return ret;
        }
        public string OCRFromFile(string filename)
        {
            return analyze(filename);
        }
    }
}
