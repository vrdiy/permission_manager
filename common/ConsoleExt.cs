/*******************************************************************
* Name: Anthony Verdi
* Date: 10/19/24
* 
* The ConsoleExt class is a static class which provides
* useful methods for the console.
*/

public static class ConsoleExt
{
    public const ConsoleColor BLACK = ConsoleColor.Black;
    public const ConsoleColor BLUE = ConsoleColor.Blue;
    public const ConsoleColor CYAN = ConsoleColor.Cyan;
    public const ConsoleColor DARK_BLUE = ConsoleColor.DarkBlue;
    public const ConsoleColor DARK_CYAN = ConsoleColor.DarkCyan;
    public const ConsoleColor DARK_GREY = ConsoleColor.DarkGray;
    public const ConsoleColor DARK_GREEN = ConsoleColor.DarkGreen;
    public const ConsoleColor DARK_MAGENTA = ConsoleColor.DarkMagenta;
    public const ConsoleColor DARK_YELLOW = ConsoleColor.DarkYellow;
    public const ConsoleColor GRAY = ConsoleColor.Gray;
    public const ConsoleColor GREEN = ConsoleColor.Green;
    public const ConsoleColor MAGENTA = ConsoleColor.Magenta;
    public const ConsoleColor RED = ConsoleColor.Red;
    public const ConsoleColor WHITE = ConsoleColor.White;
    public const ConsoleColor YELLOW = ConsoleColor.Yellow;
    public const ConsoleColor NO_COLOR = ConsoleColor.Gray;
    public static bool g_AllowClear = false;
    public static string g_Seperator = "===========================";
    public static ConsoleColor g_SeperatorColor = GRAY;
    public static Action? g_ClearFunc = null;
    public static void WARN(string warning_message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Error.WriteLine(warning_message);
        Console.ResetColor();
    }
    public static void ERROR(string warning_message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine(warning_message);
        Console.ResetColor();
    }
    public static void CONTINUE()
    {
        Console.Write("Continue...");
        Console.ReadLine();
        CLEAR();
    }
    public static void PRINT_COLORS(params (ConsoleColor color, string text)[] pairs)
    {
        foreach ((ConsoleColor color, string text) s in pairs)
        {
            Console.ForegroundColor = s.color;
            Console.Write(s.text);
        }
        Console.ResetColor();
    }
    public static void CLEAR()
    {
        if (g_AllowClear)
        {
            Console.Clear();
            if (null != g_ClearFunc)
            {
                g_ClearFunc();
            }
        }
        else
        {
            Console.ForegroundColor = g_SeperatorColor;
            Console.WriteLine(g_Seperator);
            Console.ResetColor();
        }
    }

}