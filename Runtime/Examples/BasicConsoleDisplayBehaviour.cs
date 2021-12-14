using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevConsole.Behaviours;
using DevConsole.Commands;
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

        [HideInInspector]
        public List<BasicConsoleDisplayText> displayTexts = new List<BasicConsoleDisplayText>();
        
        public override void Print(string text, DevConsolePrintType printType)
        {
            var displayText = Instantiate(displayPrefab, contentLayoutGroup);
            displayText.text.text = $"<color=#{GetColor(printType).ToHexString()}>{text}</color>";
            displayTexts.Add(displayText);

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
            for (var i = displayTexts.Count - 1; i >= 0; i--)
            {
                var displayText = displayTexts[i];
                DestroyImmediate(displayText.gameObject);
                displayTexts.RemoveAt(i);
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentLayoutGroup);
            scrollRect.normalizedPosition = Vector2.zero;
        }

        public override void RemoveHistoryAt(int index)
        {
            var displayText = displayTexts[index];
            displayTexts.RemoveAt(index);
            
            DestroyImmediate(displayText.gameObject);
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentLayoutGroup);
        }
    }

    public class DumpCommand : DevConsoleCommand
    {
        public override string[] GetNames()
        {
            return new[] { "dump" };
        }

        public override string GetHelp()
        {
            return "Dumps the visible console content to a file of specified name. If no name is specified, a generic one will be used."; 
        }

        public override void Execute(List<string> parameters)
        {
            var filename = parameters.Count > 0
                ? string.Join(" ", parameters)
                : $"console_dump_{DateTime.Now:dd_mm_yyyy_hh_mm_ss}.txt";

            var basicConsoleDisplayBehaviour = GameObject.FindObjectOfType<BasicConsoleDisplayBehaviour>();

            if (!basicConsoleDisplayBehaviour)
            {
                DevConsole.PrintError("No basic console display behaviour detected.");
                return;
            }

            var filePath = Path.Combine(Application.dataPath, filename);            
            
            try
            {
                File.WriteAllLines(
                    filePath,
                    basicConsoleDisplayBehaviour.displayTexts
                        .Select(dt => dt.text.text)
                        .ToList());
                
                DevConsole.PrintSuccess($"Dumped to:\n{filePath}");
            }
            catch (Exception exception)
            {
                DevConsole.PrintError($"Failed to dump file, reason: {exception.Message}");
            }
        }
    }
}