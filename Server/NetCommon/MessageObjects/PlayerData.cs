using System;


namespace NetCommon.MessageObjects
{
    [Serializable]
    public class PlayerData
    {
        public long Id { get; set; }

        public PositionData PositionData { get; set; }

        public float Health { get; set; }

        public float Damage { get; set; }

        public float MoveSpeed { get; set; }

        public float AttackRadius { get; set; }
    }
}
