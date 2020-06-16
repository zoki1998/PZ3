using PZ2.ELEMENTS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PZ2
{
    public class XmlParser
    {
        public XmlParser()
        {
        }
    
        public void ParseXML(string xmlPath, out List<SubstationEntity> subList, out List<NodeEntity> nodeList, out List<SwitchEntity> switchList, out List<LineEntity> lineList )        //cita sve korisnika iz xml-a
        {          
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);

            SubstationEntity sub = new SubstationEntity();
            NodeEntity ne = new NodeEntity();
            SwitchEntity se = new SwitchEntity();
            LineEntity le = new LineEntity();
            MyPoint p = new MyPoint();
            double tempX, tempY;

            subList = new List<SubstationEntity>();
            nodeList = new List<NodeEntity>();
            switchList = new List<SwitchEntity>();
            lineList = new List<LineEntity>();

            foreach (XmlNode node in doc.DocumentElement.ChildNodes)    //uzima sve cvorove
            {
                foreach (XmlNode item in node.ChildNodes)       //uzima sve child cvorove prvih cvorova (tu su konkretni elementi)
                {
                    if (item.Name.Equals("SubstationEntity"))           //za svaki element konkretnog taga, pravi istancu tog elementa i dodaje je u odgovarajucu grupu
                    {
                        sub = new SubstationEntity();
                        sub.ID = long.Parse(item.ChildNodes[0].InnerText);
                        sub.Name = item.ChildNodes[1].InnerText;                       

                        UTMtoDecimal.ToLatLon(double.Parse(item.ChildNodes[2].InnerText), double.Parse(item.ChildNodes[3].InnerText), 34, out tempX, out tempY);    //odmah pretvara u decimalne vrednosti i takve cuva u x i y
                        sub.X = tempX;
                        sub.Y = tempY;
                        subList.Add(sub);
                    }

                    if (item.Name.Equals("NodeEntity"))
                    {
                        ne = new NodeEntity();
                        ne.ID = long.Parse(item.ChildNodes[0].InnerText);
                        ne.Name = item.ChildNodes[1].InnerText;                      

                        UTMtoDecimal.ToLatLon(double.Parse(item.ChildNodes[2].InnerText), double.Parse(item.ChildNodes[3].InnerText), 34, out tempX, out tempY);
                        ne.X = tempX;
                        ne.Y = tempY;

                        nodeList.Add(ne);
                    }

                    if (item.Name.Equals("SwitchEntity"))
                    {
                        se = new SwitchEntity();
                        se.ID = long.Parse(item.ChildNodes[0].InnerText);
                        se.Name = item.ChildNodes[1].InnerText;
                        se.Status = item.ChildNodes[2].InnerText;                      

                        UTMtoDecimal.ToLatLon(double.Parse(item.ChildNodes[3].InnerText), double.Parse(item.ChildNodes[4].InnerText), 34, out tempX, out tempY);
                        se.X = tempX;
                        se.Y = tempY;
                        switchList.Add(se);
                    }

                    if (item.Name.Equals("LineEntity"))
                    {
                        le = new LineEntity();
                        le.ID = long.Parse(item.ChildNodes[0].InnerText);
                        le.Name = item.ChildNodes[1].InnerText;
                        le.IsUnderground = bool.Parse(item.ChildNodes[2].InnerText);
                        le.R = double.Parse(item.ChildNodes[3].InnerText);
                        le.ConductorMaterial = item.ChildNodes[4].InnerText;
                        le.LineType = item.ChildNodes[5].InnerText;
                        le.TerminalConstantHeat = int.Parse(item.ChildNodes[6].InnerText);
                        le.FirstEnd = double.Parse(item.ChildNodes[7].InnerText);
                        le.SecondEnd = double.Parse(item.ChildNodes[8].InnerText);                       
                        foreach (XmlNode pointItem in item.ChildNodes[9].ChildNodes) 
                        {
                            p = new MyPoint();
                            UTMtoDecimal.ToLatLon(double.Parse(pointItem.ChildNodes[0].InnerText), double.Parse(pointItem.ChildNodes[1].InnerText), 34, out tempX, out tempY);
                            p.X = tempX;
                            p.Y = tempY;
                            le.Vertices.Add(p);
                        }
                        lineList.Add(le);
                    }
                }             
            }          
        }
    }
}
