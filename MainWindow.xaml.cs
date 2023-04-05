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
using System.IO;

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

            _serialPort = new SerialPort();

            cbxComPorts.Items.Add("None");
            foreach (string port in SerialPort.GetPortNames())
                cbxComPorts.Items.Add(port);
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

        private void cbxComPorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();

                if (cbxComPorts.SelectedItem.ToString() != "None")
                {
                    _serialPort.PortName = cbxComPorts.SelectedItem.ToString();
                    _serialPort.Open();
                }
            }
        }

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

        private List<Tuple<string, string>> GetSelectedMessages() // Maakt een lijst van berichten en de pagina waarbij ze horen.
        {
            var messages = new List<Tuple<string, string>>(); // Tuple is om meedere dingen samen te versturen.

            //if (cbx1.IsChecked == true) // Als de checkbox is aangevinkt.
            //{
            //    messages.Add(new Tuple<string, string>("A", cbx1.Content.ToString()));
            //}
            //if (cbx2.IsChecked == true)
            //{
            //    messages.Add(new Tuple<string, string>("B", cbx2.Content.ToString()));
            //}
            ////if (cbx3.IsChecked == true)
            ////{
            ////    messages.Add(new Tuple<string, string>("C", cbx3.Content.ToString()));
            ////}
            ///

            int pageNumber = 1;
            foreach (CheckBox cb in lbxLichtkrant.Items) // Voor alle checkboxes in de list wordt deze code doorlopen.
            {
                if (cb.IsChecked == true) // Als de checkbox is aangevinkt wordt een nieuwe boodschap toegevoegd aan de list.
                {
                    string pageLetter = ((char)('A' + pageNumber - 1)).ToString(); // Bereken de paginaletter op basis van het paginanummer.
                    messages.Add(new Tuple<string, string>(pageLetter, cb.Content.ToString())); // Voeg het bericht toe aan de lijst met de juiste paginainformatie.
                    pageNumber++; // Verhoog het paginanummer voor de volgende checkbox.
                }
            }

            using StreamWriter writer = File.CreateText(@"C:\Users\cedri\Desktop\boodschap.txt"); // Maak een bestand aan met de naam boodschap

            foreach (var message in messages)
            {
                writer.WriteLine(message.Item1 + "," + message.Item2);
            }

            return messages;
        }

        private void SendData()
        {
            foreach (var message in GetSelectedMessages()) // Verstuur alle commando's waarvan de checkbox is aangevinkt.
            {
                _serialPort.Write("<ID01><RP" + message.Item1 + ">" + message.Item2 + Convert.ToChar(13) + Convert.ToChar(10));
            }
        }
    }
}
