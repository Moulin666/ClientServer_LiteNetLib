using LiteNetLib.Utils;
using NetCommon;
using UnityEngine;


public class DestroyPlayerHandler : NetMessageHandler
{
    /// <summary>
    /// Handle message.
    /// </summary>
    /// <param name="reader">Reader of parameters</param>
    protected override void OnHandleMessage(NetDataReader reader)
    {
        //ParameterObject parameter = MessageSerializerService.DeserializeObjectOfType<ParameterObject>(reader.GetString());
        //long id = (long)parameter.Parameter;

        //if (ClientNetEventListener.Instance.NetObjects.ContainsKey(id))
        //{
        //    Destroy(ClientNetEventListener.Instance.NetObjects[id].gameObject);
        //    ClientNetEventListener.Instance.NetObjects.Remove(id);
        //}

        Debug.LogFormat("Destroy player handler.");
    }
}
