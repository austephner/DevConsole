using System;
using DevConsole.Behaviours;

namespace DevConsole.Commands
{
    /// <summary>
    /// Put onto <see cref="DevConsoleCommand"/> implementations that shouldn't be loaded initially by the
    /// <see cref="DevConsoleBehaviour"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class NonLoadableCommandAttribute : Attribute { }
}