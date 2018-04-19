using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo
{
    public class clsBase
    {
        public string Error { get; set; }
        public string SystemError { get; set; }
        public bool bError { get; set; }

        public bool setError(string error, string systemError)
        {
            this.Error = error;
            this.SystemError = systemError;
            this.bError = true;
            return false;
        }

        public string MD5(string original)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
            return string.Empty;
        }
    }
}
