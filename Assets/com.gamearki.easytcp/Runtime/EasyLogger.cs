namespace GameArki.EasyTcp.Logger {

    internal static class EasyLogger {

        internal static void Log(string text) {
#if UNITY_EDITOR
            UnityEngine.Debug.Log(text);
#else
            Console.WriteLine(text);
#endif
        }

    }

}
