// Main.cs

using System;
using System.Windows.Forms;

public class App
{
	public static void Main()
	{	
		try
		{
            Jpt.Start("jpt.ini");      // do this once on app start
            Jpt.Use2("1234567890", @"C:\Users\SSP\Downloads\Debug\fotoHector.jpg", @"C:\Users\SSP\Downloads\Debug\foto.jpg");                 // do this for each document
            Jpt.End();                 // do this once on app end

        }

        catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "JPT Error",MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
	}
}
