namespace HyperV
{
    ///<summary>Defines the type of checkpoint that will be created.</summary>
    public enum CheckpointType
    {
        ///<summary>Do not create checkpoints.</summary>
        None,

        ///<summary>Create production checkpoints.</summary>
        Production,

        ///<summary>Create standard checkpoints.</summary>
        Standard
    }
}