using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using WBid.WBidiPad.Model;
//using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Core;
//using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.Core.Enum;

namespace WBid.WBidiPad.SharedLibrary.Utility
{
    public class XmlHelper
    {
        #region Methods
        /// <summary>
        /// PURPOSE : Save Configuration details to XML
        /// </summary>
        /// <param name="wBidINI"></param>
        /// <returns></returns>
        public static bool SerializeToXml<T>(T configType, string filePath)
        {
            bool status = false;
            try
            {
                XmlWriterSettings xmlWriterSettings;
                XmlSerializerNamespaces xmlSerializerNamespaces;

                xmlWriterSettings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = false,
                    NamespaceHandling = NamespaceHandling.OmitDuplicates,
                    Encoding = Encoding.UTF8,
                    CloseOutput = true
                  

                };
                xmlSerializerNamespaces = new XmlSerializerNamespaces();
                xmlSerializerNamespaces.Add("", "");

                if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (FileStream configurationFileStream = new FileStream(filePath, FileMode.Create))
                {

                    using (XmlWriter xmlWriter = XmlWriter.Create(configurationFileStream, xmlWriterSettings))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(T));
                        serializer.Serialize(xmlWriter, configType, xmlSerializerNamespaces);
                    }
                }

                status = true;
            }
            catch (Exception ex)
            {
                status = false;
            }

            return status;
        }

        /// <summary>
        /// Load configuration details from XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T DeserializeFromXml<T>(string filePath)
        {
            try
            {

                T wBidConfiguration;
                using (TextReader configurationFileStream = new StreamReader(filePath))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    wBidConfiguration = (T)xmlSerializer.Deserialize(configurationFileStream);
                    return wBidConfiguration;
                }


            }
            catch (Exception ex)
            {
                throw;
            }
        }



//		public static void   ReCreateStateFile(string fileName, int lineCount, int startValue)
//		{
//
//
//
//
//			if (GlobalSettings.WbidUserContent == null || GlobalSettings.WbidUserContent.UserInformation == null)
//			{
//				GlobalSettings.WbidUserContent = (WbidUser)XmlHelper.DeserializeFromXml<WbidUser> (WBidHelper.WBidUserFilePath);
//
//
//			}
//
//			//WBidLogEvent logEvent = new WBidLogEvent();
//			//logEvent.LogEvent(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "wbsRecreate", "0", "0");
//
//
//
//
//
//			}

        public static WBidStateCollection ReadStateFile(string StatefileName)
        {
			

			WBidStateCollection wBidStateCollection = null;
					
			wBidStateCollection=	XmlHelper.DeserializeFromXml<WBidStateCollection>(StatefileName);
			

            if (decimal.Parse(wBidStateCollection.Version) < 2.0m)
            {
                foreach (WBidState state in wBidStateCollection.StateList)
                {
					if (state.Constraints.CitiesLegs == null) 
					{
						state.Constraints.CitiesLegs  = new Cx3Parameters {
							ThirdcellValue = "1",
							Type = (int)ConstraintType.LessThan,
							Value = 1 ,
							lstParameters = new List<Cx3Parameter> ()
						};
					}
                    if (state.Constraints.BulkOvernightCity == null)
                    {
                        state.Constraints.BulkOvernightCity = new BulkOvernightCityCx() { OverNightNo = new List<int>(), OverNightYes = new List<int>() };

                    }
                    if (state.Weights.OvernightCitybulk==null)
                    {
                        state.Weights.OvernightCitybulk = new List<Wt2Parameter>();

                    }
					if (state.Constraints.CLAuto == null) {
						state.Constraints.CLAuto = new FtCommutableLine () {ToHome = true,
							ToWork = false,
							NoNights = false,
							BaseTime = 10,
							ConnectTime = 30,
							CheckInTime = 120
						};
					}
					if (state.Weights.CLAuto == null) {
						state.Weights.CLAuto = new WtCommutableLineAuto (){
							ToHome = true,
							ToWork = false ,
							NoNights = false,
							BaseTime = 10,
							ConnectTime = 30,
							CheckInTime = 60
						};
					}
                    if (state.CxWtState.BulkOC == null)
                    {
                        state.CxWtState.BulkOC = new StateStatus() { Cx = false, Wt = false };

                    }
                    {

                    }

                 

                }
				foreach(var item in wBidStateCollection.StateList)
				{
					if (item.CxWtState.CLAuto == null) {
						item.CxWtState.CLAuto = new StateStatus{ Cx = false, Wt = false };
					}
					if (item.CxWtState.CitiesLegs == null) {
						item.CxWtState.CitiesLegs = new StateStatus{ Cx = false, Wt = false };
					}
					if (item.Constraints.CitiesLegs == null) 
					{
						item.Constraints.CitiesLegs  = new Cx3Parameters {
							ThirdcellValue = "1",
							Type = (int)ConstraintType.LessThan,
							Value = 1 ,
							lstParameters = new List<Cx3Parameter> ()
						};
					}
					if (item.Weights.CitiesLegs == null) 
					{
						item.Weights.CitiesLegs=new Wt2Parameters
						{
							Type = 1,
							Weight = 0   ,
							lstParameters=new List<Wt2Parameter>()
						};
					}
					if(item.CxWtState.Commute==null)
						item.CxWtState.Commute = new StateStatus { Wt = false, Cx = false };
					if (item.Constraints.Commute == null)
					{
						item.Constraints.Commute = new Commutability { BaseTime = 10, ConnectTime = 30, CheckInTime = 60, SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.LessThan, Value = 100 };
					}
					if (item.Weights.Commute == null)
					{
						item.Weights.Commute = new Commutability { BaseTime = 10, ConnectTime = 30, CheckInTime = 60, SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.LessThan, Value = 100,Weight=0 };
					}
                    if (item.CxWtState.StartDay == null)
                        item.CxWtState.StartDay = new StateStatus { Cx = false, Wt = false };
                    if (item.CxWtState.ReportRelease == null)
                        item.CxWtState.ReportRelease = new StateStatus { Cx = false, Wt = false };

                    if (item.Constraints.StartDay == null)
                    {
                        item.Constraints.StartDay = new Cx3Parameters { ThirdcellValue = "1", Type = 1, Value = 1 };
                    }
                    if (item.Constraints.ReportRelease == null)
                    {
                        item.Constraints.ReportRelease = new ReportReleases { AllDays = true, First = false, Last = false, NoMid = false, Report = 0, Release = 0 };
                    }

				}
                XmlHelper.SerializeToXml<WBidStateCollection>(wBidStateCollection, StatefileName);


               
            }

            return wBidStateCollection;

        }


        #endregion


    }
}
