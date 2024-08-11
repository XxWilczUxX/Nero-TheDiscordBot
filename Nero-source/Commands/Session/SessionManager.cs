using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nero.Commands.Session;

public class SessionManager
{
    private readonly Dictionary<ulong, List<ulong>> _userSessions = new Dictionary<ulong, List<ulong>>();
    private readonly int _maxSessionsPerUser;

    public SessionManager(int maxSessionsPerUser)
        => _maxSessionsPerUser = maxSessionsPerUser;

    public bool CanCreateSession(ulong userId)
    {
        if (_userSessions.TryGetValue(userId, out var sessions))
            return sessions.Count < _maxSessionsPerUser;

        return true;
    }

    public void AddSession(ulong userId, ulong sessionId)
    {
        if (!_userSessions.ContainsKey(userId))
        {
            _userSessions[userId] = new List<ulong>();
        }

        _userSessions[userId].Add(sessionId);
    }

    public void RemoveSession(ulong userId, ulong sessionId)
    {
        if (_userSessions.ContainsKey(userId))
        {
            _userSessions[userId].Remove(sessionId);
        }
    }

    public int GetSessionCount(ulong userId)
    {
        return _userSessions.ContainsKey(userId) ? _userSessions[userId].Count : 0;
    }
}
