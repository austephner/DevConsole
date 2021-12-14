using System;
using System.Collections.Generic;
using DevConsole.Behaviours;
using DevConsole.Enums;

namespace DevConsole.Commands
{
    /// <summary>
    /// Prints all parameterized text to the console.
    /// </summary>
    public class EchoCommand : DevConsoleCommand
    {
        public override string[] GetNames()
        {
            return new string[] {"echo"};
        }

        public override string GetHelp()
        {
            return "Takes input and \"echoes\" it back to the console.";
        }

        public override void Execute(List<string> parameters)
        {
            DevConsoleBehaviour.Instance.Print(string.Join(" ", parameters));
        }
    }
    
    /// <summary>
    /// Displays help for the given command.
    /// </summary>
    public class HelpCommand : DevConsoleCommand
    {
        public override string[] GetNames()
        {
            return new string[] { "h", "help" };
        }

        public override string GetHelp()
        {
            return "Provides help, info, documentation, etc. about the given command.";
        }

        public override void Execute(List<string> parameters)
        {
            if (parameters.Count == 0)
            {
                foreach (var command in DevConsoleBehaviour.Instance.GetAllRegisteredCommands())
                {
                    if (command.cheatModeOnly && !DevConsoleBehaviour.Instance.isCheatModeEnabled)
                    {
                        continue;
                    }

                    if (command.devModeOnly && !DevConsoleBehaviour.Instance.isDevModeEnabled)
                    {
                        continue;
                    }

                    var help = command.GetHelp();

                    if (string.IsNullOrWhiteSpace(help))
                    {
                        continue;
                    }
                    
                    DevConsoleBehaviour.Instance.Print($"{string.Join(", ", command.GetNames())} --> <i> {help}</i>");
                }
            }
            else
            {
                var command = DevConsoleBehaviour.Instance.GetCommandByName(parameters[0]);

                if (command == null || 
                    command.cheatModeOnly && !DevConsoleBehaviour.Instance.isCheatModeEnabled ||
                    command.devModeOnly && !DevConsoleBehaviour.Instance.isDevModeEnabled)
                {
                    PrintNotAvailable();
                    return;
                }
                
                DevConsoleBehaviour.Instance.Print(command.GetHelp());
            }
        }

        private void PrintNotAvailable()
        {
            DevConsoleBehaviour.Instance.Print("Help for this command is not available.", DevConsolePrintType.Error);
        }
    }

    /// <summary>
    /// Clears the console.
    /// </summary>
    public class ClearCommand : DevConsoleCommand
    {
        public override string[] GetNames()
        {
            return new string[] { "c", "cls", "clr", "clear" };
        }

        public override string GetHelp()
        {
            return "Clears the console of all text.";
        }

        public override void Execute(List<string> parameters)
        {
            DevConsoleBehaviour.Instance.Clear();
        }
    }

    /// <summary>
    /// Displays <see cref="DateTime.Now"/>
    /// </summary>
    public class TimeCommand : DevConsoleCommand
    {
        public override string[] GetNames()
        {
            return new string[] { "time" };
        }

        public override void Execute(List<string> parameters)
        {
            DevConsoleBehaviour.Instance.Print(DateTime.Now.ToString("g"));
        }
    }
}