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
				if (filePath.Substring (filePath.Length - 2, 2) == "qs")
					File.Delete (filePath);
				
                throw;
            }
        }
		private static Object readlLock = new Object();
		private static Object writecLock = new Object();
		public static bool SerializeToXmlForUserFile<T>(T configType, string filePath)
		{
			
			bool status = false;
			try
			{
				lock (writecLock)
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

					using (FileStream configurationFileStream = new FileStream(filePath, FileMode.Create,FileAccess.ReadWrite,FileShare.ReadWrite))
				{

					using (XmlWriter xmlWriter = XmlWriter.Create(configurationFileStream, xmlWriterSettings))
					{
						XmlSerializer serializer = new XmlSerializer(typeof(T));
						serializer.Serialize(xmlWriter, configType, xmlSerializerNamespaces);
							xmlWriter.Close();
					}
				}

				status = true;
				}
					
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
		public static T DeserializeFromXmlForUserFile<T>(string filePath)
		{
			try
			{

				lock (readlLock)
				{
				T wBidConfiguration;
				using (TextReader configurationFileStream = new StreamReader(filePath))
				{
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
					wBidConfiguration = (T)xmlSerializer.Deserialize(configurationFileStream);
					return wBidConfiguration;
				}
				}


			}
			catch (Exception ex)
			{
				if (filePath.Substring (filePath.Length - 2, 2) == "qs")
					File.Delete (filePath);

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

			foreach (WBidState state in wBidStateCollection.StateList)
			{
				
				if (state.CxWtState.CLAuto == null) {
					state.CxWtState.CLAuto = new StateStatus{ Cx = false, Wt = false };
				}
				if (state.CxWtState.CitiesLegs == null) {
					state.CxWtState.CitiesLegs = new StateStatus{ Cx = false, Wt = false };
				}
				if(state.CxWtState.Commute==null)
					state.CxWtState.Commute = new StateStatus { Wt = false, Cx = false };

				if (state.Constraints.CitiesLegs == null)
				{
					state.Constraints.CitiesLegs = new Cx3Parameters {
						ThirdcellValue = "1",
						Type = (int)ConstraintType.LessThan,
						Value = 1 ,
						lstParameters = new List<Cx3Parameter> ()
					};
				}
				if (state.Weights.CitiesLegs == null)
				{
					state.Weights.CitiesLegs = new Wt2Parameters {
						Type = 1,
						Weight = 0,
						lstParameters = new List<Wt2Parameter> ()
					};
				}
				if (state.Constraints.Commute == null)
				{
					state.Constraints.Commute = new Commutability { BaseTime = 10, ConnectTime = 30, CheckInTime = 60, SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.MoreThan, Value = 100 };
				}
				if (state.Weights.Commute == null)
				{
					state.Weights.Commute = new Commutability { BaseTime = 10, ConnectTime = 30, CheckInTime = 60, SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.MoreThan, Value = 100,Weight=0 };
				}
				if(state.Weights.ETOPS==null)
                {
					state.Weights.ETOPS = new Wt1Parameters
					{
						
						Weight = 0,
						lstParameters = new List<Wt1Parameter>()
					};
				}
				if (state.Weights.ETOPSRes == null)
				{
					state.Weights.ETOPSRes = new Wt1Parameters
					{

						Weight = 0,
						lstParameters = new List<Wt1Parameter>()
					};
				}
				if (state.CxWtState.ETOPS == null)
				{
					state.CxWtState.ETOPS = new StateStatus { Cx = false, Wt = false };
				}
				if (state.CxWtState.ETOPSRes == null)
				{
					state.CxWtState.ETOPSRes = new StateStatus { Cx = false, Wt = false };
				}
				state.CxWtState.Commute.Cx = false;
                state.CxWtState.Commute.Wt = false;
				if ((decimal.Parse(wBidStateCollection.Version) < 2.6m))
				{
					//setting the default value for the PDO to any city and any date
					state.Constraints.PDOFS.SecondcellValue = "300";
					state.Constraints.PDOFS.ThirdcellValue = "400";
					state.Weights.PDAfter.FirstValue = 300;
					state.Weights.PDAfter.ThrirdCellValue = 400;

					state.Weights.PDBefore.FirstValue = 300;
					state.Weights.PDBefore.ThrirdCellValue = 400;
				}
			}

            if (decimal.Parse(wBidStateCollection.Version) < 2.0m)
            {
                foreach (WBidState state in wBidStateCollection.StateList)
                {
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
							CheckInTime = 120
						};
					}
                    if (state.CxWtState.BulkOC == null)
                    {
                        state.CxWtState.BulkOC = new StateStatus() { Cx = false, Wt = false };

                    }
                    {

                    }

                 

                }
//				foreach(var item in wBidStateCollection.StateList)
//				{
//					if (item.CxWtState.CLAuto == null) {
//						item.CxWtState.CLAuto = new StateStatus{ Cx = false, Wt = false };
//					}
//					if (item.CxWtState.CitiesLegs == null) {
//						item.CxWtState.CitiesLegs = new StateStatus{ Cx = false, Wt = false };
//					}
//				}
                XmlHelper.SerializeToXml<WBidStateCollection>(wBidStateCollection, StatefileName);


               
            }

			if (decimal.Parse (wBidStateCollection.Version) < 2.4m) 
			{
				XmlHelper.SerializeToXml<WBidStateCollection>(wBidStateCollection, StatefileName);
			}
			if (decimal.Parse(wBidStateCollection.Version) < 2.8m)
			{
				foreach (WBidState state in wBidStateCollection.StateList)
				{
					state.Constraints.EQUIP.ThirdcellValue = "700";
					if (state.CxWtState.EQUIP.Cx)
					{
						state.Constraints.EQUIP.lstParameters.RemoveAll(x => x.ThirdcellValue == "800" || x.ThirdcellValue == "300");
						if (state.Constraints.EQUIP.lstParameters.Count == 0)
							state.CxWtState.EQUIP.Cx = false;
					}
					state.Weights.EQUIP.SecondlValue = 700;
					if (state.CxWtState.EQUIP.Wt)
					{
						state.Weights.EQUIP.lstParameters.RemoveAll(x => x.SecondlValue == 800 || x.SecondlValue == 300);
						if (state.Weights.EQUIP.lstParameters.Count == 0)
							state.CxWtState.EQUIP.Wt = false;
					}
					if (state.BidAuto != null && state.BidAuto.BAFilter != null && state.BidAuto.BAFilter.Count > 0)
					{
						//var BAEquipmenFilter = state.BidAuto.BAFilter.Where(x => x.Name == "ET");
						state.BidAuto.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "500");

						state.BidAuto.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "300");
					}
					if (state.CalculatedBA != null && state.CalculatedBA.BAFilter != null && state.CalculatedBA.BAFilter.Count > 0)
					{
						state.CalculatedBA.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "500");

						state.CalculatedBA.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "300");
					}
		   

				}
			}
            if (decimal.Parse(wBidStateCollection.Version) < 2.9m)
            {
                foreach (WBidState state in wBidStateCollection.StateList)
                {
                    if (state.CxWtState.StartDay == null)
                        state.CxWtState.StartDay = new StateStatus { Cx = false, Wt = false };
                    if (state.CxWtState.ReportRelease == null)
                        state.CxWtState.ReportRelease = new StateStatus { Cx = false, Wt = false };

                    if(state.Constraints.StartDay==null)
                    {
                        state.Constraints.StartDay = new Cx3Parameters { ThirdcellValue = "1", Type = 1, Value = 1 };
                    }
                    if(state.Constraints.ReportRelease==null)
                    {
                        state.Constraints.ReportRelease = new ReportReleases { AllDays = true, First = false, Last = false, NoMid = false, Report = 0, Release = 0 };
                    }

                      
                }
            }

            return wBidStateCollection;

        }


        #endregion


    }
}
