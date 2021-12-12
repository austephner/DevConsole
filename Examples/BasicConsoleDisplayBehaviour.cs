using System;
using System.Collections.Generic;
using DevConsole.Behaviours;
using DevConsole.Enums;
using DevConsole.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace DevConsole.Examples
{
    public class BasicConsoleDisplayBehaviour : ConsoleDisplayBehaviour
    {
        public ScrollRect scrollRect;
        
        public RectTransform contentLayoutGroup;

        public BasicConsoleDisplayText displayPrefab;

        public Color
            normalTextColor = Color.white,
            successTextColor = Color.green,
            warningTextColor = new Color(1.0f, 0.5f, 0.1f, 1.0f),
            errorTextColor = new Color(1.0f, 0.1f, 0.1f, 1.0f);

        private List<BasicConsoleDisplayText> _displayTexts = new List<BasicConsoleDisplayText>();
        
        public override void Print(string text, DevConsolePrintType printType)
        {
            var displayText = Instantiate(displayPrefab, contentLayoutGroup);
            displayText.text.text = $"<color=#{GetColor(printType).ToHexString()}>{text}</color>";
            _displayTexts.Add(displayText);

            LayoutRebuilder.ForceRebuildLayoutImmediate(displayText.layout);
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentLayoutGroup);
            scrollRect.normalizedPosition = Vector2.zero;
        }

        private Color GetColor(DevConsolePrintType printType)
        {
            switch (printType)
            {
                case DevConsolePrintType.Info: return normalTextColor;
                case DevConsolePrintType.Warning: return warningTextColor;
                case DevConsolePrintType.Success: return successTextColor;
                case DevConsolePrintType.Error: return errorTextColor;
                case DevConsolePrintType.Misc: return normalTextColor;
                default: throw new ArgumentOutOfRangeException(nameof(printType), printType, null);
            }
        }

        public override void Clear()
        {
            for (var i = _displayTexts.Count - 1; i >= 0; i--)
            {
                var displayText = _displayTexts[i];
                DestroyImmediate(displayText.gameObject);
                _displayTexts.RemoveAt(i);
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentLayoutGroup);
            scrollRect.normalizedPosition = Vector2.zero;
        }

        public override void RemoveHistoryAt(int index)
        {
            var displayText = _displayTexts[index];
            _displayTexts.RemoveAt(index);
            
            DestroyImmediate(displayText.gameObject);
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentLayoutGroup);
        }
    }
}