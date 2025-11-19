namespace Frends.Community.IBMMQ.Helpers;

/// <summary>
/// Connection type with Ibm Mqclient
/// </summary>
public enum ConnectionType
{
    /// <summary>
    /// refers to MQC.TRANSPORT_MQSERIES_MANAGED - connect as non-XA managed client
    /// </summary>
    Managed = 1,

    /// <summary>
    /// refers to MQC.TRANSPORT_MQSERIES_BINDINGS - connect as server
    /// </summary>
    Bindings = 2,

    /// <summary>
    /// refers to MQC.TRANSPORT_MQSERIES_CLIENT - connect as non-XA client
    /// </summary>
    Client = 3,

    /// <summary>
    /// refers to MQC.TRANSPORT_MQSERIES_XACLIENT - connect as XA client
    /// </summary>
    XaClient = 4,
}
