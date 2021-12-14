using System;
using System.Collections.Generic;
using System.Linq;
using DevConsole.Commands;
using DevConsole.CustomEvents;
using DevConsole.Enums;
using DevConsole.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace DevConsole.Behaviours
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

        [Header("General"), SerializeField, FormerlySerializedAs("_logUnityEventsToConsole"),
        Tooltip("When enabled, this DevConsoleBehaviour will regularly log information to the console such as " +
                "exceptions, invocations, etc.")] 
        protected bool _debug;

        [SerializeField,
        Tooltip("Enable if this DevConsoleBehaviour is considered open when the game starts.")]
        protected bool _startsOpen;
        
        [SerializeField,
        Tooltip("When enabled, \"Open()\" will be called when this DevConsoleBehaviour starts and" +
                " automatically open.")]
        protected bool _openOnStart;

        [SerializeField,
        Tooltip("When enabled, Unity console logs will appear in this DevConsoleBehaviour. Avoid toggling this field during runtime.")] 
        protected bool _printUnityConsoleLogs;
        
        [SerializeField,
        Tooltip("If the user attempts to submit a command that doesn't exist, the console will print an " +
                "error message.")] 
        protected bool _showCommandDoesntExistError = true;

        [SerializeField,
        Tooltip("When enabled the console will clear the input buffer after \"Submit()\" is called.")] 
        protected bool _clearInputBufferAfterSubmit = true;
        
        [SerializeField,
        Tooltip("Allows the usage of \"Dev Mode\". When disabled, \"SetDevMode(...)\" cannot be called and " +
                "dev-mode-only commands cannot be executed..")] 
        protected bool _allowDevMode = true;
        
        [SerializeField,
        Tooltip("When enabled, this DevConsoleBehaviour will enter \"Dev Mode\" on start if possible.")] 
        protected bool _enableDevModeOnStart;

        [FormerlySerializedAs("_allowCheatModeCommands"), SerializeField,
         Tooltip("Allows the usage of \"Cheat Mode\". When disabled, \"SetCheatMode(...)\" cannot be called and" +
                 "cheat-mode-only commands cannot be executed.")] 
        protected bool _allowCheatMode = false;
        
        [SerializeField,
         Tooltip("When enabled, this DevConsoleBehaviour will enter \"Cheat Mode\" on start if possible.")] 
        protected bool _enableCheatModeOnStart;
        
        [SerializeField,
        Tooltip("The total number of entries the console will remember.")] 
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

        public bool isDevModeEnabled => _isDevModeEnabled;

        public bool isCheatModeEnabled => _isCheatModeEnabled;

        #endregion

        #region Protected Fields

        protected bool _open, _isCheatModeEnabled, _isDevModeEnabled;

        protected List<DevConsoleCommand> _devConsoleCommands = new List<DevConsoleCommand>();

        protected List<string> _inputHistory = new List<string>();

        protected int _historyPosition = 0;

        protected string _inputBuffer;

        #endregion

        #region Unity Events

        private void OnEnable()
        {
            if (_debug) DebugLogToConsole(UNITY_EVENT_INITIALIZE, "OnEnable");

            if (Instance && Instance != this)
            {
                if (_debug) DebugLogToConsole(UNITY_EVENT_INIT_FAILED, "OnEnable");
                DestroyImmediate(gameObject);
                return;
            }

            if (_debug) DebugLogToConsole(UNITY_EVENT_INIT_SUCCESS, "OnEnable");
            
            Instance = this;

            _devConsoleCommands = 
                TypeUtil.GetNonAbstractSubTypes(typeof(DevConsoleCommand))
                    .Select(devConsoleCommand => (DevConsoleCommand) Activator.CreateInstance(devConsoleCommand))
                    .ToList();

            if (_printUnityConsoleLogs) Application.logMessageReceived += OnUnityLog;
            if (_startsOpen) _open = true;
            if (_openOnStart && !_open) Open();
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
                if (_debug) DebugLogToConsole(UNITY_EVENT_DISABLE, "OnDisable");
                if (_printUnityConsoleLogs) Application.logMessageReceived -= OnUnityLog;
                
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

        /// <summary>
        /// Invoked when a Unity log is made.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        protected virtual void OnUnityLog(string condition, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                    Print(condition, DevConsolePrintType.Error);
                    break;
                case LogType.Assert:
                    Print(condition, DevConsolePrintType.Misc);
                    break;
                case LogType.Warning:
                    Print(condition, DevConsolePrintType.Warning);
                    break;
                case LogType.Log:
                    Print(condition);
                    break;
                case LogType.Exception:
                    Print(condition, DevConsolePrintType.Error);
                    break;
            }
        }

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
                    if (command.cheatModeOnly && (!_allowCheatMode || !_isCheatModeEnabled))
                    {
                        Print(CHEAT_MODE_ONLY, DevConsolePrintType.Error);
                        return;
                    }

                    if (command.devModeOnly && (!_allowDevMode || !_isDevModeEnabled))
                    {
                        Print(DEV_MODE_ONLY, DevConsolePrintType.Error);
                        return;
                    }
                    
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
            if (!_allowCheatMode)
            {
                if (_debug) Debug.LogError(CHEAT_MODE_DISABLED);

                if (_isCheatModeEnabled)
                {
                    _isCheatModeEnabled = false;
                    onCheatModeStateChanged?.Invoke(false);
                }
                
                return;
            }

            _isCheatModeEnabled = cheatMode;
            onCheatModeStateChanged?.Invoke(cheatMode);
        }

        public void SetDevMode(bool devMode)
        {
            if (!_allowDevMode)
            {
                if (_debug) Debug.LogError(DEV_MODE_DISABLED);
                
                if (_isDevModeEnabled)
                {
                    _isDevModeEnabled = false;
                    onDevModeStateChanged?.Invoke(false);
                }
                
                return;
            }

            _isDevModeEnabled = devMode;
            onDevModeStateChanged?.Invoke(devMode);
        }
        
        #endregion
    }
}