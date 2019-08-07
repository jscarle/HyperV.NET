namespace HyperV.Definitions
{
    ///<summary>Defines the Checkpoints settings.</summary>
    public class CheckpointsDefinition
    {
        ///<summary>The type of checkpoint that will be created.</summary>
        public CheckpointType Type { get; set; }

        private bool fallback;

        ///<summary>Create standard checkpoints if it's not possible to create a production checkpoint.</summary>
        public bool Fallback
        {
            get { return fallback; }
            set
            {
                if (value)
                {
                    Type = CheckpointType.Production;
                    fallback = true;
                }
                else
                {
                    fallback = false;
                }
            }
        }

        ///<summary>The path where the Checkpoint files will be stored.</summary>
        public string Path { get; set; }

        ///<summary>Initializes a new instance of the <see cref="CheckpointsDefinition"/> class.</summary>
        public CheckpointsDefinition()
        {
            Type = CheckpointType.Production;
            Fallback = true;
        }
    }
}