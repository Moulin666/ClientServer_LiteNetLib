using System;


namespace NetCommon.Codes
{
    public enum NetOperationCode : Byte
    {
        SpawnPlayerCode = 1,

        SpawnPlayersCode,

        WorldEnter,

        MovePlayerCode,

        DestroyPlayer,
    }
}
