using System;


namespace NetCommon
{
    [Serializable]
    public class PlayerData
    {
        public long Id { get; set; }

        public Boolean IsMine { get; set; }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }
}
