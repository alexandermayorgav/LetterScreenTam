using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LetterScreen
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmEstatus());
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message + " " + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }
    }
}
