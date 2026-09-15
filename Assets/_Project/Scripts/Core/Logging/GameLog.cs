using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace BoomPG.Core.Logging
{
    /// <summary>로그 발생 주체를 구분한다.</summary>
    public enum LogRole { Unknown, Host, Client }

    /// <summary>빌드 종류와 역할에 따라 로그 출력을 통일한다.</summary>
    public static class GameLog
    {
        private static string _role = "?";

        /// <summary>세션의 로그 역할을 설정한다.</summary>
        public static void SetRole(LogRole role)
        {
            _role = role == LogRole.Host ? "H" : role == LogRole.Client ? "C" : "?";
        }

        /// <summary>코어 진단 메시지를 출력한다.</summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEBUG")]
        public static void Core(string message) => Debug.Log(Format("Core", message));

        /// <summary>전투 진단 메시지를 출력한다.</summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEBUG")]
        public static void Combat(string message) => Debug.Log(Format("Combat", message));

        /// <summary>네트워크 진단 메시지를 출력한다.</summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEBUG")]
        public static void Net(string message) => Debug.Log(Format("Net", message));

        /// <summary>UI 진단 메시지를 출력한다.</summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEBUG")]
        public static void UI(string message) => Debug.Log(Format("UI", message));

        /// <summary>릴리즈에서도 확인할 경고를 출력한다.</summary>
        public static void Warn(string category, string message) => Debug.LogWarning(Format(category, message));

        /// <summary>릴리즈에서도 확인할 오류를 출력한다.</summary>
        public static void Error(string category, string message) => Debug.LogError(Format(category, message));

        private static string Format(string category, string message) => $"[{_role}][{category}] {message}";
    }
}
