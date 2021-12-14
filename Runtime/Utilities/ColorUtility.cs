using UnityEngine;

namespace DevConsole.Utilities
{
    public static class ColorUtility
    {
        public static string ToHexString(this Color color)
        {
            return
                ((byte)(color.r * 255)).ToString("X2") +
                ((byte)(color.g * 255)).ToString("X2") +
                ((byte)(color.b * 255)).ToString("X2") +
                ((byte)(color.a * 255)).ToString("X2");
        }
    }
}