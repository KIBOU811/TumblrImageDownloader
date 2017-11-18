using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace TumblrImageDownloader
{
    class WebClass
    {
        private readonly string _id;

        public WebClass(string id)
        {
            _id = id;
        }

        private HttpStatusCode GetStatusCode(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse res = null;
            HttpStatusCode statusCode;

            try
            {
                res = (HttpWebResponse)req.GetResponse();
                statusCode = res.StatusCode;
            }
            catch (WebException ex)
            {
                res = (HttpWebResponse)ex.Response;

                if (res != null)
                {
                    statusCode = res.StatusCode;
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                res?.Close();
            }
            return statusCode;
        }

        public bool IsWebPageExist(string url)
        {
            int code = (int) GetStatusCode(url);
            if (code >= 400)
            {
                return false;
            }
            return true;
        }

        private XmlDocument GetXml(string url)
        {
            WebRequest req = WebRequest.Create(url);
            WebResponse res = req.GetResponse();

            Stream st = res.GetResponseStream();
            StreamReader sr = new StreamReader(st, Encoding.UTF8);
            string xml = sr.ReadToEnd();
            st.Close();
            sr.Close();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            return doc;
        }

        public int GetPostCount(string url)
        {
            XmlDocument doc = GetXml(url);

            XmlNodeList nodes = doc.GetElementsByTagName("posts");
            for (int i = 0; i < nodes.Count; i++)
            {
                XmlElement elm = nodes[i] as XmlElement;
                if (elm == null) continue;
                string value = elm.GetAttribute("total");
                int count;
                if (int.TryParse(value, out count)) return count;
            }
            return 0;
        }

        private void GetPicture(string url)
        {
            string folderPath = AppDomain.CurrentDomain.BaseDirectory + _id;
            Directory.CreateDirectory(folderPath);
            string filePath = $@"{folderPath}\{url.Split('/')[4]}";

            if (File.Exists(filePath))
            {
                Console.WriteLine($"{url.Split('/')[4]}は存在するためスキップします");
                return;
            }
            Console.WriteLine($"{url.Split('/')[4]}をダウンロード中");
            WebClient wc = new WebClient();
            wc.DownloadFile(url, filePath);
            Console.WriteLine($"{url.Split('/')[4]}をダウンロードしました");
        }

        private void CollectPictureUrl(XmlDocument doc, string tag, string attr)
        {
            XmlNodeList nodes = doc.GetElementsByTagName(tag);
            for (int i = 0; i < nodes.Count; i++)
            {
                XmlElement elm = nodes[i] as XmlElement;
                if (elm == null) continue;
                string value = elm.GetAttribute(attr);
                if (value == "1280") GetPicture(nodes[i].InnerText);
            }
        }

        public void SearchPicture(string url)
        {
            XmlDocument doc = GetXml(url);

            CollectPictureUrl(doc, "photo-url", "max-width");
        }
    }
}