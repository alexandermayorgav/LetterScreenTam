// Jpt.cs

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class Jpt
{
    const int OK = 0;

    [DllImport("jpt.dll")] private static extern int ReadIni(String inifilename);
    [DllImport("jpt.dll")] private static extern unsafe char* JptErrorW(int code);
    [DllImport("jpt.dll")] private static extern int SetField(String name, String value);
    [DllImport("jpt.dll")] private static extern int Code2File(String photo, String data, String dest);
    [DllImport("jpt.dll")] private static extern int CleanUp();


    private static String Error(int ret)
        {
        String r;
        unsafe { r = new String(JptErrorW(ret)); }
        return r;
        }

    public static void Start(String inifilename)
    {
        int ret = Jpt.OK;
        try
            {
                ret = Jpt.ReadIni(inifilename);
            }
        catch (Exception DllNotFoundException)
            {
            throw new Exception("Could not load JPT.DLL\nHardware key missing or software not installed properly.");
            }
        if (ret != Jpt.OK)
            {
            Jpt.CleanUp();
            throw new Exception(Jpt.Error(ret));
            }
    }

    public static void Use()
    {
        int ret = Jpt.OK;
        
        Jpt.SetField("documentno", "123456789");
        Jpt.SetField("surname", "MAYORGA VICENCIO");
        Jpt.SetField("givenname", "ALEXANDER");
        Jpt.SetField("birthdate", "870108");
        Jpt.SetField("birthplace", "MEXICO");
        Jpt.SetField("dateexpiry", "201220");
        Jpt.SetField("sex", "F");
        Jpt.SetField("ocr", "P<JURSINGER<<VICTORIA<<<<<<<<<<<<<<<<<<<<<<<\r\nJU155483<3JUR8709136F1602113<<<<<<<<<<<<<<00");

        string imagen = "foto2";
        ret = Jpt.Code2File(imagen +".jpg", "", imagen + "       .BMP");
        if (ret != Jpt.OK)
            MessageBox.Show(Jpt.Error(ret), "JPT Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
    }

    public static void End()
    {
        Jpt.CleanUp();
    }

}
