using Server.GameData;
using Server.Utils;
using System.Collections.Generic;
using System.Linq;


namespace Server.GameLogic.Session
{
    public class SessionCache
    {
        private static SessionCache _instance;

        public static SessionCache Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SessionCache();

                return _instance;
            }
        }

        public Dictionary<byte[], Session> Sessions { get; protected set; }

        private SessionCache () => Sessions = new Dictionary<byte[], Session>(new ByteArrayComparer ());

        public void ClearSessions ()
        {
            lock (Sessions)
            {
                foreach (var session in Sessions.Values)
                    session.Dispose();

                Sessions.Clear();
            }
        }

        public bool JoinSession (byte[] sessionId, Client player)
        {
            lock (Sessions)
            {
                if (Sessions.ContainsKey(sessionId))
                    return Sessions[sessionId].Join(player);

                return false;
            }
        }

        public void LeaveSession (byte[] sessionId, Client player)
        {
            lock (Sessions)
                if (Sessions.ContainsKey(sessionId))
                    Sessions[sessionId].Leave(player.NetPeer.Id);
        }

        public void CreateSession (byte[] sessionId, Dictionary<int, Unit> units, float startSessionTime)
        {
            if (GetSessionById(sessionId) != null)
                return;

            var session = new Session(sessionId, units, startSessionTime);

            lock (Sessions)
                Sessions[session.Id] = session;
        }

        public void DeleteSession (byte[] sessionId)
        {
            lock (Sessions)
            {
                if (Sessions.ContainsKey(sessionId))
                {
                    Sessions[sessionId].Dispose();
                    Sessions.Remove(sessionId);
                }
            }
        }

        public Session GetSessionById (byte[] sessionId)
        {
            lock (Sessions)
                return Sessions.FirstOrDefault(s => s.Key == sessionId).Value;
        }
    }
}
