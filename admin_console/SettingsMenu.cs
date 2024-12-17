/*******************************************************************
* Name: Anthony Verdi
* Date: 10/19/24
* 
* The settings menu is used to adjust the behaviour, more
* specifically the output of the application to the user's 
* preference.
*/

using static ConsoleExt;
class SettingsMenu : IMenu
{
    public SettingsMenu() { }
    public IMenu Start()
    {
        CLEAR();
        bool exit = false;
        while (!exit)
        {
            string on_off = g_AllowClear ? "OFF" : "ON";
            PRINT_COLORS(
                (DARK_MAGENTA, "Settings\n"),
                (GRAY, $"Console Clearing - Turn "),
                (g_AllowClear ? RED : GREEN, $"{on_off}"),
                (GRAY, " console clearing (ON by default). (C)\n"),
                (GRAY, "Seperator - Set the console seperator for when console clearing is off (S)\n"),
                (GRAY, "Exit Menu - Go back to the previous menu (X)\n"),
                (GRAY, "->")
            );
            Console.ForegroundColor = GREEN;
            string? choice = Console.ReadLine();
            Console.ResetColor();

            if (null != choice)
            {
                switch (choice.ToUpper())
                {
                    case "C":
                        g_AllowClear = !g_AllowClear;
                        break;
                    case "S":
                        Console.Write("Enter a new seperator: ");
                        Console.ForegroundColor = GREEN;
                        string? new_seperator = Console.ReadLine();
                        Console.ResetColor();
                        if (null != new_seperator)
                        {
                            g_Seperator = new_seperator;
                        }
                        break;
                    case "X":
                        return new ExitMenu();
                    default:
                        ERROR("Invalid choice");
                        break;
                }
            }
            CONTINUE();
        }
        return new ExitMenu();
    }
}