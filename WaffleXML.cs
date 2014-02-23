using System;
using System.Linq;
using System.Text;
using System.Xml;

namespace xk3yDVDMenu
{
    public class WaffleXML
    {
        string _filename;
        public WaffleXML(string fileName)
        {
            _filename = fileName;
        }
        public string Title {
            get
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                var reader = new XmlTextReader(_filename);
                xml.Load(reader);
                reader.Close();
                var gameInfo = xml.SelectNodes("/gameinfo");
                return gameInfo[0]["title"].InnerText;
            }
            catch
            {
                
            }
            return "";
        } 
            set{
                XmlDocument xml = new XmlDocument();
                if (System.IO.File.Exists(_filename))
                {   var reader = new XmlTextReader(_filename);
                    xml.Load(reader);
                    reader.Close();
                }
                var gameInfo = xml.SelectNodes("/gameinfo");
                if (gameInfo.Count == 0)
                {
                    XmlDeclaration dec = xml.CreateXmlDeclaration("1.0","UTF8",null);
                                           xml.AppendChild(dec);
                                           xml.AppendChild(xml.CreateNode(XmlNodeType.Element, "gameinfo", ""));
                }
                gameInfo = xml.SelectNodes("/gameinfo");
                if (gameInfo[0]["title"] == null)
                {
                    gameInfo[0].AppendChild(xml.CreateNode(XmlNodeType.Element, "title", ""));
                }
                gameInfo = xml.SelectNodes("/gameinfo");
                gameInfo[0]["title"].InnerText = value;            
                var writter = new XmlTextWriter(_filename, Encoding.UTF8);
                xml.Save(writter);
                writter.Close();
                
            } 
        }
        public string Summary
        {
            get
            {
                try
                {
                    XmlDocument xml = new XmlDocument();
                    var reader = new XmlTextReader(_filename);
                    xml.Load(reader);
                    reader.Close();
                    var gameInfo = xml.SelectNodes("/gameinfo");
                    return gameInfo[0]["summary"].InnerText;
                }
                catch
                {

                }
                return "";
            }
            set
            {
                XmlDocument xml = new XmlDocument();
                if (System.IO.File.Exists(_filename))
                {
                    var reader = new XmlTextReader(_filename);
                    xml.Load(reader);
                    reader.Close();
                }
                var gameInfo = xml.SelectNodes("/gameinfo");
                if (gameInfo.Count == 0)
                {
                    XmlDeclaration dec = xml.CreateXmlDeclaration("1.0", "UTF8", null);
                    xml.AppendChild(dec);
                    xml.AppendChild(xml.CreateNode(XmlNodeType.Element, "gameinfo", ""));
                }
                gameInfo = xml.SelectNodes("/gameinfo");
                if (gameInfo[0]["summary"] == null)
                {
                    gameInfo[0].AppendChild(xml.CreateNode(XmlNodeType.Element, "summary", ""));
                }
                gameInfo = xml.SelectNodes("/gameinfo");
                gameInfo[0]["summary"].InnerText = value;
                var writter = new XmlTextWriter(_filename, Encoding.UTF8);
                xml.Save(writter);
                writter.Close();

            }
        }
        public string InfoItem(string key)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                var reader = new XmlTextReader(_filename);
                xml.Load(reader);
                reader.Close();
                var gameInfo = xml.SelectNodes("/gameinfo/info");
                var childnodes = gameInfo[0].ChildNodes;
                foreach (XmlElement node in childnodes)
                {
                    if (node.Name == "infoitem" && node.HasAttribute("name") && node.Attributes["name"].InnerText == key)
                        return node.InnerText;
                }
                
            }
            catch
            {

            }
            return "";
        }
        public void InfoItem(string key, string value)
        {

            XmlDocument xml = new XmlDocument();
            if (System.IO.File.Exists(_filename))
            {
                var reader = new XmlTextReader(_filename);
                xml.Load(reader);
                reader.Close();
            }
            var gameInfo = xml.SelectNodes("/gameinfo");
            if (gameInfo.Count == 0)
            {
                XmlDeclaration dec = xml.CreateXmlDeclaration("1.0", "UTF8", null);
                xml.AppendChild(dec);
                xml.AppendChild(xml.CreateNode(XmlNodeType.Element, "gameinfo", ""));
            }
            var gameInfoInfo = xml.SelectNodes("/gameinfo/info");
            if (gameInfoInfo.Count == 0)
            {


                gameInfo[0].AppendChild(xml.CreateNode(XmlNodeType.Element, "info", ""));
            }

            gameInfoInfo = xml.SelectNodes("/gameinfo/info");

            if (gameInfoInfo[0]["infoitem"] == null || (from XmlElement d in gameInfoInfo[0]["infoitem"] where d.Attributes["name"].InnerText == key select d.InnerText).Any() == false)
            {
                var newElement = xml.CreateElement("infoitem", "");
                var arr = xml.CreateAttribute("name");
                arr.InnerText = key;
                gameInfoInfo[0].AppendChild(newElement);
                newElement.Attributes.Append(arr);
            }
            gameInfoInfo = xml.SelectNodes("/gameinfo/info");
            try
            {
                var childnodes = gameInfoInfo[0].ChildNodes;
                foreach (XmlElement node in childnodes)
                {
                    if (node.Name == "infoitem" && node.HasAttribute("name") && node.Attributes["name"].InnerText == key )
                    node.InnerText = value;
                }

                var writter = new XmlTextWriter(_filename, Encoding.UTF8);
                xml.Save(writter);
                writter.Close();
            }
            catch (Exception)
            {
            }
        }

        public byte[] Banner
        {
            get
            {
                try
                {
                    XmlDocument xml = new XmlDocument();
                    var reader = new XmlTextReader(_filename);
                    xml.Load(reader);
                    reader.Close();
                    var gameInfo = xml.SelectNodes("/gameinfo");
                    return Convert.FromBase64String(gameInfo[0]["banner"].InnerText);
                }
                catch
                {

                }
                return null;
            }
            set
            {
                XmlDocument xml = new XmlDocument();
                if (System.IO.File.Exists(_filename))
                {
                    var reader = new XmlTextReader(_filename);
                    xml.Load(reader);
                    reader.Close();
                }
                var gameInfo = xml.SelectNodes("/gameinfo");
                if (gameInfo.Count == 0)
                {
                    XmlDeclaration dec = xml.CreateXmlDeclaration("1.0", "UTF8", null);
                    xml.AppendChild(dec);
                    xml.AppendChild(xml.CreateNode(XmlNodeType.Element, "gameinfo", ""));
                }
                gameInfo = xml.SelectNodes("/gameinfo");
                if (gameInfo[0]["banner"] == null)
                {
                    gameInfo[0].AppendChild(xml.CreateNode(XmlNodeType.Element, "banner", ""));
                }
                gameInfo = xml.SelectNodes("/gameinfo");
                gameInfo[0]["banner"].InnerText = Convert.ToBase64String(value);
                var writter = new XmlTextWriter(_filename, Encoding.UTF8);
                xml.Save(writter);
                writter.Close();

            }
        }
        public byte[] BoxArt
        {
            get
            {
                try
                {
                    XmlDocument xml = new XmlDocument();
                    var reader = new XmlTextReader(_filename);
                    xml.Load(reader);
                    reader.Close();
                    var gameInfo = xml.SelectNodes("/gameinfo");
                    return Convert.FromBase64String(gameInfo[0]["boxart"].InnerText);
                }
                catch
                {

                }
                return null;
            }
            set
            {
                XmlDocument xml = new XmlDocument();
                if (System.IO.File.Exists(_filename))
                {
                    var reader = new XmlTextReader(_filename);
                    xml.Load(reader);
                    reader.Close();
                }
                var gameInfo = xml.SelectNodes("/gameinfo");
                if (gameInfo.Count == 0)
                {
                    XmlDeclaration dec = xml.CreateXmlDeclaration("1.0", "UTF8", null);
                    xml.AppendChild(dec);
                    xml.AppendChild(xml.CreateNode(XmlNodeType.Element, "gameinfo", ""));
                }
                gameInfo = xml.SelectNodes("/gameinfo");
                if (gameInfo[0]["boxart"] == null)
                {
                    gameInfo[0].AppendChild(xml.CreateNode(XmlNodeType.Element, "boxart", ""));
                }
                gameInfo = xml.SelectNodes("/gameinfo");
                gameInfo[0]["boxart"].InnerText = Convert.ToBase64String(value);
                var writter = new XmlTextWriter(_filename, Encoding.UTF8);
                xml.Save(writter);
                writter.Close();

            }
        }
      
  
    }

}

