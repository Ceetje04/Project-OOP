using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cédric_Vindevogel___Project_OOP
{
    public class COMPoortInGebruikException : InvalidOperationException
    {
        public COMPoortInGebruikException()
            : base("Kies een geldige COM-poort.")
        {

        }
    }

    public class GeenCOMPoortException : InvalidOperationException
    {
        public GeenCOMPoortException()
            : base("Fout bij het openen van seriële poort.")
        {

        }
    }

    public class OngeldigeTekensException : ApplicationException
    {
        public OngeldigeTekensException()
            : base("Gebruik geen tekens die niet in de ASCII-tabel staan.")
        {
        }
    }

    //public class LichtkrantException : ApplicationException
    //{
    //    public LichtkrantException() : base() { }

    //    public LichtkrantException(string message) : base(message) { }

    //    public LichtkrantException(string message, Exception innerException) : base(message, innerException) { }
    //}
}
