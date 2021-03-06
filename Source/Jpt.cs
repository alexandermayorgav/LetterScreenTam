// Jpt.cs

using System;
using System.IO;
using System.Net;
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

   
    public static String Error(int ret)
        {
        String r;
        unsafe { r = new String(JptErrorW(ret)); }
        return r;
        }

    public static int Start(String inifilename)
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
        return ret;
    }

    public static int Use(string numero, string apellidos, string nombre, DateTime fechaNacimiento, string nacionalidad, DateTime fechaVigencia, string sexo, string MRZ,string rutaFoto, int idPersona, string archivoSalida)
    {
        int ret = Jpt.OK;
        
        Jpt.SetField("documentno",numero);
        Jpt.SetField("surname", apellidos);
        Jpt.SetField("givenname", nombre);
        Jpt.SetField("birthdate", fechaNacimiento.ToString("yyMMdd"));
        Jpt.SetField("birthplace", nacionalidad);
        Jpt.SetField("dateexpiry",  fechaVigencia.ToString("yyMMdd"));
        Jpt.SetField("sex", sexo);
        Jpt.SetField("ocr", MRZ);
        
        ret = Jpt.Code2File(rutaFoto, "", archivoSalida);
        return ret;
        //if (ret != Jpt.OK)
        //    MessageBox.Show(Jpt.Error(ret), "JPT Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
    }

    public static int Use2(string numero,string rutaFoto, string archivoSalida)
    {
        int ret = Jpt.OK;

        Jpt.SetField("documentno", numero);
  
        ret = Jpt.Code2File(rutaFoto, "", archivoSalida);
       if (ret != Jpt.OK)
            MessageBox.Show(Jpt.Error(ret), "JPT Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        return ret;
        
    }



    public static void End()
    {
        Jpt.CleanUp();
    }

}
