# DevConsole
#### Summary
A simple in-game developer console with easy-to-implement commands and scripting.

![Example](https://i.imgur.com/hWwjmZl.gif)

#### Features
- Easily code new commands with no additional setup - just implement the command class!
- Some default/starter commands are included
- Modular and extensible components
- Tons of events to hook into. This includes `UnityEvent`, inheritable class events, and `static Action` events
- Navigable command and input history
- Working console prefab included, ready to be customized!
- Separate "Dev Mode" and "Cheat Mode" with the ability to specify commands that should only be ran during certain modes of the console

#### Todo
- Some functions/classes need additional documentation
- Scripting for executing a series of commands from a text file or string
- Configurable commands that can be created in the inspector with `UnityEvent` and require no coding.
- Control improvements in the example console.
- Improved README documentation to describe all settings/configurations/options for the console's game object.

# Getting Started
1. Import the package or Github content into your Assets folder
   - Get the HTTPS Github link to this repo and add it through the Package Manager
   - OR Clone the repo to a zip and extract the contents to your Assets folder
2. Drag and drop the "basic dev console" prefab from `Packages/DevConsole/Samples/Prefabs` into your scene
3. Add an "EventSystem" into your scene if one doesn't already exist. The UI for the basic dev console won't work otherwise.
4. Start the game and begin using the console as needed!
   - Configure the console through the inspector

Note that this API does come with some default commands, but to embrace the power of this asset you'll have to make your own.

### Default Controls for the "Basic Dev Console" Demo
| Action  | Description | Hotkey(s) |
| ------------- | ------------- | ------------- |
| Toggle  | Opens/closes the console depending on its current state. | Left Shift + Back-Quote/Tilde |
| Close  | Closes the console if it's open.  | Escape |
| Submit  | Parses the current text in the input buffer.  | Return |
| Clear Console  | Clears all content and history in the console.  | Left Ctrl + Backspace |
| Previous Command  | Assigns the "previous" command sent relative to the current history position into the input buffer.  | Up Arrow |
| Next Command  | Assigns the "next" command sent relative to the current history position into the input buffer. | Down Arrow |

### Creating New Commands
The system uses C# Reflection to find commands. All you have to do is implement the `DevConsoleCommand` class and start the game.
```c#
using System.Collections.Generic;
using DevConsole;

public class HelloWorldCommand : DevConsoleCommand 
{
        // All names associated with this command. These are the case-insensitive values users can enter to use the command.
        public override string[] GetNames()
        {
            return new string[] {"helloworld", "hw"};
        }

        // The action that actually happens when this command is executed.
        public override void Execute(List<string> parameters)
        {
            DevConsole.Print("Hello world!");
        }
        
        // (OPTIONAL) The text displayed when the "help helloworld" or "help hw" command is executed
        public override string GetHelp()
        {
            return "Displays \"Hello World\" into the console.";
        }
        
        // (OPTIONAL) Denotes whether or not this command can only be run in "dev mode"
        public override bool devModeOnly => false;
        
        // (OPTIONAL) Denotes whether or not this command can only be run in "cheat mode"
        public override bool cheatModeOnly => false;
}
```

### Tips 
- You don't need to use `DevConsoleBehaviour.Instance` to call console functions. You can simply reference `DevConsole` instead which is a static class that allows for shorthand/convenient calling of the `DevConsoleBehaviour` instance.

# Configuration
This section describes how to configure a `DevConsoleBehaviour`. 
### General
| Field | Description |
| ----- | ----------- |
| Debug | When enabled, this DevConsoleBehaviour will regularly log information to the console such as exceptions, invocations, etc. |
| Starts Open | Enable this if the `DevConsoleBehaviour` is considered "open" by default. |
| Open On Start | When enabled, the `DevConsoleBehaviour` will invoke `Open()` in `Start()` and automatically open. |
| Print Unity Console Logs | When enabled, Unity console messages will appear in the `DevConsoleBehaviour`. Avoid toggling this field during runtime. |
| Show Command Doesn't Exist Error | When enabled, if a user tries to submit a command that doesn't exist, an error will be displayed. |
| Clear Input Buffer After Submit | When enabled, the input buffer for the `DevConsoleBehaviour` will be cleared whenever `Submit()` is invoked. |
| Allow Dev Mode | Allows the usage of "Dev Mode". If disabled, `SetDevMode(...)` cannot be called and dev-mode-only commands cannot be executed. |
| Enable Dev Mode On Start | When enabled, the `DevConsoleBehaviour` will automatically enter "Dev Mode" if possible when it starts. |
| Allow Cheat Mode | Allows the usage of "Cheat Mode". If disabled, `SetCheatMode(...)` cannot be called and cheat-mode-only commands cannot be executed. |
| Enable Cheat Mode On Start | When enabled, the `DevConsoleBehaviour` will automatically enter "Cheat Mode" if possible when it starts. | 
| Max History | The maximum amount of entries the console will remember. 

### Components
| Field | Description | 
| --- | --- |
| Console Input Behaviour | The behaviour that decides how input is passed into the `DevConsoleBehaviour`. |
| Console Display Behaviour | The behaviour that handles displaying log entries for the `DevConsoleBehaviour`. |

### Events
This table refers to the `UnityEvent<T>` fields, not the `public static event Action` events available to the developers in C#.

| Field | Description |
| --- | --- |
| OnInitialized | Invoked when the `DevConsoleBehaviour` is initialized, during `OnEnable()`. |
| OnShutdown | Invoked when the `DevConsoleBehaviour` is going to be disabled, during `OnDisable()` |
| OnOpen | Invoked whenever the `DevConsoleBehaviour` is opened. |
| OnClosed | Invoked whenever the `DevConsoleBehaviour` is closed. |
| OnToggle | Invoked whenever the `DevConsoleBehaviour` is opened or closed. |
| OnUpdate | Invoked whenever the `DevConsoleBehaviour`'s `Update()` function is invoked. |
| OnClear | Invoked whenever the `DevConsoleBehaviour` is cleared. |
| OnSubmit(string) | Invoked along with `inputBuffer` whenever the `DevConsoleBehaviour`'s `Submit()` function is invoked. |
| OnPrint(string) | Invoked whenever something is printed to the `DevConsoleBehaviour`. |
| OnInputBufferChanged(string) | Invoked whenever the value of the `inputBuffer` on the `DevConsoleBehaviour` changes. |

# Credits
Thanks to the [Unity Discord Community](https://discord.com/invite/unity) for helping settle the package issues with this repo.