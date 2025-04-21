using UnityEngine;

public class ConsoleUtil
{
    public static void ClearConsole()
    {
        return;
        // This calls Unity's internal "ClearConsole" menu command
        ////var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
        ////var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        ////clearMethod?.Invoke(null, null);
    }
}
