using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using AForge.Imaging.Filters;
using System.ComponentModel;
using System.Threading;
using System.Collections;
using System.Collections.ObjectModel;
using System.Runtime.Serialization.Json;
using System.Windows.Forms; 

namespace LolTroll
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        BackgroundWorker imageWorker = new BackgroundWorker();
        BackgroundWorker searchWorker = new BackgroundWorker();
        

        private TrollData data = new TrollData();

        public MainWindow()
        {            
            InitializeComponent();
            Debug.WriteLine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            this.DataContext = data;
            trollTable.ItemsSource = data.TrollList;

            imageWorker.DoWork += (s, e) =>
            {
                Bitmap source = (Bitmap)e.Argument;
                fetchBitmapNames(source);
                fetchStringNames();
            };

            searchWorker.DoWork += (s, e) =>
            {
                for (int i = 0; i < data.Sections.Length; i++)
                {
                    Troll troll = getTroll(data.TrollNames[i]);
                    if (troll == null)
                    {
                        data.BackgroundInfo[i] = System.Windows.Media.Brushes.Green;
                        data.FucktardInfo[i] = "";
                    }
                    else
                    {
                        data.BackgroundInfo[i] = System.Windows.Media.Brushes.Red;
                        data.FucktardInfo[i] = "" + troll.FucktardLevel;
                    }
                }
            };

            
            


            
        }

        private void getSummonersButtonClick(object sender, RoutedEventArgs e)
        {
            if (!imageWorker.IsBusy)
            {

                Process[] lolProcesses = Process.GetProcessesByName("LolClient");

                if (lolProcesses.Length > 0)
                {
                    IntPtr window = FindWindow(null, lolProcesses[0].ProcessName);
                    System.Drawing.Image im = Utilities.CaptureWindow(lolProcesses[0].MainWindowHandle);

                    Bitmap source = new Bitmap(im);
                    imageWorker.RunWorkerAsync(source);
                }
                else
                {
                    string message = "The client window with the summoners names cannot be found. Please be sure to be in the champion select room";
                    string caption = "Client window dont found";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    System.Windows.Forms.MessageBox.Show(message, caption, buttons);
                }
            }

        }

        private void fetchBitmapNames(Bitmap source)
        {

            for (var i = 0; i < data.Sections.Length; i++)
            {
                System.Drawing.Rectangle section = data.Sections[i];
                Bitmap bmp = new Bitmap(section.Width, section.Height);
                Graphics g = Graphics.FromImage(bmp);

                g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
                FiltersSequence filters = new FiltersSequence(
                    new Grayscale(0.2125, 0.7154, 0.0721),
                    new ResizeBicubic(500, 47),
                    new Shrink(System.Drawing.Color.Black),
                    new Invert()
                    );
                bmp = filters.Apply(bmp);
                bmp.Save(data.DataDir + @"\crop" + i + ".tif", ImageFormat.Tiff);
            }
        }

        
        private void fetchStringNames()
        {
            TesseractOCR ocr = new TesseractOCR(data.WorkingDir+ @"\Tesseract-OCR\tesseract.exe");
            for (int i = 0; i < 5; i++)
            {
                //data.TrollNames[i] = ocr.OCRFromBitmap(new Bitmap(data.DataDir + @"\crop"+i+".tif"));
                data.TrollNames[i] = ocr.OCRFromFile(data.DataDir + @"\crop" + i + ".tif");
                //data.TrollNames[i] = "" + i;
            }
        }

        private Troll getTroll(string name)
        {
            // Define the query expression.
            IEnumerable<Troll> scoreQuery =
                from troll in data.TrollList
                where troll.Name == name
                select troll;

            // Execute the query.
            if (scoreQuery.Count() > 0)
            {
                //Encontrado
                return scoreQuery.First();
            }
            else return null;
        }

        private void saveDatabaseButtonClick(object sender, RoutedEventArgs routedEvent)
        {
            // Displays a SaveFileDialog so the user can save the troll list
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "JSON File|*.json";
            saveFileDialog1.Title = "Save troll database";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                BackgroundWorker databaseWorker = new BackgroundWorker();
                databaseWorker.DoWork += (s, e) => {
                    MemoryStream stream1 = new MemoryStream();
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<Troll>));
                    ser.WriteObject(stream1, data.TrollList);
                    stream1.Position = 0;
                    StreamReader sr = new StreamReader(stream1);
                    System.IO.File.WriteAllText(@saveFileDialog1.FileName, sr.ReadToEnd());
                };
                databaseWorker.RunWorkerAsync();
            }

            
        }


        private void loadDatabaseButtonClick(object sender, RoutedEventArgs routedEvent)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".json";
            dlg.Filter = "JSON Files (*.json)|*.json";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                data.TrollList.Clear();
                BackgroundWorker databaseWorker = new BackgroundWorker();
                databaseWorker.WorkerReportsProgress = true;
                databaseWorker.ProgressChanged += (s, e) =>
                {
                    foreach (Troll troll in (List<Troll>)e.UserState)
                    {
                        data.TrollList.Add(troll);
                    }
                };
                
                databaseWorker.DoWork += (s, e) =>
                {
                    MemoryStream ms = new MemoryStream();
                    using (FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    {
                        byte[] bytes = new byte[file.Length];
                        file.Read(bytes, 0, (int)file.Length);
                        ms.Write(bytes, 0, (int)file.Length);
                    }

                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<Troll>));
                    ms.Position = 0;
                    List<Troll> l = (List<Troll>)ser.ReadObject(ms);
                    databaseWorker.ReportProgress(1,l);
                };
                databaseWorker.RunWorkerAsync();


            }
        }

        private void searchTrollsButtonClick(object sender, RoutedEventArgs e)
        {

            if (!searchWorker.IsBusy)
            {
                searchWorker.RunWorkerAsync();
            }
        }
    }
}
