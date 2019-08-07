namespace HyperV.Definitions
{
    ///<summary>Defines the Integration Services settings.</summary>
    public class IntegrationServicesDefinition
    {
        ///<summary>Offer Data Exchange services.</summary>
        public bool DataExchange { get; set; }

        ///<summary>Offer Guest services.</summary>
        public bool GuestServices { get; set; }

        ///<summary>Offer Heartbeat services.</summary>
        public bool Heartbeat { get; set; }

        ///<summary>Offer Operating System Shutdown services.</summary>
        public bool Shutdown { get; set; }

        ///<summary>Offer Time Synchronization services.</summary>
        public bool TimeSynchronisation { get; set; }

        ///<summary>Offer Backup (Volume Shadow Copy) services.</summary>
        public bool VolumeShadowCopy { get; set; }

        ///<summary>Initializes a new instance of the <see cref="IntegrationServicesDefinition"/> class.</summary>
        public IntegrationServicesDefinition()
        {
            DataExchange = true;
            Heartbeat = true;
            Shutdown = true;
            TimeSynchronisation = true;
            VolumeShadowCopy = true;
        }
    }
}