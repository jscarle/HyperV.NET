namespace HyperV.Definitions
{
    public class CheckpointsDefinition
    {
        public CheckpointType Type { get; set; }
        private bool fallback;

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

        public string Path { get; set; }

        public CheckpointsDefinition()
        {
            Type = CheckpointType.Production;
            Fallback = true;
        }
    }
}