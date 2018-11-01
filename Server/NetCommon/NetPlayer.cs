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

        public Boolean Moved;

        public PlayerData(long id)
        {
            Id = id;

            X = 0f;
            Y = 5f;
            Z = 0f;

            IsMine = false;
            Moved = false;
        }

        public PlayerData(long id, Boolean isMine)
        {
            Id = id;

            X = 0f;
            Y = 5f;
            Z = 0f;

            IsMine = IsMine;
            Moved = false;
        }
    }
}
