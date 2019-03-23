using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace WICR_Estimator.ViewModels
{
    //[Serializable]
    [DataContract]
    public class BaseViewModel : INotifyPropertyChanged
    {
        public BaseViewModel()
        { }
        
        public event PropertyChangedEventHandler PropertyChanged;

        //public event EventHandler JobPropertyChanged;
        
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
                //switch (propertyName)
                //{
                //    case "SpecialProductName":
                //    case "HasContingencyDisc":
                //    case "WeatherWearType":
                //    case "JobName":
                //    case "VendorName":
                //    case "TotalSqft":
                //    case "DeckPerimeter":
                //    case "RiserCount":
                //    case "IsApprovedForSandCement":
                //    case "IsPrevalingWage":
                //    case "HasSpecialMaterial":
                //    case "IsFlashingRequired":
                //    case "HasSpecialPricing":
                //    case "HasDiscount":
                //    case "MarkupPercentage":
                //    case "StairWidth":
                //    case "MaterialName":
                //        JobPropertyChanged.Invoke(this, EventArgs.Empty);
                //    default:
                //        break;
                //}
            }
        }

        //XmlSchema IXmlSerializable.GetSchema()
        //{
        //    return null;
        //}
        //private IList<IList<object>> m_Child;
        //public IList<IList<object>> Child
        //{
        //    get { return m_Child; }
        //    set { m_Child = value; }
        //}
        //void IXmlSerializable.ReadXml(XmlReader reader)
        //{
        //    reader.ReadStartElement("Child");
        //    string strType = reader.GetAttribute("type");
        //    XmlSerializer serial = new XmlSerializer(Type.GetType(strType));
        //    m_Child = (IList<IList<object>>)serial.Deserialize(reader);
        //    reader.ReadEndElement();
        //}

        //void IXmlSerializable.WriteXml(XmlWriter writer)
        //{
        //    writer.WriteStartElement("Child");
        //    string strType = m_Child.GetType().FullName;
        //    writer.WriteAttributeString("type", strType);
        //    XmlSerializer serial = new XmlSerializer(Type.GetType(strType));
        //    serial.Serialize(writer, m_Child);
        //    writer.WriteEndElement();
        //}
    }
}
