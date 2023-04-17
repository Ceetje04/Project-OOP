using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Cédric_Vindevogel___Project_OOP
{
    public class SeriëlePoort
    {

        SerialPort _serialPort;

        public void OpenPoort(string poortNaam)
        {
            _serialPort = new SerialPort();
            _serialPort.PortName = poortNaam;
            _serialPort.Open();
        }

        public void SluitPoort()
        {
            if ((_serialPort != null) && (_serialPort.IsOpen))
            {
                _serialPort.Write(new byte[] { 0 }, 0, 1);
                _serialPort.Close();
                _serialPort.Dispose();
            }
        }

        public void Schrijf(string data)
        {
            try
            {
                _serialPort.Write(data);

            }
            catch(LicktkrantException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //public void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        //{
        //    string tekst = _serialPort.ReadLine();

        //    Dispatcher.Invoke(new Action<string>(UpdateLabel), tekst);
        //}

        //private void UpdateLabel(string text)
        //{
        //    lblControle.Content = text;
        //}
    }
}
