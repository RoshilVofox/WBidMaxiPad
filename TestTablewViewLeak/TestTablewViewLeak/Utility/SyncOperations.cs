using System;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;

namespace TestTablewViewLeak.Utility
{
    public class SyncOperations
    {
        public SyncOperations()
        {
        }
        public WBStateInfoDTO GetWBServerStateandquicksetVersionNumber(WBGetStateDTO wbStateDTO)
        {
            string data = SmartSyncLogic.JsonObjectToStringSerializer<WBGetStateDTO>(wbStateDTO);
            string url = GlobalSettings.synchServiceUrl+ "GetWBServerStateandquicksetVersionNumber";
            RestServiceUtil obj = new RestServiceUtil();
            string response = obj.PostData(url,data);
            WBStateInfoDTO versionInfo = CommonClass.ConvertJSonToObject<WBStateInfoDTO>(response);
            return versionInfo;
        }
    }
}
