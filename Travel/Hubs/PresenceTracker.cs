namespace TravelApi.Hubs
{
    public class PresenceTracker
    {
        // userId → список connectionId (один юзер может быть с нескольких вкладок)
        private static readonly Dictionary<int, HashSet<string>> _connections = new();
        private static readonly object _lock = new();

        public void UserConnected(int userId, string connectionId)
        {
            lock (_lock)
            {
                if (!_connections.ContainsKey(userId))
                    _connections[userId] = new HashSet<string>();
                _connections[userId].Add(connectionId);
            }
        }

        public void UserDisconnected(int userId, string connectionId)
        {
            lock (_lock)
            {
                if (_connections.TryGetValue(userId, out var conns))
                {
                    conns.Remove(connectionId);
                    if (conns.Count == 0)
                        _connections.Remove(userId);
                }
            }
        }

        public bool IsOnline(int userId)
        {
            lock (_lock)
                return _connections.ContainsKey(userId);
        }

        public List<int> GetOnlineUsers()
        {
            lock (_lock)
                return _connections.Keys.ToList();
        }
    }
}
