using LiteNetLib.Utils;
using UnityEngine;


/// <summary>
/// Net message handler.
/// </summary>
public abstract class NetMessageHandler : MonoBehaviour
{
    /// <summary>
    /// Net message scriptable object.
    /// </summary>
    public NetMessage Message;

    /// <summary>
    /// Unity API event, callable when enable object.
    /// </summary>
    protected virtual void OnEnable() => Message.Subscribe(this);

    /// <summary>
    /// Unity API event, callable when disable object.
    /// </summary>
    protected virtual void OnDisable() => Message.Unsubscribe(this);

    /// <summary>
    /// Handle message.
    /// </summary>
    /// <param name="reader">Reader of parameters</param>
    public void HandleMessage(NetDataReader reader) => OnHandleMessage(reader);

    /// <summary>
    /// Event of handle message.
    /// </summary>
    /// <param name="reader">Reader of parameters</param>
    protected abstract void OnHandleMessage(NetDataReader reader);
}
