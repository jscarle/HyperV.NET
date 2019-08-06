namespace HyperV.Definitions
{
    ///<summary>Defines the Automatic Stop Action settings.</summary>
    public class AutomaticStopDefinition
    {
        ///<summary>The action when the physical computer shuts down.</summary>
        public AutomaticStopAction Action { get; set; }

        public AutomaticStopDefinition()
        {
            Action = AutomaticStopAction.Save;
        }
    }
}