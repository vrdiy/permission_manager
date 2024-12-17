/*******************************************************************
* Name: Anthony Verdi
* Date: 10/19/24
* 
* The UserMenu class is the top level menu for managing user's permissions.
*/
using PermissionManagerCore;
using static ConsoleExt;

class UserMenu : IMenu
{
    public UserMenu() { }
    public IMenu Start()
    {
        CLEAR();
        bool exit = false;
        while (!exit)
        {
            PRINT_COLORS(
                (GRAY, "Select Operation ("), (BLUE, "->Users"), (GRAY, "):\n"),
                (GRAY, "List - Lists all Users (L)\n"),
                (GRAY, "Add - Add a User (A)\n"),
                (GRAY, "Delete - Delete a User (D)\n"),
                (GRAY, "Edit - Edit a User (E)\n"),
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
                        PermissionManager.PrintUsersColored(GREEN);
                        break;
                    case "A":
                        Console.Write("Enter a username: ");
                        Console.ForegroundColor = GREEN;
                        string? user_to_add = Console.ReadLine();
                        Console.ResetColor();
                        if (null != user_to_add)
                        {
                            if (0 == PermissionManager.AddUser(user_to_add))
                            {
                                PRINT_COLORS(
                                    (GRAY, $"User: "),
                                    (GREEN, $"{user_to_add}"),
                                    (GRAY, " added.\n")
                                );
                            }
                            else
                            {
                                PRINT_COLORS(
                                    (RED, $"Unable to add {user_to_add}\n")
                                );
                            }
                        }
                        break;
                    case "D":
                        Console.Write("Enter a username: ");
                        Console.ForegroundColor = GREEN;
                        string? user_to_del = Console.ReadLine();
                        Console.ResetColor();
                        if (null != user_to_del)
                        {
                            if (!PermissionManager.UserExists(user_to_del))
                            {
                                ERROR($"User: {user_to_del} does not exist");
                                break;
                            }
                            PRINT_COLORS(
                                (GRAY, $"Are you sure you want to "),
                                (RED, $"DELETE"),
                                (GRAY, $" user "),
                                (RED, $"{user_to_del}"),
                                (GRAY, $"?\nType YES in all caps to confirm: ")
                            );

                            Console.ForegroundColor = RED;
                            string? confirm_delete = Console.ReadLine();
                            Console.ResetColor();

                            if (null == confirm_delete)
                            {
                                break;
                            }
                            if (confirm_delete != "YES")
                            {
                                break;
                            }
                            if (0 == PermissionManager.DeleteUser(user_to_del))
                            {
                                PRINT_COLORS(
                                    (GRAY, $"User: "),
                                    (GREEN, $"{user_to_del}"),
                                    (GRAY, " deleted.\n")
                                );
                            }
                        }
                        break;
                    case "E":
                        PermissionManager.PrintUsersColored(GREEN);
                        Console.Write("Enter a username: ");
                        Console.ForegroundColor = GREEN;
                        string? user_to_edit = Console.ReadLine();
                        Console.ResetColor();
                        if (null != user_to_edit)
                        {
                            if (!PermissionManager.UserExists(user_to_edit))
                            {
                                ERROR($"User: {user_to_edit} does not exist");
                                break;
                            }
                            return new EditUserMenu(user_to_edit);
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
