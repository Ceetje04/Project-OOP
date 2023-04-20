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
using static System.Net.Mime.MediaTypeNames;

namespace Cédric_Vindevogel___Project_OOP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialPort _serialPort;

        private DispatcherTimer _timer;
        private int _currentPageIndex = 0;
        private List<Tuple<string, string>> _messages;

        public MainWindow()
        {
            InitializeComponent();

            // Maak een nieuwe serialport aan.
            _serialPort = new SerialPort();

            // Komt er data binnen op de seriële poort, vang ze op...
            _serialPort.DataReceived += _serialPort_DataReceived;


            cbxComPorts.Items.Add("None");
            foreach (string port in SerialPort.GetPortNames()) // Zoek de beschikbare poorten.
            {
                cbxComPorts.Items.Add(port);
            }
        }

        private void cbxComPorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (_serialPort != null)
                {
                    // Start de seriele poort als hij beschikbaar is.
                    if (_serialPort.IsOpen)
                        _serialPort.Close();

                    if (cbxComPorts.SelectedItem.ToString() != "None")
                    {
                        _serialPort.PortName = cbxComPorts.SelectedItem.ToString();
                        _serialPort.Open();
                    }
                }
            }
            catch (InvalidOperationException ex) // Als de er geen geldige poort gekozen is of de poort is al in gebruik.
            {
                MessageBox.Show("Kies een geldige COM-poort die nog niet in gebruik is.");
            }
        }

        public void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Lees alle tekst in, tot je een 'nieuwe lijn symbool' binnenkrijgt.
            // New line = '\n' = ASCII-waarde 10 = ALT 10.
            string receivedText = _serialPort.ReadLine();

            // Geef de ontvangen data door aan een method die op de UI thread loopt.
            // Doe dat via een Action delegate... Delegates en Events zullen 
            // in detail behandeld worden in het vak OOP.
            Dispatcher.Invoke(new Action<string>(LabelControle), receivedText);
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Als de seriale poort beschikbaar is en wordt data verstuurd dan mag de tweede window geopend worden.
                if ((_serialPort.IsOpen) && (_serialPort != null))
                {
                    btnStart.Background = new SolidColorBrush(Colors.LightGreen);
                    SwitchPage();
                }
                else
                {
                    btnStart.Background = new SolidColorBrush(Colors.Red);
                    throw new GeenCOMPoortException();
                }
            }
            catch (GeenCOMPoortException ex) // Geef een waarschuwing wanneer er geen COM-poort geselecteerd is.
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckBox item = new CheckBox();
                item.Content = tbxToevoegen.Text;
                lbxLichtkrant.Items.Add(item);
                if (tbxToevoegen.Text.Contains("€") || tbxToevoegen.Text.Contains("é") || tbxToevoegen.Text.Contains("ë") || tbxToevoegen.Text.Contains("£"))
                {
                    throw new OngeldigeTekensException();
                }

            }
            catch (OngeldigeTekensException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SwitchPage();
                ReadMessages();
                foreach (var message in SelectedMessages()) // Verstuur alle tekst van de checkboxen die zijn aangevinkt.
                {
                    _serialPort.Write("<ID01><P" + message.Item1 + ">" + "<SB>" + "<FS>" + message.Item2 + "     " + Convert.ToChar(13) + Convert.ToChar(10));
                    Thread.Sleep(1000);
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("Fout bij het openen van seriële poort.");
            }
            //catch (GeenCOMPoortException ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void ReadMessages()
        {
            var messages = new List<Tuple<string, string>>();
            using StreamReader reader = File.OpenText(@"C:\Users\cedri\Desktop\boodschap.txt");
            string line;
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

            SaveMessages(messages);

            return messages;
        }

        private void SaveMessages(List<Tuple<string, string>> messages)
        {
            using StreamWriter writer = File.CreateText(@"C:\Users\cedri\Desktop\boodschap.txt"); // Maak een bestand aan met de naam boodschap

            foreach (var message in messages)
            {
                writer.WriteLine(message.Item1 + "," + message.Item2);
            }
        }

        public void SwitchPage()
        {
            _messages = SelectedMessages(); // Bewaar de geselecteerde berichten in een veld.

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(25);
            _timer.Start(); // start de timer
            _timer.Tick += (sender, args) =>
            {
                if (_currentPageIndex < _messages.Count) // Als er nog pagina's zijn om te verzenden.
                {
                    var message = _messages[_currentPageIndex];

                    _serialPort.Write("<ID01><RP" + message.Item1 + ">"+ Convert.ToChar(13) + Convert.ToChar(10));

                    _currentPageIndex++; // Verhoog de huidige pagina-index voor de volgende verzending.

                }
                else // Als alle pagina's zijn verzonden.
                {
                    _currentPageIndex = 0; // Reset de pagina-index.
                }
            };
        }

        private void btnVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            object selectedItem = lbxLichtkrant.SelectedItem;
            if (selectedItem != null)
                lbxLichtkrant.Items.Remove(selectedItem);
        }

        private void LabelControle(string data)
        {
            lblControle.Content = data;
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
    }
}
