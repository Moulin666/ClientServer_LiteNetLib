using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;


namespace NetCommon
{
    /// <summary>
    /// Serializer service. JSon and BSon serialize and deserialize operations.
    /// </summary>
    public class MessageSerializerService
    {
        /// <summary>
        /// Serialize object of type to JSon or BSon.
        /// </summary>
        /// <typeparam name="T">Class</typeparam>
        /// <param name="objectToSerialize">Object to serialize of type to JSon or BSon</param>
        /// <returns>Serialize object.</returns>
        public static String SerializeObjectOfType<T>(T objectToSerialize) where T : class
        {
            string result = null;

#if DEBUG
            result = SerializeJson<T>(objectToSerialize);
#else
			result = SerializeBson<T>(objectToSerialize);
#endif

            return result;
        }

        /// <summary>
        /// Deserialize object of type from JSon or BSon.
        /// </summary>
        /// <typeparam name="T">Class</typeparam>
        /// <param name="objectToDeserialize">Object to deserialize of type from JSon or BSon</param>
        /// <returns>Deserialize T. Where T : class</returns>
        public static T DeserializeObjectOfType<T>(Object objectToDeserialize) where T : class
        {
            T result = null;

#if DEBUG
            result = DeserializeJson<T>(objectToDeserialize);
#else
			result = DeserializeBson<T>(objectToDeserialize);
#endif

            return result;
        }

        #region JSon

        protected static String SerializeJson<T>(T objectToSerialize) where T : class
        {
            return JsonConvert.SerializeObject(objectToSerialize);
        }

        protected static T DeserializeJson<T>(Object objectToDeserialize) where T : class
        {
            return JsonConvert.DeserializeObject<T>((string)objectToDeserialize);
        }

        #endregion

        #region BSon

        protected static String SerializeBson<T>(T objectToSerialize) where T : class
        {
            MemoryStream memoryStream = new MemoryStream();

            using (BsonDataWriter writer = new BsonDataWriter(memoryStream))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, objectToSerialize);
            }

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        protected static T DeserializeBson<T>(Object objectToDeserialize) where T : class
        {
            byte[] data = Convert.FromBase64String((string)objectToDeserialize);

            MemoryStream memoryStream = new MemoryStream(data);

            using (BsonDataReader reader = new BsonDataReader(memoryStream))
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<T>(reader);
            }
        }

        #endregion
    }
}