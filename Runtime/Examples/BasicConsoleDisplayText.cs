using UnityEngine;
using UnityEngine.UI;

namespace DevConsole.Examples
{
    public class BasicConsoleDisplayText : MonoBehaviour
    {
        public Text text => GetComponent<Text>() ?? GetComponentInChildren<Text>();
        public RectTransform layout => GetComponent<RectTransform>();
    }
}