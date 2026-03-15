using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace UFUAServerBase.AuditTrace
{
    internal enum AuditState : short
    {
        None,
        WaitingForUser,
        WaitingForPassword,
        WaitingForLevel,
        Completed
    }

    internal class AuditTraceToken
    {
        #region Declarations
        static Dictionary<Guid, AuditTraceToken> tokens = new Dictionary<Guid, AuditTraceToken>();
        static List<Guid> sortedGuids = new List<Guid>();

        readonly Guid tokenId;
        readonly Timer cleanupTimer;
        #endregion

        #region Constructors
        public AuditTraceToken(AuditTraceToken token) 
            : this (token.tokenId, token.stateId)
        {
            userName = token.userName;
            password = token.password;
        }

        public AuditTraceToken(Guid tokenId) 
            : this (tokenId, AuditState.None)
        { }

        public AuditTraceToken(Guid tokenId, AuditState stateId)
        {
            this.tokenId = tokenId;
            this.stateId = stateId;

            var auditExpiringTime = Properties.Settings.Default.AuditExpiringTime;
            if (auditExpiringTime > 0)
                cleanupTimer = new Timer(new TimerCallback(OnCleanup), tokenId, TimeSpan.FromMilliseconds(auditExpiringTime), TimeSpan.FromMilliseconds(-1));
        }
        #endregion

        #region Public Properties
        public Guid TokenId
        {
            get
            {
                return tokenId;
            }
        }

        string userName;
        public String UserName
        {
            get
            {
                return userName;
            }
            set
            {
                if (userName == value)
                    return;

                userName = value;
            }
        }

        string password;
        public String Password
        {
            get
            {
                return password;
            }
            set
            {
                if (password == value)
                    return;

                password = value;
            }
        }

        AuditState stateId;
        public AuditState StateId
        {
            get
            {
                return stateId;
            }
            set
            {
                if (stateId == value)
                    return;

                stateId = value;
            }
        }
        #endregion

        #region Static Methods
        public static AuditTraceToken RestoreToken(string token)
        {
            Guid guid;
            if (!Guid.TryParse(token, out guid))
                return null;

            return RestoreToken(guid);
        }

        public static AuditTraceToken RestoreToken(Guid guid)
        {
            lock (tokens)
            {
                AuditTraceToken auditTraceToken = null;
                if (tokens.ContainsKey(guid))
                {
                    auditTraceToken = tokens[guid];
                    RemoveToken(guid);
                }

                sortedGuids.Add(guid);
                if (auditTraceToken != null)
                    tokens.Add(guid, new AuditTraceToken(auditTraceToken));
                else
                    tokens.Add(guid, new AuditTraceToken(guid));
                
                if (sortedGuids.Count > 1 &&
                    sortedGuids.Count > Properties.Settings.Default.AuditMaxTokenNum)
                    RemoveToken(sortedGuids[0]);

                return tokens[guid];
            }
        }

        public static bool RemoveToken(string token)
        {
            Guid guid;
            if (!Guid.TryParse(token, out guid))
                return false;

            return RemoveToken(guid);
        }

        public static bool RemoveToken(Guid guid)
        {
            lock (tokens)
            {
                if (!tokens.ContainsKey(guid))
                    return false;

                var auditTraceToken = tokens[guid];
                if (auditTraceToken.cleanupTimer != null)
                    auditTraceToken.cleanupTimer.Dispose();
                sortedGuids.Remove(guid);
                tokens.Remove(guid);
                return true;
            }
        }

        public static void ClearAllTokens()
        {
            lock (tokens)
            {
                var guids = tokens.Keys.ToList();
                foreach (var guid in guids)
                    RemoveToken(guid);
            }
        }
        #endregion

        #region Private Methods
        void OnCleanup(object state)
        {
            var token = (Guid)state;
            RemoveToken(token);
        }
        #endregion
    }
}
