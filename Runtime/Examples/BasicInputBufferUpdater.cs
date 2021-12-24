using DevConsole.Behaviours;
using UnityEngine;
using UnityEngine.UI;

namespace DevConsole.Examples
{
    public class BasicInputBufferUpdater : MonoBehaviour
    {
        public InputField inputField;
        
        public void OnTextChanged(string value)
        {
            Console.inputBuffer = value;
        }

        public void OnInputBufferChanged(string value)
        {
            inputField.text = value;
        }
    }
}