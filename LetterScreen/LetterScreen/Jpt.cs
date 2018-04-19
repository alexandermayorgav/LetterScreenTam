// Jpt.cs

using LetterScreen.Modelo.bases;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class Jpt
{
    const int OK = 0;

    [DllImport("Jpt.dll")] private static extern int ReadIni(String inifilename);
    [DllImport("Jpt.dll")] private static extern unsafe char* JptErrorW();
    [DllImport("Jpt.dll")] private static extern int SetField(String name, String value);
    [DllImport("Jpt.dll")] private static extern int Code2File(String photo, String data, String dest);
    [DllImport("Jpt.dll")] private static extern int CleanUp();


    private static String Error(int ret)
        {
        String r;
        unsafe { r = new String(JptErrorW()); }
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

    public static void Use( clsMicrotexto objMicrotexto)
    {
        int ret = Jpt.OK;
        
        Jpt.SetField("documentno", objMicrotexto.numeroLicencia);
        Jpt.SetField("surname",  objMicrotexto.apellidos);
        Jpt.SetField("givenname", objMicrotexto.nombre);
        Jpt.SetField("birthdate", objMicrotexto.fechaNacimiento.ToString("yyMMdd"));
        Jpt.SetField("birthplace", objMicrotexto.nacionalidad);
        Jpt.SetField("dateexpiry", objMicrotexto.fechaVigencia.ToString("yyMMdd"));
        Jpt.SetField("sex", objMicrotexto.sexo);
        Jpt.SetField("ocr", objMicrotexto.MRZ);

        ret = Jpt.Code2File("Viki.jpg", "", "LS.BMP");
        if (ret != Jpt.OK)
            MessageBox.Show(Jpt.Error(ret), "JPT Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
    }

    public static void End()
    {
        Jpt.CleanUp();
    }

}
