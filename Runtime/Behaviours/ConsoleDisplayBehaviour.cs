using DevConsole.Runtime.Enums;
using UnityEngine;

namespace DevConsole.Runtime.Behaviours
{
    /// <summary>
    /// Handles displaying text from a <see cref="DevConsoleBehaviour"/>.
    /// </summary>
    public abstract class ConsoleDisplayBehaviour : MonoBehaviour
    {
        public abstract void Print(string text, DevConsolePrintType printType);
        public abstract void Clear();
        public abstract void RemoveHistoryAt(int index);
    }
}