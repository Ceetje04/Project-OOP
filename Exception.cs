using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cédric_Vindevogel___Project_OOP
{
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

    public class GeenInvoerException : ApplicationException
    {
        public GeenInvoerException()
            : base("Geef eerst iets in in de textbox.")
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
