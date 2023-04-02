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
using System.IO.Ports;
using System.Drawing;

namespace Cédric_Vindevogel___Project_OOP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort _serialPort;

        public MainWindow()
        {
            InitializeComponent();

            _serialPort = new SerialPort("COM3");
            _serialPort.Open();

            //cbxComPorts.Items.Add("None");
            //foreach (string port in SerialPort.GetPortNames())
            //    cbxComPorts.Items.Add(port);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ((_serialPort != null) && (_serialPort.IsOpen))
            {
                _serialPort.Write(new byte[] { 0 }, 0, 1);

                // Gooi alle info van de _serialPort in de vuilbak.
                _serialPort.Dispose();
            }
        }

        //private void cbxComPorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (_serialPort != null)
        //    {
        //        if (_serialPort.IsOpen)
        //            _serialPort.Close();

        //        if (cbxComPorts.SelectedItem.ToString() != "None")
        //        {
        //            _serialPort.PortName = cbxComPorts.SelectedItem.ToString();
        //            _serialPort.Open();
        //        }
        //    }
        //}

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            CheckBox item = new CheckBox();
            item.Content = tbxToevoegen.Text;
            lbxLichtkrant.Items.Add(item);
            SendData();
        }

        private void SendData()
        {
            string pagina = "A";

            _serialPort.Write("<ID01><RP" + pagina + ">" + Convert.ToChar(13) + Convert.ToChar(10));
        }
    }
}
