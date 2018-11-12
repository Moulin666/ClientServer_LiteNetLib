using NetCommon.MessageObjects;
using System;


namespace Server.GameData
{
    public class Unit
    {
        public UnitData UnitData { get; set; }

        public Unit(UnitData unitData) => UnitData = unitData;

        public void SendDamage (Unit sender)
        {
            // TODO : Check valid operation.

            Random random = new Random();
            double range = sender.UnitData.MaxDamage - sender.UnitData.MinDamage;
            double nextDouble = random.NextDouble();
            float damage = (float)(nextDouble * range) + sender.UnitData.MinDamage;

            UnitData.Health -= damage;

            if (UnitData.Health < 0)
                UnitData.Health = 0; // TODO : Death.

            Console.WriteLine($"SendDamage. UnitId {UnitData.UnitId} get {damage} damage from UnitId {sender.UnitData.UnitId}");
        }
    }
}
