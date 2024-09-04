using System.Collections;
using System.Collections.Generic;

namespace DevConsole.Commands
{
    public abstract class DevConsoleCommand
    {
        /// <summary>
        /// This command can only be ran during dev mode.
        /// </summary>
        public virtual bool devModeOnly { get; }
        
        /// <summary>
        /// This command can only be ran during cheat mode.
        /// </summary>
        public virtual bool cheatModeOnly { get; }
        
        /// <summary>
        /// Gets a list of all names this console command could be recognized by.
        /// </summary>
        public abstract string[] GetNames();

        /// <summary>
        /// Gets a string with helpful information about this command.
        /// </summary>
        public virtual string GetHelp() => "none"; 
        
        /// <summary>
        /// Called when this command is executed by the console.
        /// </summary>
        /// <param name="parameters">All parameters from the console input which have been separated by spaces.</param>
        public abstract void Execute(List<string> parameters);
        
        /// <summary>
        /// Called when this command is executed by the console.
        /// </summary>
        /// <param name="parameters">All parameters from the console input which have been separated by spaces.</param>
        public virtual IEnumerator ExecuteAsync(List<string> parameters)
        {
            yield break;
        }
    }
}