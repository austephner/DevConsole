using System;
using System.Collections.Generic;
using DevConsole.Runtime.Behaviours;
using DevConsole.Runtime.Enums;
using UnityEngine;

namespace DevConsole.Runtime.Examples
{
    /// <summary>
    /// Uses the old standard Unity input manager to get key-down state information. Very basic and rudimentary but
    /// works for most cases.
    /// </summary>
    public class BasicConsoleInputBehaviour : ConsoleInputBehaviour
    {
        [SerializeField] private InputMapping
            _toggle = new InputMapping("Toggle (Open & Close)", DevConsoleBehaviourCommand.Toggle) { keyCodes = new KeyCode[] { KeyCode.LeftShift, KeyCode.Tilde }},
            _submit = new InputMapping("Submit", DevConsoleBehaviourCommand.Submit) { keyCodes = new KeyCode[] { KeyCode.Return }},
            _close = new InputMapping("Close", DevConsoleBehaviourCommand.Close) { keyCodes = new KeyCode[] { KeyCode.Escape }},
            _clear = new InputMapping("Clear", DevConsoleBehaviourCommand.Clear) { keyCodes = new KeyCode[] { KeyCode.LeftControl, KeyCode.Backspace }},
            _goBackInHistory = new InputMapping("History (up)", DevConsoleBehaviourCommand.GoBackInHistory) { keyCodes = new KeyCode[] { KeyCode.UpArrow }},
            _goForwardInHistory = new InputMapping("History (down)", DevConsoleBehaviourCommand.GoForwardInHistory) { keyCodes = new KeyCode[] { KeyCode.DownArrow }};

        private List<InputMapping> _inputMappings = new List<InputMapping>();

        private void Awake()
        {
            _inputMappings.Add(_toggle);
            _inputMappings.Add(_submit);
            _inputMappings.Add(_close);
            _inputMappings.Add(_clear);
            _inputMappings.Add(_goBackInHistory);
            _inputMappings.Add(_goForwardInHistory);
        }

        public override DevConsoleBehaviourCommand GetNextCommand()
        {
            foreach (var inputMapping in _inputMappings)
            {
                if (inputMapping.disableTimer > 0)
                {
                    inputMapping.disableTimer -= Time.unscaledDeltaTime;
                    continue;
                }

                if (inputMapping.Check())
                {
                    inputMapping.disableTimer = inputMapping.disableAfterPressTimer;
                    return inputMapping.command;
                }
            }

            return DevConsoleBehaviourCommand.None;
        }

        [Serializable]
        private class InputMapping
        {
            [HideInInspector] public readonly string name = "";
            [HideInInspector] public readonly DevConsoleBehaviourCommand command;
            [HideInInspector] public float disableTimer = 0.0f;

            [Tooltip("How long this input mapping is disabled for after it's successfully been used. Helpful for stopping spamming.")]
            public float disableAfterPressTimer = 0.5f;

            public KeyCode[] keyCodes;

            public bool Check()
            {
                if (disableTimer > 0)
                {
                    return false;
                }
                
                foreach (var keyCode in keyCodes)
                {
                    if (!Input.GetKey(keyCode))
                    {
                        return false;
                    }
                }

                return true;
            }

            public InputMapping(string name, DevConsoleBehaviourCommand command)
            {
                this.name = name;
                this.command = command;
            }
        }
    }
}