using LiteNetLib.Utils;
using NetCommon.Codes;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Scriptable net message object.
/// </summary>
[CreateAssetMenu]
public class NetMessage : ScriptableObject
{
    /// <summary>
    /// Net operation code.
    /// </summary>
    [Header ("Net operation code")] public NetOperationCode Code;

    /// <summary>
    /// List of net message handlers.
    /// </summary>
    private List<NetMessageHandler> handlers = new List<NetMessageHandler>();

    /// <summary>
    /// Subscribe to notify new handler.
    /// </summary>
    /// <param name="handler">Handler for subscribe</param>
    public void Subscribe (NetMessageHandler handler)
    {
        handlers.Add(handler);
    }

    /// <summary>
    /// unsubscribe to notify handler.
    /// </summary>
    /// <param name="handler">Handler for unsubscribe</param>
    public void Unsubscribe (NetMessageHandler handler)
    {
        handlers.Remove(handler);
    }

    /// <summary>
    /// Notify handlers.
    /// </summary>
    /// <param name="reader">Reader of parameters</param>
    public void Notify (NetDataReader reader)
    {
        for (int i = handlers.Count - 1; i >= 0; i--)
            handlers[i].HandleMessage(reader);
    }
}
