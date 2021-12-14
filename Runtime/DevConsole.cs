using System.Collections.Generic;
using DevConsole.Behaviours;
using DevConsole.Commands;
using DevConsole.Enums;
using static DevConsole.Behaviours.DevConsoleBehaviour;

namespace DevConsole
{
    /// <summary>
    /// Allows quick and easy access to <see cref="DevConsoleBehaviour"/> instance functionality.
    /// </summary>
    public static class DevConsole
    {
        public static void Print(string message) => Instance?.Print(message);

        public static void Print(string message, DevConsolePrintType printType) => Instance?.Print(message, printType);

        public static void PrintError(string message) => Instance?.Print(message, DevConsolePrintType.Error);
        
        public static void PrintWarning(string message) => Instance?.Print(message, DevConsolePrintType.Warning);

        public static void PrintSuccess(string message) => Instance?.Print(message, DevConsolePrintType.Success);
        
        public static void Toggle() => Instance?.Toggle();

        public static void Open() => Instance?.Open();
        
        public static void Close() => Instance?.Close();

        public static void Clear() => Instance?.Clear();

        public static List<DevConsoleCommand> GetAllRegisteredCommands() => Instance?.GetAllRegisteredCommands();

        public static DevConsoleCommand GetCommandByName(string name) => Instance?.GetCommandByName(name);

        public static bool isOpen => Instance?.isOpen ?? false;

        public static bool isDevModeEnabled => Instance?.isDevModeEnabled ?? false;

        public static bool isCheatModeEnabled => Instance?.isCheatModeEnabled ?? false;

        public static string inputBuffer
        {
            get => Instance?.inputBuffer;
            set
            {
                if (Instance)
                {
                    Instance.inputBuffer = value;
                }
            }
        }
    }
}