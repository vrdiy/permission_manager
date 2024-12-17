/*******************************************************************
* Name: Anthony Verdi
* Date: 10/19/24
* 
* This menu is the top level permissions menu.
*/
using PermissionManagerCore;
using static ConsoleExt;

class PermissionMenu : IMenu
{
    public PermissionMenu() { }
    public IMenu Start()
    {
        CLEAR();
        bool exit = false;
        while (!exit)
        {
            PRINT_COLORS(
                (GRAY, "Select Operation ("), (GREEN, "->Permissions"), (GRAY, "):\n"),
                (GRAY, "List - Lists all Permissions (L)\n"),
                (GRAY, "Add - Add a Permission (A)\n"),
                (GRAY, "Delete - Delete a Permission (D)\n"),
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
                    case "L":
                        PermissionManager.PrintPerms(2);
                        break;
                    case "A":
                        Console.Write("Enter a name for the permission: ");
                        Console.ForegroundColor = GREEN;
                        string? perm_to_add = Console.ReadLine();
                        Console.ResetColor();
                        if (null != perm_to_add)
                        {
                            if (0 == PermissionManager.AddPerm(perm_to_add))
                            {
                                PRINT_COLORS(
                                    (GRAY, $"Perm: "),
                                    (GREEN, $"{perm_to_add}"),
                                    (GRAY, " added.\n")
                                );
                            }
                            else
                            {
                                PRINT_COLORS(
                                    (RED, $"Unable to add {perm_to_add}\n")
                                );
                            }
                        }
                        break;
                    case "D":
                        Console.Write("Enter a name for the permission: ");
                        Console.ForegroundColor = GREEN;
                        string? perm_to_del = Console.ReadLine();
                        Console.ResetColor();
                        if (null != perm_to_del)
                        {
                            if (0 == PermissionManager.DeletePerm(perm_to_del))
                            {
                                PRINT_COLORS(
                                    (GRAY, $"Perm: "),
                                    (GREEN, $"{perm_to_del}"),
                                    (GRAY, " deleted.\n")
                                );
                            }
                            else
                            {
                                PRINT_COLORS(
                                    (RED, $"Permission name given did not match any existing\n")
                                );
                            }
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