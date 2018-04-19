using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetterScreen.Modelo.bases
{
    public    class clsMicrotexto
    {
        public int idMicrotexto { get; set; }
        public int idPersona { get; set; }
        public string estatus { get; set; }
        public DateTime fechaRegistro { get; set; }
        public string ip { get; set; }
        public int idTurno { get; set; }
        public string numeroLicencia { get; set; }
        public string nombre { get; set; }
        public string apellidos { get; set; }
        public DateTime fechaExpedicion { get; set; }
        public DateTime fechaNacimiento { get; set; }
        public string nacionalidad { get; set; }
        public DateTime fechaVigencia { get; set; }
        public string sexo { get; set; }
        public string MRZ { get; set; }

        private ConsumeWS objWS;

        public clsMicrotexto()
        {
            objWS = new ConsumeWS();
        }

        public List<clsMicrotexto> getSolicitudes()
        {
            objWS.ejecutarConsulta("SELECT * from microtexto WHERE estatus = 'espera' order by fechaRegistro ASC LIMIT 1");
            List<clsMicrotexto> lstSolicitudes = new System.Collections.Generic.List<clsMicrotexto>();
            String[] arr;
            foreach (var item in objWS.getListaDatos()[0].recordset)
            {
                arr = this.objWS.getDatoLimpio(item.ToString());
                lstSolicitudes.Add(
                    new clsMicrotexto()
                    {
                        idMicrotexto = Convert.ToInt32(arr[this.objWS.getListaDatos()[0].nameFields.IndexOf("idmicrotexto")]),
                        numeroLicencia = arr[this.objWS.getListaDatos()[0].nameFields.IndexOf("numeroLicencia")],
                        nombre = arr[this.objWS.getListaDatos()[0].nameFields.IndexOf("nombre")],
                        apellidos = arr[this.objWS.getListaDatos()[0].nameFields.IndexOf("apellidos")],
                        fechaExpedicion = Convert.ToDateTime(arr[this.objWS.getListaDatos()[0].nameFields.IndexOf("fechaExpedicion")]),
                        fechaNacimiento = Convert.ToDateTime(arr[this.objWS.getListaDatos()[0].nameFields.IndexOf("fechaNacimiento")]),
                        nacionalidad = arr[this.objWS.getListaDatos()[0].nameFields.IndexOf("nacionalidad")],
                         fechaVigencia = Convert.ToDateTime (arr[this.objWS.getListaDatos()[0].nameFields.IndexOf("fechaVigencia")]),
                         sexo = arr[this.objWS.getListaDatos()[0].nameFields.IndexOf("sexo")],
                         MRZ = arr[this.objWS.getListaDatos()[0].nameFields.IndexOf("MRZ")]

                        // tipoImagen = (clsDocumentoLicencia.TipoImagen)Enum.Parse(typeof(clsDocumentoLicencia.TipoImagen), arr[0].TrimEnd()),
                        //URL = arr[1].TrimStart().TrimEnd(),

                    }
                    );
            }


            return lstSolicitudes;

        }

    }
}
