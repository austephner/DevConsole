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

#### Default Controls for the "Basic Dev Console" Demo
| Action  | Description | Hotkey(s) |
| ------------- | ------------- | ------------- |
| Toggle  | Opens/closes the console depending on its current state. | Left Shift + Back-Quote/Tilde |
| Close  | Closes the console if it's open.  | Escape |
| Submit  | Parses the current text in the input buffer.  | Return |
| Clear Console  | Clears all content and history in the console.  | Left Ctrl + Backspace |
| Previous Command  | Assigns the "previous" command sent relative to the current history position into the input buffer.  | Up Arrow |
| Next Command  | Assigns the "next" command sent relative to the current history position into the input buffer. | Down Arrow |

# Creating New Commands
The system uses C# Reflection to find commands. All you have to do is implement the `DevConsoleCommand` class and start the game.
```c#
using System.Collections.Generic;
using DevConsole.Behaviours;

public class HelloWorldCommand : DevConsoleCommand 
{
        // All names associated with this command. These are the case-insensitive values users can enter to use the command.
        public override string[] GetNames()
        {
            return new string[] {"helloworld", "hw"};
        }

        // The text displayed when the "help helloworld" or "help hw" command is executed
        public override string GetHelp()
        {
            return "Displays \"Hello World\" into the console.";
        }

        // The action that actually happens when this command is executed.
        public override void Execute(List<string> parameters)
        {
            DevConsoleBehaviour.Instance.Print("Hello world!");
        }
}
```