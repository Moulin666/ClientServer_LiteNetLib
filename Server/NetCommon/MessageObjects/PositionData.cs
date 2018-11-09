using System;


namespace NetCommon.MessageObjects
{
    [Serializable]
    public class PositionData
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public short Heading { get; set; }

        public PositionData() : this(0, 0, 0) { }

        public PositionData(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
            Heading = 0;
        }

        public PositionData(float x, float y, float z, short heading)
        {
            X = x;
            Y = y;
            Z = z;
            Heading = heading;
        }
    }
}
