// Main.cs

using System;
using System.Windows.Forms;

public class App
{
	public static void Main()
	{	
		try
		{
            //Jpt.Start("jpt.ini");      // do this once on app start
            //     	Jpt.Use();                 // do this for each document
            //Jpt.End();                 // do this once on app end
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmEstatus());
        }

		catch(Exception ex)
		{
			MessageBox.Show(ex.Message, "JPT Error",MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
	}
}
