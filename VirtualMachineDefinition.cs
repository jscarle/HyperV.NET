using HyperV.Definitions;

namespace HyperV
{
    public class VirtualMachineDefinition
    {
        public AutomaticStartDefinition AutomaticStart { get; set; } = new AutomaticStartDefinition();
        public AutomaticStopDefinition AutomaticStop { get; set; } = new AutomaticStopDefinition();
        public CheckpointsDefinition Checkpoints { get; set; } = new CheckpointsDefinition();
        public IntegrationServicesDefinition IntegrationServices { get; set; } = new IntegrationServicesDefinition();
        public MemoryDefinition Memory { get; set; } = new MemoryDefinition();
        public string Name { get; set; }
        public string Notes { get; set; }
        private string path;

        public string Path
        {
            get { return path; }
            set
            {
                path = value;
                Checkpoints.Path = value;
                SmartPaging.Path = value;
            }
        }

        public ProcessorDefinition Processor { get; set; } = new ProcessorDefinition();
        public ScsiControllersDefinition ScsiControllers { get; set; } = new ScsiControllersDefinition();
        public SecurityDefinition Security { get; set; } = new SecurityDefinition();
        public SmartPagingDefinition SmartPaging { get; set; } = new SmartPagingDefinition();
        public NetworkAdaptersDefinition NetworkAdapters { get; set; } = new NetworkAdaptersDefinition();

        public VirtualMachineDefinition(string name, string path)
        {
            Name = name;
            Notes = "";
            Path = path;
            ScsiControllers.Add(new ScsiController());
            NetworkAdapters.Add(new NetworkAdapter());
        }
    }
}