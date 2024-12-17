/*******************************************************************
* Name: Anthony Verdi
* Date: 10/19/24
* 
* This menu is used to edit a specific group.
*/
using PermissionManagerCore;
using static ConsoleExt;

class EditGroupMenu : IMenu
{
    private string m_group_name;
    public EditGroupMenu(string group_name)
    {
        m_group_name = group_name;
    }
    public IMenu Start()
    {
        bool exit = false;
        CLEAR();
        while (!exit)
        {

            PRINT_COLORS(
                (GRAY, "Group: "),
                (YELLOW, $"{m_group_name}\n"),
                (GRAY, "Select Operation ("), (MAGENTA, $"->{m_group_name}"), (GRAY, "):\n"),
                (GRAY, $"List - Lists Permissions (L)\n"),
                (GRAY, $"Delete - Delete this Group (D)\n"),
                (GRAY, $"Edit - Edit a Permission for this Group (E)\n"),
                (GRAY, $"Add User to Group - (A)\n"),
                (GRAY, $"Remove User from Group - (R)\n"),
                (GRAY, $"Give User permissions of a Group - (G)\n"),
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
                        PermissionManager.PrintGroupPerms(m_group_name);
                        break;
                    case "D":
                        PRINT_COLORS(
                            (GRAY, $"Are you sure you want to "),
                            (RED, $"DELETE"),
                            (GRAY, $" group "),
                            (YELLOW, $"{m_group_name}"),
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
                        if (0 == PermissionManager.DeleteGroup(m_group_name))
                        {
                            PRINT_COLORS(
                                (GRAY, $"Group: "),
                                (GREEN, $"{m_group_name}"),
                                (GRAY, " deleted.\n")
                            );
                        }
                        else
                        {
                            ERROR($"Group: {m_group_name} does not exist");
                        }
                        return new ExitMenu();
                    case "E":
                        return new EditGroupPermissionMenu(m_group_name);
                    case "A":
                        PermissionManager.PrintUsersColored(GREEN);
                        PRINT_COLORS(
                            (GRAY, "Enter user name: ")
                            );
                        Console.ForegroundColor = YELLOW;
                        string? user_to_add = Console.ReadLine();
                        Console.ResetColor();
                        if (null != user_to_add)
                        {
                            if (-1 != PermissionManager.AddUserToGroup(user_to_add, m_group_name))
                            {
                                PRINT_COLORS((GREEN, "Success\n"));
                            }
                            else
                            {
                                ERROR($"Error adding {user_to_add} to {m_group_name}");
                            };

                        }
                        else
                        {
                            PRINT_COLORS(
                                (GRAY, "Invalid entry")
                            );
                        }
                        break;
                    case "R":
                        PRINT_COLORS(
                            (GRAY, "Enter user name: ")
                            );
                        Console.ForegroundColor = YELLOW;
                        string? user_to_remove = Console.ReadLine();
                        Console.ResetColor();
                        if (null != user_to_remove)
                        {

                            if (-1 != PermissionManager.RemoveUserFromGroup(user_to_remove, m_group_name))
                            {
                                PRINT_COLORS((GREEN, "Success\n"));
                            }
                            else
                            {
                                ERROR($"Error removing {user_to_remove} from {m_group_name}");
                            };
                        }
                        else
                        {
                            PRINT_COLORS(
                                (GRAY, "Invalid entry")
                            );
                        }
                        break;
                    case "G":
                        PermissionManager.PrintGroups();
                        PRINT_COLORS(
                            (GRAY, "Enter user name: ")
                            );
                        Console.ForegroundColor = YELLOW;
                        string? user_to_give_perms_to = Console.ReadLine();
                        Console.ResetColor();
                        if (null != user_to_give_perms_to)
                        {
                            if (-1 != PermissionManager.SetUserPermFromGroup(user_to_give_perms_to, m_group_name))
                            {
                                PRINT_COLORS((GREEN, "Success\n"));
                            }
                            else
                            {
                                ERROR($"Error adding {m_group_name} perms to {user_to_give_perms_to}");
                            };
                        }
                        else
                        {
                            PRINT_COLORS(
                                (GRAY, "Invalid entry")
                            );
                        }
                        break;

                    case "X":
                        return new ExitMenu();
                    default:
                        break;
                }
                CONTINUE();
            }
        }
        return new ExitMenu();
    }
}