namespace HyperV
{
    ///<summary>Defines the port mirroring mode for a network adapter.</summary>
    public enum PortMirroringMode
    {
        ///<summary>The network adapter will not participate in mirroring.</summary>
        None,

        ///<summary>The network adapter will accept mirrored data from source ports.</summary>
        Destination,

        ///<summary>The network adapter will send mirrored data to detination ports.</summary>
        Source
    }
}