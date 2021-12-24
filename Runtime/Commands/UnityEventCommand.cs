using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DevConsole.Commands
{
    [Serializable, NonLoadableCommand]
    public class UnityEventCommand : DevConsoleCommand
    {
        [SerializeField,
         Tooltip("Names that can be used to invoke this command. All entries are set to lowercase and trimmed before comparison.")] 
        private string[] _names;
        
        [SerializeField,
         Tooltip("The text that appears when a user types \"help [your-command-name]\"")] 
        private string _helpText;
        
        [SerializeField,
         Tooltip("Enable if this command should only be used during \"Dev Mode\".")] 
        private bool _devModeOnly;
        
        [SerializeField,
         Tooltip("Enable if this command should only be used during \"Cheat Mode\".")] 
        private bool _cheatModeOnly; 
        
        [SerializeField,
         Tooltip("Invoked when this command is executed. It'll be invoked before \"onParameterInvoke\".")] 
        private UnityEvent _onInvoke;
        
        [SerializeField,
         Tooltip("Invoked when this command is executed. It's invoked with all parameters sent to the command and invoked after \"onInvoke\".")] 
        private StringParamUnityEvent _onParameterInvoke;

        public override string[] GetNames() => _names;

        public override string GetHelp() => _helpText;

        public override bool devModeOnly => _devModeOnly;

        public override bool cheatModeOnly => _cheatModeOnly;

        public override void Execute(List<string> parameters)
        {
            _onInvoke?.Invoke();
            _onParameterInvoke?.Invoke(parameters);
        }
    }
    
    [Serializable]
    public class StringParamUnityEvent : UnityEvent<List<string>> { }
}