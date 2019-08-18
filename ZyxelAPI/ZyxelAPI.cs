using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Threading.Tasks;

namespace Zyxel.ZyxelAPI
{
    class ZyxelAPI
    {
        public const string GetConnections = "<packet ref=\" / \"><request id=\"1\" ref=\"former.formConnections[load]\"><command name=\"show system\"></command></request><request id=\"2\" ref=\"former.formConnections[load]\"><command name=\"show ip dhcp bindings\"><pool>_WEBADMIN</pool></command></request><request id=\"3\" ref=\"former.formConnections[load]\"><command name=\"show interface\"></command></request><request id=\"4\" ref=\"former.formConnections[load]\"><command name=\"show ip nat\"></command></request><request id=\"5\" ref=\"former.formConnections[load]\"><config name=\"known host\"></config></request><request id=\"6\" ref=\"former.formConnections[load]\"><command name=\"show ip arp\"><alive>alive</alive></command></request></packet>";
        public const string DefaultURL = "ci";
        public const string Protocol = "http://";
        public string URL
        {
            get;
            set;
        }
        public Uri URI
        {
            get
            {
                return new Uri(URL);
            }
        }

        public string Login
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public ZyxelAPI(string URI, string login, string password)
        {
            URL = URI;
            Login = login;
            Password = password;
        }


        public XmlNode GetConnection()
        {
            string resp = PostRequest(DefaultURL, GetConnections);
            var xml = new XmlDocument();
            xml.PreserveWhitespace = false;
            xml.LoadXml(resp);
            return xml.SelectSingleNode("/packet/response[@id='4']");
        }

        private string connect(string path)
        {
            WebRequest rq = WebRequest.Create(Protocol + URL + "//" + path);
            WebResponse res = rq.GetResponse();
            using (Stream stream = res.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
       
        private string PostRequest(string path, string data)
        {
            WebRequest request = WebRequest.Create(Protocol + URL + "/" + path);
            request.Method = "POST"; // для отправки используется метод Post

            request.PreAuthenticate = true;

            //request.Credentials = new NetworkCredential("admin", "Mart2005");

            if (Login != "" & Password != "")
            {
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)request;
                NetworkCredential myNetworkCredential = new NetworkCredential(Login, Password);
                CredentialCache myCredentialCache = new CredentialCache();
                myCredentialCache.Add(new Uri(Protocol + URL + "/" + path), "Digest", myNetworkCredential);
                myHttpWebRequest.PreAuthenticate = true;
                myHttpWebRequest.Credentials = myCredentialCache;
            }
            // преобразуем данные в массив байтов
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(data);
            // устанавливаем тип содержимого - параметр ContentType
            request.ContentType = "application/xml";
            // Устанавливаем заголовок Content-Length запроса - свойство ContentLength
            request.ContentLength = byteArray.Length;

            //записываем данные в поток запроса
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            WebResponse response = request.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return (reader.ReadToEnd());
                }
            }
        }
    }
}
