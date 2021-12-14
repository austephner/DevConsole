using System.Collections.Generic;
using DevConsole.Runtime.Behaviours;

namespace DevConsole.Runtime.Commands
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
                    DevConsoleBehaviour.Instance.Print($"{string.Join(", ", command.GetNames())} --> <i> {command.GetHelp()}</i>");
                }
            }
            else
            {
                DevConsoleBehaviour.Instance.Print(DevConsoleBehaviour.Instance.GetCommandByName(parameters[0]).GetHelp());
            }
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
}