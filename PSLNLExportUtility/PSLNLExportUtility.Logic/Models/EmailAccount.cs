using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLNLExportUtility.Logic.Models
{
    public class EmailAccount
    {
        public string Server { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public int Port { get; set; }
    }
}
