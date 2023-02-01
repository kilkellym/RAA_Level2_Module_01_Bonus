using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace RAA_Level2_Module_01_Bonus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string directory;
        public MainWindow()
        {
            InitializeComponent();
            directory = "";
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog selectFolder = new FolderBrowserDialog();
            selectFolder.ShowNewFolderButton = false;
            selectFolder.RootFolder = Environment.SpecialFolder.MyComputer;

            if(selectFolder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // get the selected folder path
                directory = selectFolder.SelectedPath;
                tbxFolder.Text = directory;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (directory == "")
            {
                System.Windows.Forms.MessageBox.Show("Please select a file.");
                this.Close();
            }

            this.Close();

            // do something interesting
            int counter = 0;
            string logPath = "";

            // create list for log file
            List<string> deletedFileLog = new List<string>();
            deletedFileLog.Add("The following backup files have been deleted:");

            // get files from selected folder
            string[] files;
            if(cbxSubFolders.IsChecked == true)
                files = Directory.GetFiles(directory, "*.r*", SearchOption.AllDirectories);
            else
                files = Directory.GetFiles(directory, "*.r*", SearchOption.TopDirectoryOnly);
            
            // loop through files
            foreach(string currentFile in files)
            {
                // check if file is a Revit file
                if (System.IO.Path.GetExtension(currentFile) == ".rvt" || 
                    System.IO.Path.GetExtension(currentFile) == ".rfa")
                {
                    // get the last 9 characters of filename to check if backup
                    string checkString = currentFile.Substring(currentFile.Length - 9, 9);

                    if(checkString.Contains(".0") == true)
                    {
                        // log file path
                        deletedFileLog.Add(currentFile);

                        // delete the file
                        File.Delete(currentFile);
                        counter++;
                    }
                }
            }
            
            // output log file
            if(counter > 0)
            {
                logPath = WriteListToTxt(directory, deletedFileLog);

                string results = "Deleted " + counter.ToString() + " backup files. Show log file?";
                MessageBoxResult result = System.Windows.MessageBox.Show(results, "Complete", System.Windows.MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                    Process.Start(logPath);
            }
        }

        private string WriteListToTxt(string logPath, List<string> deletedFileLog)
        {
            string fileName = "_Delete Backup Files.txt";
            string fullPath = logPath + "\\" + fileName;

            File.WriteAllLines(fullPath, deletedFileLog);

            return fullPath;
        }

        private void btnCanel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
