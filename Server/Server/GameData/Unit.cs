using NetCommon.MessageObjects;
using System;


namespace Server.GameData
{
    public class Unit
    {
        public Client Owner { get; set; }

        public UnitData UnitData { get; set; }

        public void SendDamage (Unit sender)
        {
            // TODO : Check valid operation.

            Random random = new Random();
            double range = sender.UnitData.MaxDamage - sender.UnitData.MinDamage;
            double nextDouble = random.NextDouble();
            float damage = (float)(nextDouble * range) + sender.UnitData.MinDamage;

            UnitData.Health -= damage;

            Console.WriteLine($"SendDamage. UnitId {UnitData.UnitId} get {damage} damage from UnitId {sender.UnitData.UnitId}");
        }
    }
}
