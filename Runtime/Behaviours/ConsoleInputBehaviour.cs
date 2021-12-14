using DevConsole.Runtime.Enums;
using UnityEngine;

namespace DevConsole.Runtime.Behaviours
{
    public abstract class ConsoleInputBehaviour : MonoBehaviour
    {
        public abstract DevConsoleBehaviourCommand GetNextCommand();
    }
}