﻿using System.Collections.Generic;
using System.Linq;


namespace Server.GameData
{
    public class SessionCache
    {
        public static SessionCache Instance = new SessionCache();

        public Dictionary<byte[], Session> Sessions { get; protected set; }

        public SessionCache() => Sessions = new Dictionary<byte[], Session>();

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

        public void CreateSession (byte[] sessionId)
        {
            var session = new Session(sessionId);

            lock (Sessions)
                Sessions[session.Id] = session;
        }

        public void DeleteSession (byte[] sessionId)
        {
            lock (Sessions)
                Sessions.Remove(sessionId);
        }

        public Session GetSessionById (byte[] sessionId)
        {
            lock (Sessions)
            {
                return Sessions.FirstOrDefault(s => s.Key == sessionId).Value;
            }
        }
    }
}
