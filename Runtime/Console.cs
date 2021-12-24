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
    public static class Console
    {
        /// <summary>
        /// Prints a message to the dev console, using the <see cref="DevConsoleBehaviour"/> <see cref="DevConsoleBehaviour.Instance"/>.
        /// </summary>
        /// <param name="message">The message to print.</param>
        public static void Print(string message) => Instance?.Print(message);

        /// <summary>
        /// Prints a message to the dev console, using the <see cref="DevConsoleBehaviour"/>
        /// <see cref="DevConsoleBehaviour.Instance"/> of the specified <see cref="DevConsolePrintType"/>.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="printType">he message type.</param>
        public static void Print(string message, DevConsolePrintType printType) => Instance?.Print(message, printType);

        /// <summary>
        /// Prints an error message to the dev console, using the <see cref="DevConsoleBehaviour"/> <see cref="DevConsoleBehaviour.Instance"/>.
        /// </summary>
        /// <param name="message">The error message to print.</param>
        public static void PrintError(string message) => Instance?.Print(message, DevConsolePrintType.Error);
        
        /// <summary>
        /// Prints a warning message to the dev console, using the <see cref="DevConsoleBehaviour"/> <see cref="DevConsoleBehaviour.Instance"/>.
        /// </summary>
        /// <param name="message">The warning message to print.</param>
        public static void PrintWarning(string message) => Instance?.Print(message, DevConsolePrintType.Warning);

        /// <summary>
        /// Prints a success message to the dev console, using the <see cref="DevConsoleBehaviour"/> <see cref="DevConsoleBehaviour.Instance"/>.
        /// </summary>
        /// <param name="message">The success message to print.</param>
        public static void PrintSuccess(string message) => Instance?.Print(message, DevConsolePrintType.Success);
        
        /// <summary>
        /// Toggles the <see cref="DevConsoleBehaviour"/> <see cref="DevConsoleBehaviour.Instance"/>.
        /// </summary>
        public static void Toggle() => Instance?.Toggle();

        /// <summary>
        /// Opens the <see cref="DevConsoleBehaviour"/> <see cref="DevConsoleBehaviour.Instance"/>.
        /// </summary>
        public static void Open() => Instance?.Open();
        
        /// <summary>
        /// Closes the <see cref="DevConsoleBehaviour"/> <see cref="DevConsoleBehaviour.Instance"/>.
        /// </summary>
        public static void Close() => Instance?.Close();

        /// <summary>
        /// Clears the <see cref="DevConsoleBehaviour"/> <see cref="DevConsoleBehaviour.Instance"/>.
        /// </summary>
        public static void Clear() => Instance?.Clear();

        /// <summary>
        /// Gets all registered commands from the <see cref="DevConsoleBehaviour"/> <see cref="DevConsoleBehaviour.Instance"/>.
        /// </summary>
        /// <returns><see cref="List{T}"/> of <see cref="DevConsoleCommand"/> registered in the <see cref="DevConsoleBehaviour"/> <see cref="DevConsoleBehaviour.Instance"/>.</returns>
        public static List<DevConsoleCommand> GetAllRegisteredCommands() => Instance?.GetAllRegisteredCommands();

        /// <summary>
        /// Gets a command matching the given <see cref="name"/>.
        /// </summary>
        /// <param name="name">The name of the command to get.</param>
        /// <returns>A <see cref="DevConsoleCommand"/> if one exists for the given name.</returns>
        public static DevConsoleCommand GetCommandByName(string name) => Instance?.GetCommandByName(name);

        /// <summary>
        /// Checks whether or not the <see cref="DevConsoleBehaviour"/> <see cref="DevConsoleBehaviour.Instance"/> is open.
        /// </summary>
        public static bool isOpen => Instance?.isOpen ?? false;

        /// <summary>
        /// Checks whether or not the <see cref="DevConsoleBehaviour"/> <see cref="DevConsoleBehaviour.Instance"/> has dev mode enabled.
        /// </summary>
        public static bool isDevModeEnabled => Instance?.isDevModeEnabled ?? false;

        /// <summary>
        /// Checks whether or not the <see cref="DevConsoleBehaviour"/> <see cref="DevConsoleBehaviour.Instance"/> has the cheat mode enabled.
        /// </summary>
        public static bool isCheatModeEnabled => Instance?.isCheatModeEnabled ?? false;

        /// <summary>
        /// The current input buffer for the <see cref="DevConsoleBehaviour"/> <see cref="DevConsoleBehaviour.Instance"/>.
        /// </summary>
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