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
using System.Timers;
using System.Threading;
using System.Windows.Threading;

namespace Cédric_Vindevogel___Project_OOP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*SerialPort _serialPort*/
        private DispatcherTimer _timer;
        private int _currentPageIndex = 0;
        private List<Tuple<string, string>> _messages;
        SeriëlePoort _serial = new SeriëlePoort();

        public MainWindow()
        {
            InitializeComponent();

            //_serialPort = new SerialPort();

            cbxComPorts.Items.Add("None");
            foreach (string port in SerialPort.GetPortNames())
                cbxComPorts.Items.Add(port);
        }

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    LoadSerialPorts();
        //    StartSwitchingPages();
        //}

        //private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    if ((_serial != null) && (_serialPort.IsOpen))
        //    {
        //        _serialPort.Write(new byte[] { 0 }, 0, 1);

        //        // Gooi alle info van de _serialPort in de vuilbak.
        //        _serialPort.Dispose();
        //    }
        //}

        private void cbxComPorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbxComPorts.SelectedItem.ToString() != "None")
                {
                    _serial.OpenPoort(cbxComPorts.SelectedItem.ToString());
                    SwitchPage();
                }
                else
                {
                    _serial.SluitPoort();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kies de juiste seriële poort.");
            } 
            //try
            //{
            //    if (_serialPort != null)
            //    {
            //        if (_serialPort.IsOpen)
            //            _serialPort.Close();

            //        if (cbxComPorts.SelectedItem.ToString() != "None")
            //        {
            //            _serialPort.PortName = cbxComPorts.SelectedItem.ToString();
            //            _serialPort.Open();
            //            SwitchPage();
            //        }
            //        //if (cbxComPorts.SelectedItem != null)
            //        //{
            //        //    string selectedPoort = cbxComPorts.SelectedItem.ToString();
            //        //    SeriëlePoort serial = new SeriëlePoort();
            //        //    serial.OpenPoort(selectedPoort);
            //        //}
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Kies de juiste seriële poort.");
            //}
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            CheckBox item = new CheckBox();
            item.Content = tbxToevoegen.Text;
            lbxLichtkrant.Items.Add(item);
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SwitchPage();
                foreach (var message in SelectedMessages()) // Verstuur alle tekst van de checkboxen die zijn aangevinkt.
                {
                    _serial.Write("<ID01><P" + message.Item1 + ">" + "<SB>" + "<FS>" + message.Item2 + "     " + Convert.ToChar(13) + Convert.ToChar(10));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kies eerst een seriële poort.");
            }
        }

        private List<Tuple<string, string>> SelectedMessages() // Maakt een lijst van berichten en de pagina waarbij ze horen.
        {
            var messages = new List<Tuple<string, string>>(); // Tuple is om meedere dingen samen te versturen.

            int pageNumber = 0;
            foreach (CheckBox cb in lbxLichtkrant.Items) // Voor alle checkboxes in de list wordt deze code doorlopen.
            {
                if (cb.IsChecked == true) // Als de checkbox is aangevinkt wordt een nieuwe boodschap toegevoegd aan de list.
                {
                    string pageLetter = ((char)('A' + pageNumber)).ToString(); // Bereken de paginaletter op basis van het paginanummer.
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

        public void SwitchPage()
        {
            _messages = SelectedMessages(); // Bewaar de geselecteerde berichten in een veld.

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(2);
            _timer.Start(); // start de timer
            _timer.Tick += (sender, args) =>
            {
                if (_currentPageIndex < _messages.Count) // Als er nog pagina's zijn om te verzenden.
                {
                    var message = _messages[_currentPageIndex];

                    _serial.Write("<ID01><RP" + message.Item1 + ">"+ Convert.ToChar(13) + Convert.ToChar(10));

                    _currentPageIndex++; // Verhoog de huidige pagina-index voor de volgende verzending.

                }
                else // Als alle pagina's zijn verzonden.
                {
                    _currentPageIndex = 0; // Reset de pagina-index.
                }
            };
        }
    }
}
