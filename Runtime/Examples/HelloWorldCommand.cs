using System.Collections.Generic;
using DevConsole;
using DevConsole.Commands;

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
        Console.Print("Hello world!");
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