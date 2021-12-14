using System;
using System.Collections.Generic;
using System.Linq;
using DevConsole.Runtime.Commands;
using DevConsole.Runtime.CustomEvents;
using DevConsole.Runtime.Enums;
using DevConsole.Runtime.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace DevConsole.Runtime.Behaviours
{
    public class DevConsoleBehaviour : MonoBehaviour
    {
        #region Events

        public static event Action 
            onInitialized,
            onShutdown,
            onOpen,
            onClosed,
            onClear;

        public static event Action<string> 
            onPrint,
            onInputBufferChanged;

        public static event Action<bool>
            onDevModeStateChanged,
            onCheatModeStateChanged;

        #endregion
        
        #region Settings

        [Header("General"), SerializeField] 
        protected bool _logUnityEventsToConsole;

        [SerializeField]
        protected bool _startsOpen;
        
        [SerializeField]
        protected bool _openOnStart;

        [SerializeField] 
        protected bool _logCommandsToConsole;

        [SerializeField] 
        protected bool _logUnityApplicationMessages;
        
        [SerializeField,
        Tooltip("If the user attempts to submit a command that doesn't exist, the console will print an error message.")] 
        protected bool _showCommandDoesntExistError = true;

        [SerializeField,
        Tooltip("When enabled the console will clear the input buffer after \"Submit()\" is called.")] 
        protected bool _clearInputBufferAfterSubmit = true;
        
        [SerializeField,
        Tooltip("This enables the ability to use \"dev mode only\" commands.")] 
        protected bool _allowDevModeCommands = true;
        
        [SerializeField,
        Tooltip("Enables \"Dev Mode\" for this console when the game object starts. \"AllowDevModeCommands\" must be true in order for it to work.")] 
        protected bool _enableDevModeOnStart;

        [SerializeField,
         Tooltip("This enables the ability to use \"cheat mode only\" commands.")] 
        protected bool _allowCheatModeCommands = false;
        
        [SerializeField,
         Tooltip("Enables \"Cheat Mode\" for this console when the game object starts. \"AllowCheatModeCommands\" must be true in order for it to work.")] 
        protected bool _enableCheatModeOnStart;
        
        [SerializeField] 
        protected float _maxHistory = 100;

        [Header("Components"), SerializeField,
        Tooltip("Determines how input is handled when updating this console.")] 
        protected ConsoleInputBehaviour _consoleInputBehaviour;

        [SerializeField,
        Tooltip("Determines how printing and clearing text is handled for this console.")] 
        protected ConsoleDisplayBehaviour _consoleDisplayBehaviour;

        [Header("Events"), SerializeField] 
        protected UnityEvent _onInitialized;

        [SerializeField] 
        protected UnityEvent
            _onShutdown,
            _onOpen,
            _onClosed,
            _onToggle,
            _onUpdate,
            _onClear;

        [SerializeField] 
        protected StringUnityEvent 
            _onSubmit,
            _onPrint,
            _onInputBufferChanged;

        #endregion

        #region Constants

        protected const string 
            DEBUG_LOG_FORMAT = "<b>DevConsole</b> :: {0} :: {1}",
            UNITY_EVENT_INITIALIZE = "Initializing developer console.",
            UNITY_EVENT_INIT_FAILED = "Failed to initialize, a developer console already exists.",
            UNITY_EVENT_INIT_SUCCESS = "Successfully initialized developer console.", 
            UNITY_EVENT_DISABLE = "Shutting down developer console.",
            ERROR_COMMAND_DOESNT_EXIST = "\"{0}\" is not a command.",
            LOG_COMMAND = "Executing command named \"{0}\" with arguments \"{1}\"",
            DEV_MODE_ONLY = "Please enable dev mode to use this command.",
            DEV_MODE_DISABLED = "Cannot set dev mode, its usage has been disabled.",
            CHEAT_MODE_ONLY = "Cheats not allowed.",
            CHEAT_MODE_DISABLED = "Cannot set cheat mode, its usage has been disabled.";

        #endregion

        #region Properties

        public static DevConsoleBehaviour Instance { get; private set; }

        public string inputBuffer
        {
            get => _inputBuffer;
            set
            {
                _inputBuffer = value;
                onInputBufferChanged?.Invoke(value);
                _onInputBufferChanged?.Invoke(value);
            }
        }

        public bool isOpen => _open;

        public bool isClosed => !isOpen;

        public bool devMode => _devMode;

        public bool cheatMode => _cheatMode;

        #endregion

        #region Protected Fields

        protected bool _open, _cheatMode, _devMode;

        protected List<DevConsoleCommand> _devConsoleCommands = new List<DevConsoleCommand>();

        protected List<string> _inputHistory = new List<string>();

        protected int _historyPosition = 0;

        protected string _inputBuffer;

        #endregion

        #region Unity Events

        private void OnEnable()
        {
            if (_logUnityEventsToConsole) DebugLogToConsole(UNITY_EVENT_INITIALIZE, "OnEnable");

            if (Instance && Instance != this)
            {
                if (_logUnityEventsToConsole) DebugLogToConsole(UNITY_EVENT_INIT_FAILED, "OnEnable");
                DestroyImmediate(gameObject);
                return;
            }

            if (_logUnityEventsToConsole) DebugLogToConsole(UNITY_EVENT_INIT_SUCCESS, "OnEnable");
            
            Instance = this;

            _devConsoleCommands = 
                TypeUtil.GetNonAbstractSubTypes(typeof(DevConsoleCommand))
                    .Select(devConsoleCommand => (DevConsoleCommand) Activator.CreateInstance(devConsoleCommand))
                    .ToList();

            if (_startsOpen) _open = true;
            if (_openOnStart) Open();
            if (_enableCheatModeOnStart) SetCheatMode(true);
            if (_enableDevModeOnStart) SetDevMode(true);
            
            OnInitialize();
            onInitialized?.Invoke();
            _onInitialized?.Invoke();
        }

        private void OnDisable()
        {
            if (Instance == this)
            {
                if (_logUnityEventsToConsole) DebugLogToConsole(UNITY_EVENT_DISABLE, "OnDisable");
                
                OnShutdown();
                onShutdown?.Invoke();
                _onShutdown?.Invoke();
                
                Instance = null; 
            }
        }

        private void Update()
        {
            Update(_consoleInputBehaviour?.GetNextCommand() ?? DevConsoleBehaviourCommand.None);
            OnUpdate();
            _onUpdate?.Invoke();
        }

        #endregion

        #region Protected Events

        /// <summary>
        /// Invoked on the <see cref="Instance"/> when the <see cref="DevConsoleBehaviour"/> is enabled in <see cref="OnEnable"/>.
        /// </summary>
        protected virtual void OnInitialize() { }
        
        /// <summary>
        /// Invoked on the <see cref="Instance"/> when the <see cref="DevConsoleBehaviour"/> is disabled in <see cref="OnDisable"/>.
        /// </summary>
        protected virtual void OnShutdown() { }
        
        /// <summary>
        /// Invoked on the <see cref="Instance"/> when <see cref="Update"/> is called.
        /// </summary>
        protected virtual void OnUpdate() { }
        
        /// <summary>
        /// Invoked when the "open" state of the <see cref="Instance"/> changed.
        /// </summary>
        protected virtual void OnOpenClosedStateChanged() { }
        
        /// <summary>
        /// Invoked when <see cref="Clear"/> is called on the <see cref="Instance"/>.
        /// </summary>
        protected virtual void OnClear() { }
        
        /// <summary>
        /// Invoked when the <see cref="Instance"/> has successfully parsed text and has already executed some
        /// <see cref="DevConsoleCommand"/> with the given <see cref="arguments"/>.
        /// </summary>
        /// <param name="command">The command that has been executed.</param>
        /// <param name="arguments">Any arguments that were used while executing the command.</param>
        protected virtual void OnCommandExecuted(DevConsoleCommand command, List<string> arguments) { }

        #endregion

        #region Protected Utilities

        protected void DebugLogToConsole(string message, string function)
        {
            Debug.Log(string.Format(DEBUG_LOG_FORMAT, function, message));
        }

        /// <summary>
        /// Parses the text for any commands/arguments.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        protected virtual void ParseText(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                _inputHistory.Add(text);

                if (_inputHistory.Count > _maxHistory)
                {
                    _consoleDisplayBehaviour?.RemoveHistoryAt(0);
                    _inputHistory.RemoveAt(0);
                }

                _historyPosition = _inputHistory.Count - 1;
            }
            else
            {
                return;
            }

            var formattedInput = text.Trim().Split(' ');
            
            if (formattedInput.Length > 0)
            {
                var commandText = formattedInput[0];
                var arguments = formattedInput.Skip(1).Take(formattedInput.Length - 1).ToList();
                var command = GetCommandByName(commandText);

                if (command != null)
                {
                    if (command.cheatModeOnly && (!_allowCheatModeCommands || !_cheatMode))
                    {
                        Print(CHEAT_MODE_ONLY, DevConsolePrintType.Error);
                        return;
                    }

                    if (command.devModeOnly && (!_allowDevModeCommands || !_devMode))
                    {
                        Print(DEV_MODE_ONLY, DevConsolePrintType.Error);
                        return;
                    }
                    
                    if (_logCommandsToConsole) DebugLogToConsole(string.Format(LOG_COMMAND, commandText, string.Join(",", arguments)), "ParseText");
                    command.Execute(arguments);
                    OnCommandExecuted(command, arguments);
                }
                else if (_showCommandDoesntExistError)
                {
                    Print(string.Format(ERROR_COMMAND_DOESNT_EXIST, commandText), DevConsolePrintType.Error);
                }
            }
            else
            {
                Print("");
            }
        }

        #endregion

        #region Public Utilities

        /// <summary>
        /// Updates this <see cref="DevConsoleBehaviour"/> over the given <see cref="DevConsoleBehaviourCommand"/>.
        /// </summary>
        /// <param name="command">The <see cref="DevConsoleBehaviourCommand"/> to update with.</param>
        public void Update(DevConsoleBehaviourCommand command)
        {
            switch (command)
            {
                case DevConsoleBehaviourCommand.Open:
                    Open();
                    break;
                case DevConsoleBehaviourCommand.Close:
                    Close();
                    break;
                case DevConsoleBehaviourCommand.Toggle:
                    Toggle();
                    break;
                case DevConsoleBehaviourCommand.Submit:
                    Submit();
                    break;
                case DevConsoleBehaviourCommand.Clear:
                    Clear();
                    break;
                case DevConsoleBehaviourCommand.GoBackInHistory:
                    if (_inputHistory.Count > 0)
                    {
                        _historyPosition = Mathf.Clamp(_historyPosition - 1, 0, _inputHistory.Count - 1);
                        inputBuffer = _inputHistory[_historyPosition];
                    }
                    break;
                case DevConsoleBehaviourCommand.GoForwardInHistory:
                    if (_inputHistory.Count > 0)
                    {
                        _historyPosition = Mathf.Clamp(_historyPosition + 1, 0, _inputHistory.Count - 1);
                        inputBuffer = _inputHistory[_historyPosition];
                    }
                    break;
            }
        }

        /// <summary>
        /// Opens the <see cref="DevConsoleBehaviour"/>.
        /// </summary>
        public void Open()
        {
            if (_open) return;
            
            _open = true;
            OnOpenClosedStateChanged();
            onOpen?.Invoke();
            _onOpen?.Invoke();
        }

        /// <summary>
        /// Closes the <see cref="DevConsoleBehaviour"/>.
        /// </summary>
        public void Close()
        {
            if (!_open) return;
            
            _open = false;
            OnOpenClosedStateChanged();
            onClosed?.Invoke();
            _onClosed?.Invoke();
        }

        /// <summary>
        /// Toggles the open/close state of the <see cref="DevConsoleBehaviour"/>.
        /// </summary>
        public void Toggle()
        {
            if (_open) Close();
            else Open();
            _onToggle?.Invoke();
        }

        /// <summary>
        /// Processes the text in <see cref="inputBuffer"/>.
        /// </summary>
        public void Submit()
        {
            Submit(inputBuffer);

            if (_clearInputBufferAfterSubmit) inputBuffer = "";
        }

        /// <summary>
        /// Processes text by parsing it for commands and arguments.
        /// </summary>
        /// <param name="text">The text to process.</param>
        public void Submit(string text)
        {
            ParseText(text);
            _onSubmit?.Invoke(text);
        }

        /// <summary>
        /// Clears the command history.
        /// </summary>
        public void Clear()
        {
            _consoleDisplayBehaviour?.Clear();
            _historyPosition = 0;
            _inputHistory.Clear();
            OnClear();
            onClear?.Invoke();
            _onClear?.Invoke();
        }

        /// <summary>
        /// Prints the given <see cref="text"/>.
        /// </summary>
        /// <param name="text">The text to print.</param>
        public void Print(string text)
        {
            _consoleDisplayBehaviour?.Print(text, DevConsolePrintType.Info);
            onPrint?.Invoke(text);
            _onPrint?.Invoke(text);
        }
        
        /// <summary>
        /// Prints the given <see cref="text"/> using the given <see cref="DevConsolePrintType"/>.
        /// </summary>
        /// <param name="text">The text to print.</param>
        /// <param name="devConsolePrintType">The <see cref="DevConsolePrintType"/> to style or format the <see cref="text"/> with.</param>
        public void Print(string text, DevConsolePrintType devConsolePrintType)
        {
            _consoleDisplayBehaviour?.Print(text, devConsolePrintType);
            onPrint?.Invoke(text);
            _onPrint?.Invoke(text);
        }
        
        /// <summary>
        /// Retrieves a command matching the given command name.
        /// </summary>
        /// <param name="searchCommandName">The text to search a command for.</param>
        /// <returns>The first or default command found that matches the name.</returns>
        public DevConsoleCommand GetCommandByName(string searchCommandName)
        {
            return _devConsoleCommands.FirstOrDefault(devConsoleCommand =>
                devConsoleCommand.GetNames()
                    .Any(commandName => commandName.ToLower() == searchCommandName.ToLower().Trim()));
        }

        public List<DevConsoleCommand> GetAllRegisteredCommands() => _devConsoleCommands;

        public void SetCheatMode(bool cheatMode)
        {
            if (!_allowCheatModeCommands)
            {
                if (_logUnityEventsToConsole) Debug.LogError(CHEAT_MODE_DISABLED);

                if (_cheatMode)
                {
                    _cheatMode = false;
                    onCheatModeStateChanged?.Invoke(false);
                }
                return;
            }

            _cheatMode = cheatMode;
            onCheatModeStateChanged?.Invoke(cheatMode);
        }

        public void SetDevMode(bool devMode)
        {
            if (!_allowDevModeCommands)
            {
                if (_logUnityEventsToConsole) Debug.LogError(DEV_MODE_DISABLED);
                
                if (_devMode)
                {
                    _devMode = false;
                    onDevModeStateChanged?.Invoke(false);
                }
                return;
            }

            _devMode = devMode;
            onDevModeStateChanged?.Invoke(devMode);
        }
        
        #endregion
    }
}