using System;


namespace NetCommon.Codes
{
    public enum NetOperationCode : Byte
    {
        JoinSession = 1,

        LeaveSession,

        StartSession,

        SetUnitPosition,

        MoveUnit,

        SendUnit,

        SendDamage,

        SendAttackUnit
    }
}
