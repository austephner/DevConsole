using DevConsole.Runtime.Behaviours;
using UnityEngine;
using UnityEngine.UI;

namespace DevConsole.Runtime.Examples
{
    public class BasicInputBufferUpdater : MonoBehaviour
    {
        public InputField inputField;
        
        public void OnTextChanged(string value)
        {
            DevConsoleBehaviour.Instance.inputBuffer = value;
        }

        public void OnInputBufferChanged(string value)
        {
            inputField.text = value;
        }
    }
}