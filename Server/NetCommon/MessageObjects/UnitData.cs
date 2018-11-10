using System;


namespace NetCommon.MessageObjects
{
    [Serializable]
    public class UnitData
    {
        public int UnitId { get; set; }

        public PositionData PositionData { get; set; }

        public float Health { get; set; }

        public float MoveSpeed { get; set; }

        public float AttackRadius { get; set; }

        public float MinDamage { get; set; }

        public float MaxDamage { get; set; }

        public UnitData (int unitId, PositionData positionData, float health, float moveSpeed, float attackRadius, float minDamage,
            float maxDamage)
        {
            UnitId = unitId;

            PositionData = positionData;

            Health = health;
            MoveSpeed = moveSpeed;
            AttackRadius = attackRadius;
            MinDamage = minDamage;
            MaxDamage = maxDamage;
        }
    }
}
