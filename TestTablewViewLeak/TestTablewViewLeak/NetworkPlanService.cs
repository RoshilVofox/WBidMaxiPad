//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18051
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by SlSvcUtil, version 5.0.61118.0
// 
namespace NetworkPlanService.Model
{
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="FlightPlan", Namespace="http://schemas.datacontract.org/2004/07/NetworkPlanService.Model")]
    public partial class FlightPlan : object
    {
        
        private NetworkPlanService.Model.FlightDetails[] FlightDetailsField;
        
        private NetworkPlanService.Model.FlightRoute[] FlightRoutesField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public NetworkPlanService.Model.FlightDetails[] FlightDetails
        {
            get
            {
                return this.FlightDetailsField;
            }
            set
            {
                this.FlightDetailsField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public NetworkPlanService.Model.FlightRoute[] FlightRoutes
        {
            get
            {
                return this.FlightRoutesField;
            }
            set
            {
                this.FlightRoutesField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="FlightDetails", Namespace="http://schemas.datacontract.org/2004/07/NetworkPlanService.Model")]
    public partial class FlightDetails : object
    {
        
        private int CarrField;
        
        private int CdepField;
        
        private string DestField;
        
        private int FlightField;
        
        private int FlightIdField;
        
        private int LarrField;
        
        private int LdepField;
        
        private string OrigField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Carr
        {
            get
            {
                return this.CarrField;
            }
            set
            {
                this.CarrField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Cdep
        {
            get
            {
                return this.CdepField;
            }
            set
            {
                this.CdepField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Dest
        {
            get
            {
                return this.DestField;
            }
            set
            {
                this.DestField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Flight
        {
            get
            {
                return this.FlightField;
            }
            set
            {
                this.FlightField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int FlightId
        {
            get
            {
                return this.FlightIdField;
            }
            set
            {
                this.FlightIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Larr
        {
            get
            {
                return this.LarrField;
            }
            set
            {
                this.LarrField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Ldep
        {
            get
            {
                return this.LdepField;
            }
            set
            {
                this.LdepField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Orig
        {
            get
            {
                return this.OrigField;
            }
            set
            {
                this.OrigField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="FlightRoute", Namespace="http://schemas.datacontract.org/2004/07/NetworkPlanService.Model")]
    public partial class FlightRoute : object
    {
        
        private System.DateTime FlightDateField;
        
        private int FlightIdField;
        
        private int RouteNumField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime FlightDate
        {
            get
            {
                return this.FlightDateField;
            }
            set
            {
                this.FlightDateField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int FlightId
        {
            get
            {
                return this.FlightIdField;
            }
            set
            {
                this.FlightIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int RouteNum
        {
            get
            {
                return this.RouteNumField;
            }
            set
            {
                this.RouteNumField = value;
            }
        }
    }
}


[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(ConfigurationName="INetworkPlanService")]
public interface INetworkPlanService
{
    
    [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/INetworkPlanService/GetFlights", ReplyAction="http://tempuri.org/INetworkPlanService/GetFlightsResponse")]
    System.IAsyncResult BeginGetFlights(System.DateTime fromDate, System.DateTime toDate, System.AsyncCallback callback, object asyncState);
    
    NetworkPlanService.Model.FlightPlan EndGetFlights(System.IAsyncResult result);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public interface INetworkPlanServiceChannel : INetworkPlanService, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public partial class GetFlightsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
{
    
    private object[] results;
    
    public GetFlightsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
            base(exception, cancelled, userState)
    {
        this.results = results;
    }
    
    public NetworkPlanService.Model.FlightPlan Result
    {
        get
        {
            base.RaiseExceptionIfNecessary();
            return ((NetworkPlanService.Model.FlightPlan)(this.results[0]));
        }
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public partial class NetworkPlanServiceClient : System.ServiceModel.ClientBase<INetworkPlanService>, INetworkPlanService
{
    
    private BeginOperationDelegate onBeginGetFlightsDelegate;
    
    private EndOperationDelegate onEndGetFlightsDelegate;
    
    private System.Threading.SendOrPostCallback onGetFlightsCompletedDelegate;
    
    private BeginOperationDelegate onBeginOpenDelegate;
    
    private EndOperationDelegate onEndOpenDelegate;
    
    private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
    
    private BeginOperationDelegate onBeginCloseDelegate;
    
    private EndOperationDelegate onEndCloseDelegate;
    
    private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
    
    public NetworkPlanServiceClient()
    {
    }
    
    public NetworkPlanServiceClient(string endpointConfigurationName) : 
            base(endpointConfigurationName)
    {
    }
    
    public NetworkPlanServiceClient(string endpointConfigurationName, string remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public NetworkPlanServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public NetworkPlanServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
    {
    }
    
    public System.Net.CookieContainer CookieContainer
    {
        get
        {
            System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
            if ((httpCookieContainerManager != null))
            {
                return httpCookieContainerManager.CookieContainer;
            }
            else
            {
                return null;
            }
        }
        set
        {
            System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
            if ((httpCookieContainerManager != null))
            {
                httpCookieContainerManager.CookieContainer = value;
            }
            else
            {
                throw new System.InvalidOperationException("Unable to set the CookieContainer. Please make sure the binding contains an HttpC" +
                        "ookieContainerBindingElement.");
            }
        }
    }
    
    public event System.EventHandler<GetFlightsCompletedEventArgs> GetFlightsCompleted;
    
    public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
    
    public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
    
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    System.IAsyncResult INetworkPlanService.BeginGetFlights(System.DateTime fromDate, System.DateTime toDate, System.AsyncCallback callback, object asyncState)
    {
        return base.Channel.BeginGetFlights(fromDate, toDate, callback, asyncState);
    }
    
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    NetworkPlanService.Model.FlightPlan INetworkPlanService.EndGetFlights(System.IAsyncResult result)
    {
        return base.Channel.EndGetFlights(result);
    }
    
    private System.IAsyncResult OnBeginGetFlights(object[] inValues, System.AsyncCallback callback, object asyncState)
    {
        System.DateTime fromDate = ((System.DateTime)(inValues[0]));
        System.DateTime toDate = ((System.DateTime)(inValues[1]));
        return ((INetworkPlanService)(this)).BeginGetFlights(fromDate, toDate, callback, asyncState);
    }
    
    private object[] OnEndGetFlights(System.IAsyncResult result)
    {
        NetworkPlanService.Model.FlightPlan retVal = ((INetworkPlanService)(this)).EndGetFlights(result);
        return new object[] {
                retVal};
    }
    
    private void OnGetFlightsCompleted(object state)
    {
        if ((this.GetFlightsCompleted != null))
        {
            InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
            this.GetFlightsCompleted(this, new GetFlightsCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
        }
    }
    
    public void GetFlightsAsync(System.DateTime fromDate, System.DateTime toDate)
    {
        this.GetFlightsAsync(fromDate, toDate, null);
    }
    
    public void GetFlightsAsync(System.DateTime fromDate, System.DateTime toDate, object userState)
    {
        if ((this.onBeginGetFlightsDelegate == null))
        {
            this.onBeginGetFlightsDelegate = new BeginOperationDelegate(this.OnBeginGetFlights);
        }
        if ((this.onEndGetFlightsDelegate == null))
        {
            this.onEndGetFlightsDelegate = new EndOperationDelegate(this.OnEndGetFlights);
        }
        if ((this.onGetFlightsCompletedDelegate == null))
        {
            this.onGetFlightsCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetFlightsCompleted);
        }
        base.InvokeAsync(this.onBeginGetFlightsDelegate, new object[] {
                    fromDate,
                    toDate}, this.onEndGetFlightsDelegate, this.onGetFlightsCompletedDelegate, userState);
    }
    
    private System.IAsyncResult OnBeginOpen(object[] inValues, System.AsyncCallback callback, object asyncState)
    {
        return ((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(callback, asyncState);
    }
    
    private object[] OnEndOpen(System.IAsyncResult result)
    {
        ((System.ServiceModel.ICommunicationObject)(this)).EndOpen(result);
        return null;
    }
    
    private void OnOpenCompleted(object state)
    {
        if ((this.OpenCompleted != null))
        {
            InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
            this.OpenCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
        }
    }
    
    public void OpenAsync()
    {
        this.OpenAsync(null);
    }
    
    public void OpenAsync(object userState)
    {
        if ((this.onBeginOpenDelegate == null))
        {
            this.onBeginOpenDelegate = new BeginOperationDelegate(this.OnBeginOpen);
        }
        if ((this.onEndOpenDelegate == null))
        {
            this.onEndOpenDelegate = new EndOperationDelegate(this.OnEndOpen);
        }
        if ((this.onOpenCompletedDelegate == null))
        {
            this.onOpenCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnOpenCompleted);
        }
        base.InvokeAsync(this.onBeginOpenDelegate, null, this.onEndOpenDelegate, this.onOpenCompletedDelegate, userState);
    }
    
    private System.IAsyncResult OnBeginClose(object[] inValues, System.AsyncCallback callback, object asyncState)
    {
        return ((System.ServiceModel.ICommunicationObject)(this)).BeginClose(callback, asyncState);
    }
    
    private object[] OnEndClose(System.IAsyncResult result)
    {
        ((System.ServiceModel.ICommunicationObject)(this)).EndClose(result);
        return null;
    }
    
    private void OnCloseCompleted(object state)
    {
        if ((this.CloseCompleted != null))
        {
            InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
            this.CloseCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
        }
    }
    
    public void CloseAsync()
    {
        this.CloseAsync(null);
    }
    
    public void CloseAsync(object userState)
    {
        if ((this.onBeginCloseDelegate == null))
        {
            this.onBeginCloseDelegate = new BeginOperationDelegate(this.OnBeginClose);
        }
        if ((this.onEndCloseDelegate == null))
        {
            this.onEndCloseDelegate = new EndOperationDelegate(this.OnEndClose);
        }
        if ((this.onCloseCompletedDelegate == null))
        {
            this.onCloseCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnCloseCompleted);
        }
        base.InvokeAsync(this.onBeginCloseDelegate, null, this.onEndCloseDelegate, this.onCloseCompletedDelegate, userState);
    }
    
    protected override INetworkPlanService CreateChannel()
    {
        return new NetworkPlanServiceClientChannel(this);
    }
    
    private class NetworkPlanServiceClientChannel : ChannelBase<INetworkPlanService>, INetworkPlanService
    {
        
        public NetworkPlanServiceClientChannel(System.ServiceModel.ClientBase<INetworkPlanService> client) : 
                base(client)
        {
        }
        
        public System.IAsyncResult BeginGetFlights(System.DateTime fromDate, System.DateTime toDate, System.AsyncCallback callback, object asyncState)
        {
            object[] _args = new object[2];
            _args[0] = fromDate;
            _args[1] = toDate;
            System.IAsyncResult _result = base.BeginInvoke("GetFlights", _args, callback, asyncState);
            return _result;
        }
        
        public NetworkPlanService.Model.FlightPlan EndGetFlights(System.IAsyncResult result)
        {
            object[] _args = new object[0];
            NetworkPlanService.Model.FlightPlan _result = ((NetworkPlanService.Model.FlightPlan)(base.EndInvoke("GetFlights", _args, result)));
            return _result;
        }
    }
}
