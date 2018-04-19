using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetterScreen.Modelo.bases
{
    public class clsDocumentoLicencia
    {
        public enum TipoImagen { Fotografia, Firma, Biometrico, Microtexto, MedidaSeguridad }
        public TipoImagen tipoImagen { get; set; }
        public string archivo { get; set; }
        public string URL { get; set; }
        private ConsumeWS objWS;

        public clsDocumentoLicencia()
        {
            objWS = new ConsumeWS();
        }

        

        public List<clsDocumentoLicencia> getDocumentosMedidaSeguridad(int idPersona)
        {
            objWS.ejecutarConsulta("SELECT D.descripcion as Documento, PD.documentoimagen FROM `persona_documento` PD INNER JOIN documento D ON PD.iddocumento=D.iddocumento WHERE PD.idPersona= " + idPersona.ToString() + " AND PD.estatus='vigente' AND D.idDocumento in(19) ORDER BY fechacaptura DESC LIMIT 1");
            List<clsDocumentoLicencia> lstDocumentos = new System.Collections.Generic.List<clsDocumentoLicencia>();
            String[] arr;
            foreach (var item in objWS.getListaDatos()[0].recordset)
            {
                arr = item.ToString().Replace('\n', ' ').Trim().Replace('\r', ' ').Trim().Replace('\"', ' ').Trim().Replace('[', ' ').Trim().Replace(']', ' ').Trim().Replace('\t', ' ').Trim().Split(',');
                lstDocumentos.Add(
                    new clsDocumentoLicencia()
                    {
                        tipoImagen = (clsDocumentoLicencia.TipoImagen)Enum.Parse(typeof(clsDocumentoLicencia.TipoImagen), arr[0].TrimEnd()),
                        URL = arr[1].TrimStart().TrimEnd(),

                    }
                    );
            }

           
            return lstDocumentos;
        }
    }
}
