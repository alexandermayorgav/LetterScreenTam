using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using LetterScreen;

using System.Collections.Generic;
using LetterScreen.Modelo.bases;
using System.Linq;
using System.Configuration;

namespace LetterScreen
{

    public partial class frmEstatus : Form
    {
        private readonly String ruta = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
        delegate void SetTextCallback(string text);
        //private static readonly String ruta = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
        private PictureBox pbImagen;
        public  String getRuta()
        {
            return ruta.Substring(6);
        }

        private clsMicrotexto objMicrotexto;
        
        public frmEstatus()
        {
            InitializeComponent();
          //  initTCP();
           iniciarServer();
            objMicrotexto = new clsMicrotexto();
            if (!login(ConfigurationManager.AppSettings["userWSLicencias"], ConfigurationManager.AppSettings["passWSLicencias"]))
            {
                MessageBox.Show("Error al conectarse al servidor. Revise su conexión", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                Application.Exit();

            }
        }

       
        private bool uploadImagen(clsMicrotexto objMicrotexto)
        {
            ConsumeWS objWS = new ConsumeWS();
            clsArchivo objArchivo = new clsArchivo();
            objArchivo.IdDocumento = 19;
            objArchivo.IdPersona = objMicrotexto.idPersona;
            //objArchivo.StrImagen = getBase64StringByImagen(Image.FromFile("micro_" + objMicrotexto.idPersona + ".BMP"));
            objArchivo.StrImagen = getBase64StringByImagen(Image.FromFile(objMicrotexto.rutaLocalFoto));
            return objWS.uploadArchivos(objArchivo);
        }


        public static byte[] imageToByteArray(Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }

        public static String getBase64StringByImagen(Image imageIn)
        {
            return Convert.ToBase64String(imageToByteArray(imageIn));
        }



        private void iniciarServer()
        {
            Jpt.Start("jpt.ini");
            setTexto("Servidor iniciado");
            //setTexto(Jura.Jura.getCountLicence() + " Licencias Disponibles, expira: " + fecha.Substring(0,4) + "-"+ fecha.Substring(4,2) + "-" + fecha.Substring(6,2));
        }


        private void setTexto(String texto)
        {
            if (this.txtRegistro.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(setTexto);
                this.Invoke(d, new object[] { texto });
            }
            else
            {
                string dato = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss ") + ".- " + texto + System.Environment.NewLine + txtRegistro.Text;
                txtRegistro.Text = dato;
            }
            
            
        }

        private void buscarSolicitudes()
        {
            timer1.Stop();
            try
            {
                foreach (clsMicrotexto item in objMicrotexto.getSolicitudes())
                {
                    setTexto("Procesando solicitud idPersona = " + item.idPersona);
                    pbImagen = new PictureBox();
                    pbImagen.Load(ConfigurationManager.AppSettings["SistemaURL"] + "html/" + item.URLfoto);
                    string nombreFoto = "temp_" + item.idMicrotexto + ".jpg";
                    pbImagen.Image.Save(nombreFoto);
                    item.rutaLocalFoto = nombreFoto;

                    if (Jpt.Use(item.numeroLicencia, item.apellidos, item.nombre, item.fechaNacimiento, item.nacionalidad, item.fechaVigencia, item.sexo, item.MRZ, item.rutaLocalFoto, item.idPersona) == 0)
                    {
                        if (uploadImagen(item))
                        {
                            item.Actualizar();
                            setTexto("Solicitud procesada correctamente");
                        }
                        else
                            setTexto("Error al cargar el archivo.");
                    }
                }
            }
            catch (Exception ex)
            {

                setTexto(ex.Message + " " + ex.StackTrace );
            }
            timer1.Start();
        }

        



        private void frmEstatus_Load(object sender, EventArgs e)
        {
          //  initTCP();
            deleteFiles();
            timer1.Interval = 5000;
            timer1.Start();
        }

      
        private Image getImagen(String stringImagen)
        {
            MemoryStream ms = new MemoryStream(Convert.FromBase64String(stringImagen));
            Image returnImage = Image.FromStream(ms);
            return returnImage;


        }

        private void deleteFiles()
        {
            DirectoryInfo di = new DirectoryInfo(getRuta());
            FileInfo[] files = di.GetFiles("*.jpg")
                                 .Where(p => p.Extension == ".jpg").ToArray();
            foreach (FileInfo file in files)
                try
                {
                    file.Attributes = FileAttributes.Normal;
                    File.Delete(file.FullName);
                }
                catch { }

        }

        private void frmEstatus_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Si la aplicación se cierra las recaudaciones no podrán generar licencias. ¿Desea continuar con el cierre?", "AVISO", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No)
            {
                e.Cancel = true;

                return;
            }
            if (MessageBox.Show("La aplicación se cerrará y no se podrán generar licencias.", "AVISO", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Cancel)
                e.Cancel = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            buscarSolicitudes();
        }

        private bool login(string user, string pass)
        {

            try
            {
                Usuario objUsuario = new Usuario() { user = user, pass = pass, token = string.Empty };
                ConsumeWS objWS = new ConsumeWS();
                objUsuario.token = objWS.login(objUsuario);
                if (objUsuario.token.Length > 0)
                {
                    Sesion.objUsuario = objUsuario;
                        return true;
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectarse al servidor " + ConfigurationManager.AppSettings["SistemaURL"] + " verifique su conexión", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                // throw ex;
            }
            return false;
        }
    }
}
