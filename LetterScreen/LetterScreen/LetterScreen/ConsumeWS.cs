using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Web;
using LetterScreen.Modelo.bases;


namespace LetterScreen
{
    public class ConsumeWS
    {
        private Request objRequest;
        private String url;
        private Response objResponse;
        private List<DatosWS> lstDatos;

        public ConsumeWS()
        {
            this.objRequest = new Request();
            this.objResponse = new Response();
            getConfiguracion();
            //login();
        }

        private void getConfiguracion()
        {
            this.url = System.Configuration.ConfigurationManager.AppSettings["SistemaURL"] + System.Configuration.ConfigurationManager.AppSettings["webService"];
        }

        public List<DatosWS> getListaDatos()
        {
            return this.lstDatos;
        }
        /// <summary>
        /// Elimina caracteres especiales de los datos de la consulta
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public String[] getDatoLimpio(string item)
        {
            String[] arr;
            arr = item.ToString().Replace('\n', ' ').Trim().Replace('\r', ' ').Trim().Replace('\"', ' ').Trim().Replace('[', ' ').Trim().Replace(']', ' ').Trim().Replace('\t', ' ').Trim().Split(',');
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = arr[i].TrimEnd().TrimStart();
            }
            return arr;
        }

        /// <summary>
        /// metodo de login de usuarios y retorna el token de la sesion que sera utilizado en las consultas posteriores a BD
        /// </summary>
        /// <param name="objUsuario"></param>
        /// <returns></returns>
        public String login(Usuario objUsuario)
        {
            string jsonUser = JsonConvert.SerializeObject(objUsuario);

            objRequest.cmd = "loginOld";
            objRequest.log = "login";
            objRequest.origin = Environment.MachineName;
            objRequest.token = "";
            objRequest.param = jsonUser;

            string jsonRequest = JsonConvert.SerializeObject(objRequest);

            String mensaje = HttpPostRequest(url, JsonToDictionary(jsonRequest, "request"));
            if(!System.IO.Directory.Exists(Application.StartupPath + "\\log"))
                System.IO.Directory.CreateDirectory(Application.StartupPath + "\\log");
            System.IO.File.WriteAllText(Application.StartupPath + "\\log\\" + objRequest.cmd + "-" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + ".txt", mensaje);
            if (mensaje.Substring(0) != "{" && mensaje.Substring(mensaje.Length - 1) != "}" || mensaje.Contains("NOK"))
            {
                throw new Exception("Respuesta WS: " + mensaje);
            }
            this.objResponse = JsonConvert.DeserializeObject<Response>(mensaje);
            if (this.objResponse.result == "OK")
            {
                clsToken objToken = (clsToken)JsonConvert.DeserializeObject(objResponse.data, typeof(clsToken));
                //String token = objResponse.data.Split(':')[1].Substring(1);
                //token = token.Substring(0, token.Length - 2);
                return objToken.token;
            }
            return "";
        }

        public void getDatosUsuario(ref Usuario objUsuario)
        {
            //ejecutarConsulta("SELECT mundo");
            ejecutarConsulta("SELECT id_login, id_recaudacion,id_rol  FROM login_user WHERE user_name = '" + objUsuario.user + "' LIMIT 1");
            String[] arr = null;
            //Usuario objUser = new Usuario();
            foreach (var item in this.lstDatos[0].recordset)
            {
                arr = item.ToString().Replace('\n', ' ').Trim().Replace('\r', ' ').Trim().Replace('\"', ' ').Trim().Replace('[', ' ').Trim().Replace(']', ' ').Trim().Replace('\t', ' ').Trim().Split(',');

                objUsuario.idUsuario = Convert.ToInt32(arr[this.lstDatos[0].nameFields.IndexOf("id_login")]);
                objUsuario.idRecaudacion = Convert.ToInt32(arr[this.lstDatos[0].nameFields.IndexOf("id_recaudacion")]);
                objUsuario.idRol= Convert.ToInt32(arr[this.lstDatos[0].nameFields.IndexOf("id_rol")]);
            }
          //  return objUsuario;
        }

        public string getRutaMicrotexto(int idpersona)
        {
            //ejecutarConsulta("SELECT mundo");
            ejecutarConsulta(" select documentoimagen, fechacaptura  from persona_documento where iddocumento = 19 and estatus = 'vigente' and idpersona = " + idpersona + " order by fechacaptura desc limit 1");
            String[] arr = null;
            //Usuario objUser = new Usuario();
            foreach (var item in this.lstDatos[0].recordset)
            {
                arr = item.ToString().Replace('\n', ' ').Trim().Replace('\r', ' ').Trim().Replace('\"', ' ').Trim().Replace('[', ' ').Trim().Replace(']', ' ').Trim().Replace('\t', ' ').Trim().Split(',');

                return arr[this.lstDatos[0].nameFields.IndexOf("documentoimagen")];
            }
            return string.Empty;
            //  return objUsuario;
        }


        public void updatePersonaDocumentoMicrotexto(int idpersona)
        {
            //ejecutarConsulta("SELECT mundo");
            ejecutarConsulta(" update persona_documento set estatus = 'baja' where iddocumento = 19 and estatus = 'vigente' and idpersona = " + idpersona +" order by fechacaptura desc limit 1");
           
        }




        public bool uploadArchivos(clsArchivo objArchivo)
        {
            try
            {
                this.objRequest.cmd = "microtexto";
                this.objRequest.log = "upload";
                this.objRequest.origin = Environment.MachineName;
                this.objRequest.token = Sesion.objUsuario.token;
                this.objRequest.param = JsonConvert.SerializeObject(objArchivo);
                string json = JsonConvert.SerializeObject(this.objRequest);
                string mensaje = HttpPostRequest(url, JsonToDictionary(json, "request"));
                this.objResponse = JsonConvert.DeserializeObject<Response>(mensaje);
                string[] var = objResponse.data.Split(',');
                //if (objResponse.result == "OK" && objResponse.data.Split(':')[1].Substring(0, objResponse.data.Split(':')[1].Length - 1) == "true")
                string a = var[0].Split(':')[1].Substring(0, var[0].Split(':')[1].Length);
                string b = var[1].Split(':')[1].Substring(0, var[1].Split(':')[1].Length -1);
                if (objResponse.result == "OK" && a == "true" && b=="true")
                {
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {

                throw new Exception("Error al subir la imagen de microtexto. ",ex);
            }

        }

        /// <summary>
        /// Metodo para ejecutar consultas SQL via WebService
        /// </summary>
        /// <param name="query"></param>
        public void ejecutarConsulta(String query)
        {
            string mensaje;
            try
            {
                if (Sesion.objUsuario.token == null)
                { throw new Exception("Token incorrecto."); }
                this.objRequest.cmd = "query";
                this.objRequest.log = "query";
                this.objRequest.origin = Environment.MachineName;
                this.objRequest.token = Sesion.objUsuario.token;
                Query objQuery = new Query();
                objQuery.add(query);
                this.objRequest.param = JsonConvert.SerializeObject(objQuery);
                string json = JsonConvert.SerializeObject(objRequest);
                mensaje = HttpPostRequest(url, JsonToDictionary(json, "request"));
                if (mensaje.Substring(0)!= "{" && mensaje.Substring(mensaje.Length -1)!="}")
                {
                    throw new Exception("Respuesta WS: " + mensaje);
                }
                this.objResponse = JsonConvert.DeserializeObject<Response>(mensaje);
                if (objResponse.result!="OK")
                {
                    throw new Exception("Error el ejecutar consulta: " + objResponse.msg);
                }
                this.lstDatos = (List<DatosWS>)JsonConvert.DeserializeObject(objResponse.data, typeof(List<DatosWS>));
                if (lstDatos.Count!= 0)
                {
                    if (lstDatos[0].error.Length!=0)
                    {
                        throw new Exception("Error al ejecutar consulta: " + lstDatos[0].error);
                    }
                }
            }
            catch (System.Exception e)
            {
                //if (e.Message == "Unable to connect to the remote server")
                //{
                //    MessageBox.Show("Error al conectarse al servidor, revise su conexión.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //}
                //else

                //    MessageBox.Show(e.Message + "\n" + e.StackTrace, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new Exception("Error al ejecutar consulta.", e);
            }
        }

        public void ejecutarConsultaT(List<String> querys)
        {
            string mensaje;
            try
            {
                Sesion.objUsuario = new Usuario() {
                    token = "80f7d02d0ddcf8043428728c6b1e9875839624954be818ecd7fb38495c5f722abaaf82df3ed4dbf9969bca2a8c1f1fd44a6ffd85b94e14fede184e334a77476a" };
                if (Sesion.objUsuario.token == null)
                { throw new Exception("Token incorrecto."); }
                this.objRequest.cmd = "query";
                this.objRequest.log = "query";
                this.objRequest.origin = Environment.MachineName;
                this.objRequest.token = Sesion.objUsuario.token;
                Query objQuery = new Query();
                string queryCompuesto = "START TRANSACTION; ";
                foreach (string query in querys)
                {
                    //objQuery.add(query);
                    queryCompuesto += query;
                }
                queryCompuesto += " COMMIT;";
                objRequest.param = JsonConvert.SerializeObject(objQuery);

                string json = JsonConvert.SerializeObject(objRequest);
                mensaje = HttpPostRequest(url, JsonToDictionary(json, "request"));
                if (mensaje.Substring(0) != "{" && mensaje.Substring(mensaje.Length - 1) != "}")
                {
                    throw new Exception("Respuesta WS: " + mensaje);
                }
                this.objResponse = JsonConvert.DeserializeObject<Response>(mensaje);
                this.lstDatos = (List<DatosWS>)JsonConvert.DeserializeObject(objResponse.data, typeof(List<DatosWS>));
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }


        private string HttpPostRequest(string url, Dictionary<string, string> postParameters)
        {
            string postData = "";
            foreach (string key in postParameters.Keys)
            {
                postData += HttpUtility.UrlEncode(key) + "="
                      + HttpUtility.UrlEncode(postParameters[key]) + "&";
            }
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            myHttpWebRequest.Method = "POST";
            byte[] data = Encoding.ASCII.GetBytes(postData);
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            myHttpWebRequest.ContentLength = data.Length;
            Stream requestStream = myHttpWebRequest.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            Stream responseStream = myHttpWebResponse.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);
            string pageContent = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            responseStream.Close();
            myHttpWebResponse.Close();
            return pageContent;
        }

        /// <summary>
        /// Genera un diccionario con los parametros
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Dictionary<string, string> JsonToDictionary(string request, string paramName)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add(paramName, request);
            return result;
        }
    }

    #region Clases
   
    public class DatosWS
    {
        public double time { get; set; }
        public Newtonsoft.Json.Linq.JArray recordset { get; set; }
        public int numRows { get; set; }
        public int numFields { get; set; }
        public int lastInsertId { get; set; }
        public string error { get; set; }
        public List<String> nameFields { get; set; }
    }
    public class Query
    {
        public List<String> querys { get; set; }
        public Query()
        {
            this.querys = new List<string>();
        }
        public void add(String query)
        {
            String strQ = query.Trim();
            strQ = strQ.Replace("SELECT", "XELECT");
            strQ = strQ.Replace("UPDATE", "XPDATE");
            this.querys.Add(strQ);
        }
    }
    public class Usuario
    {
        public String user { get; set; }
        public String pass { get; set; }
        public String token { get; set; }
        public int idUsuario { get; set; }
        public int idRecaudacion { get; set; }
        public int idRol { get; set; }
    }
    public class Response
    {
        public String result { get; set; }
        public String msg { get; set; }
        public String data { get; set; }
    }
    public class Request
    {
        public String cmd { get; set; }
        public String log { get; set; }
        public String token { get; set; }
        public String param { get; set; }
        public String origin { get; set; }
    }
    public class clsToken
    {
        public string token { get; set; }
    }

    public class clsBarcode2D {
        //num_rows,num_cols,bcode,code
        public String num_rows { get; set; }
        public String num_cols { get; set; }
        public String[] bcode { get; set; }
        public String code { get; set; }
    }
    #endregion
}
