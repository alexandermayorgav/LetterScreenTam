using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using LetterScreen;
using UseJptCS.Modelo.bases;

namespace UseJptCS
{

    public partial class frmEstatus : Form
    {
        private readonly String ruta = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
        private Socket listener;
        byte[] buffer;
        delegate void SetTextCallback(string text);
        private List
        
        public frmEstatus()
        {
            InitializeComponent();
          //  initTCP();
           iniciarServer();
        }

        private string getStr64Imagen(String rutaImagen)
        {
            string base64String;
            using (Image image = Image.FromFile(rutaImagen))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                     base64String = Convert.ToBase64String(imageBytes);
                    
                }
            }
            return base64String;
        }

        private string getImagenSinFondo(String urlImagen)
        {
            string ruta = getImageFromURL(urlImagen);
            Bitmap imagen = new Bitmap(Image.FromFile(ruta));
            imagen.MakeTransparent(Color.White);
            string rutaFinal = ruta.Substring(0, ruta.Length - 3) + "jpg";
            imagen.Save(rutaFinal);
            return rutaFinal;
        }
        private bool getMedidaSeguridadByIdTurno(Int64 idTurno, String strImagen)
        {
            objTurno = new Turno();
            objTurno.IdTurno = idTurno;
            
            string medidaSeguridadImagen = getRuta() + "\\MedidaSeguridad_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".jpg";
            //int ret = Jura.Jura.getImage(getImageFromURL(strImagen), objTurno.persona.nombres.ToUpper(), objTurno.persona.primerAp.ToUpper().TrimStart() + " " + objTurno.persona.segundoAp.ToUpper().TrimStart(), objTurno.licencia.numero.TrimStart(), medidaSeguridadImagen);
         //   int ret = Jura.Jura.getImage(getImagenSinFondo(strImagen), "", "", objTurno.licencia.numero.TrimStart(), medidaSeguridadImagen);
            int ret = Jura.Jura.getImage(getImageFromURL(strImagen), "", "", objTurno.licencia.numero.TrimStart(), medidaSeguridadImagen);
            if (ret != 0)
            {

                setTexto("Error al generar la medida de seguridad: " + Jura.Jura.getErrorString(ret));
                return false;
            }
            
            return uploadImagen(medidaSeguridadImagen); ;

        }

        private bool uploadImagen(String rutaImagen)
        {
            string base64 = getStr64Imagen(rutaImagen);
            string[] nombre = rutaImagen.Split('\\');

            ConsumeWS objWS = new ConsumeWS();
            clsArchivo objArchivo = new clsArchivo();
            objArchivo.IdDocumento = 19;
            objArchivo.IdPersona = 
            

            return objWS.uploadArchivos();
        }

        /// <summary>
        /// obtiene una copia local de una imagen a partir de una URL
        /// </summary>
        /// <param name="ruta"></param>
        /// <returns></returns>
        public string getImageFromURL(String archivo)
        {
            string ruta = archivo;
            ruta = ruta.Trim();
            if (File.Exists(ruta))
                return ruta;


            string url = ruta;
            string[] arrNombre = ruta.Split('/');
            string nombre = arrNombre[arrNombre.Length - 1];
            string rutaLocal = this.getRuta() + "\\"  + nombre ;
            rutaLocal = rutaLocal.TrimEnd().TrimStart();
            using (WebClient client = new WebClient())
            {
                //client.DownloadFileAsync(new Uri(url), rutaLocal);
                client.DownloadFile(new Uri(url),rutaLocal);
            }
            //return rutaLocal.Replace('\\', '/');
            return rutaLocal;
        }

        public String getRuta()
        {
            return ruta.Substring(6);
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

        private void frmEstatus_Load(object sender, EventArgs e)
        {
          //  initTCP();
            deleteFiles();
        }

      
        private Image getImagen(String stringImagen)
        {
            MemoryStream ms = new MemoryStream(Convert.FromBase64String(stringImagen));
            Image returnImage = Image.FromStream(ms);
            return returnImage;


        }

        private void deleteFiles()
        {
            //DirectoryInfo di = new DirectoryInfo(getRuta());
            //FileInfo[] files = di.GetFiles("*.jpg")
            //                     .Where(p => p.Extension == ".jpg").ToArray();
            //foreach (FileInfo file in files)
            //    try
            //    {
            //        file.Attributes = FileAttributes.Normal;
            //        File.Delete(file.FullName);
            //    }
            //    catch { }
            
        }

        private void frmEstatus_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Si la aplicación se cierra las recaudaciones no podrán generar licencias. ¿Desea continuar con el cierre?", "AVISO",MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2  ) == System.Windows.Forms.DialogResult.No)
                e.Cancel = true;
            if (MessageBox.Show("La aplicación se cerrará y no se podrán generar licencias.", "AVISO", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Cancel)
                e.Cancel = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }
    }
}
