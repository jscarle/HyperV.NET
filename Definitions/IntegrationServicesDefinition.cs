namespace HyperV.Definitions
{
    public class IntegrationServicesDefinition
    {
        public bool DataExchange { get; set; }
        public bool GuestServices { get; set; }
        public bool Heartbeat { get; set; }
        public bool Shutdown { get; set; }
        public bool TimeSynchronisation { get; set; }
        public bool VolumeShadowCopy { get; set; }

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