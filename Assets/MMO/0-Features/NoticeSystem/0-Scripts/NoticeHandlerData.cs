using System;
using JetBrains.Annotations;

namespace MMO
{
    public static class NoticeHandlerData
    {
        public static event Action<string, string, string, Action?> C_OnDisplayNotice;
        public static void C_DisplayNotice(string title, string description, string rewards, [CanBeNull] Action onAccept = null) =>
            C_OnDisplayNotice?.Invoke(title, description, rewards, onAccept);
        
        public static event Action<string, string, string, Action?> C_OnDisplayCompleteNotice;
        public static void C_DisplayCompleteNotice(string title, string description, string rewards, [CanBeNull] Action onAccept = null) =>
            C_OnDisplayCompleteNotice?.Invoke(title, description, rewards, onAccept);
        
        public static event Action<int, string, string> S_OnDisplayNotice;
        public static void S_DisplayNotice(int clientId, string title, string description) =>
            S_OnDisplayNotice?.Invoke(clientId, title, description);
    }
}
