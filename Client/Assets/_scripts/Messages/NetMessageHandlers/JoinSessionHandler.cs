﻿using LiteNetLib.Utils;
using NetCommon.Codes;
using UnityEngine;


public class JoinSessionHandler : NetMessageHandler
{
    /// <summary>
    /// Handle message.
    /// </summary>
    /// <param name="reader">Reader of parameters</param>
    protected override void OnHandleMessage(NetDataReader reader)
    {
        NetErrorCode errorCode = (NetErrorCode)reader.GetByte();

        if (errorCode == NetErrorCode.SessionConnectedFailed)
        {
            Debug.Log("Session connected failed");
            return;
        }

        BattleManager.Instance.JoinSessionSuccess();
    }
}
