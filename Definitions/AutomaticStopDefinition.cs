namespace HyperV.Definitions
{
    public class AutomaticStopDefinition
    {
        public AutomaticStopAction Action { get; set; }

        public AutomaticStopDefinition()
        {
            Action = AutomaticStopAction.Save;
        }
    }
}