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
        private SerialPort _serialPort; // Variabele voor seriële poort.

        private DispatcherTimer _timer;
        private List<Tuple<string, string>> _messages;

        int _currentPageIndex = 0; // Variabele voor huidige pagina van de lichtkrant in cijfers.
        int pageNumber = 0; // Variabele voor huidige pagina van de lichtkrant in cijfers.
        string pageLetter; // Variabele pageletter voor de pagina van de lichtkrant

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
                    SwitchPage(); // Start de method switchpage van zodra er op de knop gedrukt wordt.
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

        private void btnToevoegen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckBox item = new CheckBox(); // Variabele item aanmaken voor een nieuwe textbox.
                // Als de textbox één van volgende tekens bevat. 
                if (tbxToevoegen.Text.Contains("€") || tbxToevoegen.Text.Contains("é") || tbxToevoegen.Text.Contains("ë") || tbxToevoegen.Text.Contains("£"))
                {
                    throw new OngeldigeTekensException();
                }
                // Als de textbox leeg is of alleen spaties bevat.
                if (string.IsNullOrWhiteSpace(tbxToevoegen.Text))
                {
                    throw new GeenInvoerException();
                }
                // Als aan alle voorwaarden voldaan is wordt er een checkbox toegevoegd.
                else
                {
                    lbxLichtkrant.Items.Add(item);
                    item.Content = tbxToevoegen.Text;
                }
            }
            catch (OngeldigeTekensException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (GeenInvoerException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var message in SelectedMessages()) // Lus waar alle geselcteerde berichten worden opgehaald uit de list.
                {
                    // Voor elke geselecteerde checkbox wordt het protocol verstuurd.
                    _serialPort.Write("<ID01><P" + message.Item1 + ">" + "<SB>" + "<FS>" + message.Item2 + "      " + Convert.ToChar(13) + Convert.ToChar(10));
                    Thread.Sleep(1000); // Wacht één seconde omdat de communicatie anders te snel gaat voor de lichtkrant.
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("Fout bij het openen van seriële poort.");
            }
        }

        private List<Tuple<string, string>> SelectedMessages() // Maakt een lijst van berichten en de pagina waarbij ze horen.
        {
            var messages = new List<Tuple<string, string>>(); // Tuple is om meedere onjecten samen te versturen. Maak een nieuwe lijst aan.

            foreach (CheckBox cb in lbxLichtkrant.Items) // Lus waarin alle checkboxes worden doorlopen.
            {
                if (cb.IsChecked == true) // Enkel van de checkboxes die zijn aangevinkt wordt de boodschap opgeslagen in de list.
                {
                    pageLetter = ((char)('A' + pageNumber)).ToString(); // Bereken de paginaletter op basis van het paginanummer. Je gaat dus van cijfers naar letters.
                    messages.Add(new Tuple<string, string>(pageLetter, cb.Content.ToString())); // Voeg het bericht toe aan de lijst met de juiste paginainformatie.
                    pageNumber++; // Verhoog het paginanummer voor de volgende checkbox.
                }
            }

            SaveMessages(messages); // De aangemaakte lijst wordt opgeslagen.

            return messages; // De lijst wordt geretourneerd voor de update method.
        }

        private void SaveMessages(List<Tuple<string, string>> messages) // Deze methode neemt de aangemaakte lijst als invoer.
        {
            using StreamWriter writer = File.CreateText(@"./boodschap.txt"); // Maak een bestand aan met de naam boodschap in de huidige programmafolder.

            foreach (var message in messages) // Lus die elke boodschap in de lijst zal opslaan.
            {
                writer.WriteLine(message.Item1 + "," + message.Item2); // Sla eerst de paginaletter op daarna de tekst.
            }
        }

        public void SwitchPage()
        {
            _messages = SelectedMessages(); // Bewaar de geselecteerde berichten in een veld.

            _timer = new DispatcherTimer(); // Maak een nieuwe dispatchtimer.
            _timer.Interval = TimeSpan.FromSeconds(25); // De timer heeft een interval van 25 seconden.
            _timer.Start(); // start de timer
            _timer.Tick += (sender, args) => // Telkens wanneer er 25 seconden voorbij zijn.
            {
                if (_currentPageIndex < _messages.Count) // Als de huidige pagina kleiner is dan het totale aantal pagina's.
                {
                    var message = _messages[_currentPageIndex]; // De volgende pagina in de lijst wordt geselcteerd volgens currentPageIndex.

                    _serialPort.Write("<ID01><RP" + message.Item1 + ">"+ Convert.ToChar(13) + Convert.ToChar(10));

                    _currentPageIndex++; // Verhoog de huidige pagina-index voor de volgende verzending.

                }
                else // Als alle pagina's zijn verzonden.
                {
                    _currentPageIndex = 0; // Reset de pagina-index. En begin opnieuw.
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
            lblControle.Content = data; // Toon de geretourneerde text van de lichtkrant op een label.
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
