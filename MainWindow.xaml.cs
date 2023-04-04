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

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            CheckBox item = new CheckBox();
            item.Content = tbxToevoegen.Text;
            lbxLichtkrant.Items.Add(item);
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            SendData();
        }

        private List<Tuple<string, string>> GetSelectedMessages()
        {
            var messages = new List<Tuple<string, string>>();

            // Aangevinkte checkboxes toevoegen aan de lijst met berichten
            if (cbx1.IsChecked == true)
            {
                messages.Add(new Tuple<string, string>(cbx1.Content.ToString(), "A"));
            }
            if (cbx2.IsChecked == true)
            {
                messages.Add(new Tuple<string, string>(cbx2.Content.ToString(), "B"));
            }

            return messages;
        }

        private void SendData()
        {
            foreach (var message in GetSelectedMessages())
            {
                // Berichttekst en pagina gebruiken om het commando samen te stellen
                _serialPort.Write("<ID01><RP" + message.Item2 + ">" + message.Item1 + Convert.ToChar(13) + Convert.ToChar(10));
            }

            //string cbx1text = "";
            //string cbx2text = "";
            //string pagina = "A";

            //if (cbx1.IsChecked == true)
            //{
            //    cbx1text = (cbx1.Content.ToString());
            //    _serialPort.Write("<ID01><RP" + pagina + ">" + cbx1text + Convert.ToChar(13) + Convert.ToChar(10));
            //}

            //if (cbx2.IsChecked == true)
            //{
            //    cbx2text = (cbx2.Content.ToString());
            //    _serialPort.Write("<ID01><RP" + pagina + ">" + cbx2text + Convert.ToChar(13) + Convert.ToChar(10));
            //}
        }
    }
}
