namespace HyperV.Definitions
{
    ///<summary>Defines the Automatic Stop Action settings.</summary>
    public class AutomaticStopDefinition
    {
        ///<summary>The action when the physical computer shuts down.</summary>
        public AutomaticStopAction Action { get; set; }

        ///<summary>Initializes a new instance of the <see cref="AutomaticStopDefinition"/> class.</summary>
        public AutomaticStopDefinition()
        {
            Action = AutomaticStopAction.Save;
        }
    }
}