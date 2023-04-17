using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cédric_Vindevogel___Project_OOP
{
    public class SeriëlePoortOntvangen: SeriëlePoort
    {
        public string OntvangData()
        {
            return _serialPort.ReadLine();
        }
    }

}
