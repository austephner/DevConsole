using UnityEngine;

namespace DevConsole.Examples
{
    public class BasicConsoleToggler : MonoBehaviour
    {
        public CanvasGroup canvasGroup;

        public float transitionSpeed = 10.0f;

        private bool _open;

        private void Start()
        {
            _open = DevConsole.isOpen;
        }
        
        private void Update()
        {
            canvasGroup.blocksRaycasts = _open;
            canvasGroup.interactable = _open;
            canvasGroup.alpha = Mathf.MoveTowards(
                canvasGroup.alpha, 
                _open ? 1.0f : 0.0f, 
                transitionSpeed * Time.unscaledDeltaTime);
        }
        
        public void Open()
        {
            _open = true;
        }

        public void Close()
        {
            _open = false;
        }
    }
}