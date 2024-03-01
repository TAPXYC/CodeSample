using UnityEngine;



public class DebugX
{
    public static Color WarningColor = new Color(1f, 1f, 0.3f);
    public static Color ErrorColor = new Color(1f, 0.4f, 0.4f);


    public static void ColorMessage(string message, Color? color = null)
    {
        Debug.Log(ColorPart(message, color));
    }




    public static string ColorPart(string message, Color? color = null)
    {
        Color messageColor = color == null ? Color.gray : color.Value;
        return string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(messageColor.r * 255f), (byte)(messageColor.g * 255f), (byte)(messageColor.b * 255f), message);
    }
}