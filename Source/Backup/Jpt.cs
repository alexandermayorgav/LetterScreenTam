// Jpt.cs

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class Jpt
{
    const int OK = 0;

    [DllImport("Jpt.dll")] private static extern int ReadIni(String inifilename);
    [DllImport("Jpt.dll")] private static extern unsafe char* JptErrorW(int code);
    [DllImport("Jpt.dll")] private static extern int SetField(String name, String value);
    [DllImport("Jpt.dll")] private static extern int Code2File(String photo, String data, String dest);
    [DllImport("Jpt.dll")] private static extern int CleanUp();


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
        
        Jpt.SetField("documentno", "JU155483");
        Jpt.SetField("surname", "SINGER");
        Jpt.SetField("givenname", "VICTORIA");
        Jpt.SetField("birthdate", "791203");
        Jpt.SetField("birthplace", "Jurio");
        Jpt.SetField("dateexpiry", "170502");
        Jpt.SetField("sex", "F");
        Jpt.SetField("ocr", "P<JURSINGER<<VICTORIA<<<<<<<<<<<<<<<<<<<<<<<\r\nJU155483<3JUR8709136F1602113<<<<<<<<<<<<<<00");

        ret = Jpt.Code2File("http://motd.mx/Licencias2/html/images/persona/chavo.jpg", "", "LS.BMP");
        if (ret != Jpt.OK)
            MessageBox.Show(Jpt.Error(ret), "JPT Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
    }

    public static void End()
    {
        Jpt.CleanUp();
    }

}
