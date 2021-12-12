using DevConsole.Enums;
using UnityEngine;

namespace DevConsole.Behaviours
{
    public abstract class ConsoleInputBehaviour : MonoBehaviour
    {
        public abstract DevConsoleBehaviourCommand GetNextCommand();
    }
}