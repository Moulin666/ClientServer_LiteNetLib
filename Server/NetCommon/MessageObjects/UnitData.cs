using System;


namespace NetCommon.MessageObjects
{
    [Serializable]
    public class UnitData
    {
        public Guid UnitId { get; set; }

        public PositionData PositionData { get; set; }

        public float Health { get; set; }

        public float MoveSpeed { get; set; }

        public float AttackRadius { get; set; }

        public float MinDamage { get; set; }

        public float MaxDamage { get; set; }
    }
}
