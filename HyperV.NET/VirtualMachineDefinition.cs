using HyperV.Definitions;

namespace HyperV
{
    ///<summary>Defines the configuration of a new virtual machine.</summary>
    public class VirtualMachineDefinition
    {
        ///<summary>Defines the Automatic Start Action settings.</summary>
        public AutomaticStartDefinition AutomaticStart { get; set; } = new AutomaticStartDefinition();

        ///<summary>Defines the Automatic Stop Action settings.</summary>
        public AutomaticStopDefinition AutomaticStop { get; set; } = new AutomaticStopDefinition();

        ///<summary>Defines the Checkpoints settings.</summary>
        public CheckpointsDefinition Checkpoints { get; set; } = new CheckpointsDefinition();

        ///<summary>Defines the Integration Services settings.</summary>
        public IntegrationServicesDefinition IntegrationServices { get; set; } = new IntegrationServicesDefinition();

        ///<summary>Defines the Memory settings.</summary>
        public MemoryDefinition Memory { get; set; } = new MemoryDefinition();

        ///<summary>The name of the virtual machine.</summary>
        public string Name { get; set; }

        ///<summary>Defines the settings for up to 8 Network Adapters.</summary>
        ///<remarks>The configuration includes one Network Adapter by default.</remarks>
        public NetworkAdaptersDefinition NetworkAdapters { get; set; } = new NetworkAdaptersDefinition();

        ///<summary>Notes for the virtual machine.</summary>
        public string Notes { get; set; }

        private string path;
        ///<summary>The path where the configuration files will be stored.</summary>
        ///<remarks>Setting the configuration path will reset the Checkpoints and Smart Paging paths to their defaults.</remarks>
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

        ///<summary>Defines the Processor settings.</summary>
        public ProcessorDefinition Processor { get; set; } = new ProcessorDefinition();

        ///<summary>Defines the settings for up to 4 SCSI Controllers.</summary>
        ///<remarks>The configuration includes one SCSI Controller by default.</remarks>
        public ScsiControllersDefinition ScsiControllers { get; set; } = new ScsiControllersDefinition();

        ///<summary>Defines the Security settings.</summary>
        public SecurityDefinition Security { get; set; } = new SecurityDefinition();

        ///<summary>Defines the Smart Paging settings.</summary>
        public SmartPagingDefinition SmartPaging { get; set; } = new SmartPagingDefinition();

        /// <summary>Initializes a new instance of the <see cref="VirtualMachineDefinition"/> class for the specified virtual machine name and configuration path.</summary>
        /// <param name="name">The name of the virtual machine.</param>
        /// <param name="path">The path where the configuration files will be stored.</param>
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