using System;
using System.Collections.Generic;


namespace NetCommon.MessageObjects
{
    [Serializable]
    public class PlayerData
    {
        public long Id { get; set; }

        public List<UnitData> Units;
    }
}
