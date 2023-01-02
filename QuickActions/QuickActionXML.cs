using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronXml;
using Crestron.SimplSharp.CrestronXmlLinq;
using Crestron.SimplSharp.CrestronIO;


namespace ACS_4Series_Template_V2.QuickActions
{
    public class QuickActionXML
    {
        private string[] presetName = new string[100];
        public ushort[] PresetNum = new ushort[100];
        public ushort[,] Sources = new ushort[100,100];
        public ushort[,] Volumes = new ushort[100,100];
        public ushort Length;
        private string path;

        public string[] PresetName
        {
            get
            {
                return presetName;
            }
            set
            {
                presetName = value;
            }
        }

        /*public QuickActionXML(string[] names)
        {
            this.PresetName = names; //TO DO CONTINUE HERE!!! figure out why writing isn't working
        }*/
        public void readXML(string xmlFilePath)
        {
            using (XmlTextReader reader = new XmlTextReader(xmlFilePath))
            {
                ushort i = 0;
                path = xmlFilePath;
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        reader.ReadToFollowing("preset");
                        do
                        {
                            PresetNum[i] = Convert.ToUInt16(reader.GetAttribute("number"));
                            PresetName[i] = reader.GetAttribute("name");
                            string[] srcTemp = new string[100];
                            string[] volTemp = new string[100];
                            srcTemp = reader.GetAttribute("sources").Split(',');
                            volTemp = reader.GetAttribute("volumes").Split(',');
                            for (ushort j = 0; j < srcTemp.Length; j++)
                            {
                                Sources[i,j] = Convert.ToUInt16(srcTemp[j]);
                            }
                            for (ushort j = 0; j < volTemp.Length; j++)
                            {
                                Volumes[i,j] = Convert.ToUInt16(volTemp[j]);
                            }
                            i++;
                        }
                        while (reader.ReadToNextSibling("preset"));
                        Length = i;
                    }
                }
                reader.Dispose(true);
            }
        }
        public void writeXML(ushort presetNumberToWrite)
        {
            CrestronConsole.PrintLine("### {0}", presetName[presetNumberToWrite - 1]);
            XDocument loaded = XDocument.Parse(File.ReadToEnd(path, Encoding.UTF8));
            var target = loaded
                .Element("ROOT")
                .Element("musicPresets")
                .Elements("preset")
                .Where(e => e.Attribute("number").Value == Convert.ToString(presetNumberToWrite))
                .Single();
            target.Attribute("name").Value = presetName[presetNumberToWrite - 1];
            CrestronConsole.PrintLine("@@@ {0}", target);

            CrestronConsole.PrintLine("@@@ {0}", presetName[presetNumberToWrite - 1]);

            CrestronConsole.PrintLine("@@@ {0}", target.Attribute("name"));
            //target.Attribute("sources").Value = Sources[presetNumberToWrite - 1,0]

            using (var writer = new XmlTextWriter(path, new UTF8Encoding(false)))
            {
                writer.Formatting = Formatting.Indented;
                loaded.Save(writer);
            }
        }
    }
}
