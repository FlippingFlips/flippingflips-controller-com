using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace FF.Sim.COM.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string net472 = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe";
        const string net472_64 = @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe";
        const string flipsSimComDll = "FF.Sim.COM.dll";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RunSystemFileChecks())
                {
                    MainGrid.Visibility = Visibility.Hidden;

                    var result = RunProcess(true);

                    if (result > 0) ShowMessage("An error with regasm. Code: " + result);
                    else { ShowMessage(flipsSimComDll + " Registered."); }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("An error occured. " + ex.Message);
            }
            finally
            {
                MainGrid.Visibility = Visibility.Visible;
            }
        }        

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RunSystemFileChecks())
                {
                    MainGrid.Visibility = Visibility.Hidden;

                    var result = RunProcess(false);

                    if (result > 0) ShowMessage("An error with regasm. Code: " + result);
                    else { ShowMessage(flipsSimComDll + " UnRegistered."); }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("An error occured. " + ex.Message);
            }
            finally
            {
                MainGrid.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Runs regasm to install controller types
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        private int RunProcess(bool register)
        {
            var proc = new Process();
            var dllPath = System.IO.Path.GetFullPath(flipsSimComDll);
            var regasmArgs = " \"" + dllPath + "\" ";
            regasmArgs += register ? " /codebase" : " /u";
            var sinfo = new ProcessStartInfo(net472, regasmArgs);
            proc.StartInfo = sinfo;
            proc.Start();
            proc.WaitForExit();
            return proc.ExitCode;
        }

        /// <summary>
        /// Check if regasm available and controller dll is available
        /// </summary>
        /// <returns></returns>
        private bool RunSystemFileChecks()
        {
            if (!File.Exists(net472))
            {
                ShowMessage("ERROR: Net framework isn't installed." + net472);
                return false;
            }
            if (!File.Exists(flipsSimComDll))
            {
                ShowMessage("ERROR:" + flipsSimComDll + " not found to register");
                return false;
            }

            return true;
        }

        void ShowMessage(string message) => MessageBox.Show(message);
    }
}
