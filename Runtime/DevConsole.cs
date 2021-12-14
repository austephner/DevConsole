using DevConsole.Behaviours;
using DevConsole.Enums;

namespace DevConsole
{
    /// <summary>
    /// Allows quick and easy access to <see cref="DevConsoleBehaviour"/> instance functionality.
    /// </summary>
    public static class DevConsole
    {
        public static void Print(string message)
        {
            DevConsoleBehaviour.Instance?.Print(message);
        }

        public static void Print(string message, DevConsolePrintType printType)
        {
            DevConsoleBehaviour.Instance?.Print(message, printType);
        }

        public static void PrintError(string message)
        {
            DevConsoleBehaviour.Instance?.Print(message, DevConsolePrintType.Error);
        }

        public static void PrintWarning(string message)
        {
            DevConsoleBehaviour.Instance?.Print(message, DevConsolePrintType.Warning);
        }
        
        public static void PrintSuccess(string message)
        {
            DevConsoleBehaviour.Instance?.Print(message, DevConsolePrintType.Success);
        }

        public static void Toggle()
        {
            DevConsoleBehaviour.Instance?.Toggle();
        }

        public static void Open()
        {
            DevConsoleBehaviour.Instance?.Open();
        }

        public static void Close()
        {
            DevConsoleBehaviour.Instance?.Close();
        }

        public static bool isOpen => DevConsoleBehaviour.Instance?.isOpen ?? false;

        public static bool isDevModeEnabled => DevConsoleBehaviour.Instance?.isDevModeEnabled ?? false;

        public static bool isCheatModeEnabled => DevConsoleBehaviour.Instance?.isCheatModeEnabled ?? false;
    }
}