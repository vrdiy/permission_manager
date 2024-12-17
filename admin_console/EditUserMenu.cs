/*******************************************************************
* Name: Anthony Verdi
* Date: 10/19/24
* 
* This menu is used to edit a specific user.
*/
using PermissionManagerCore;
using static ConsoleExt;

class EditUserMenu : IMenu
{
    private string m_user_name;
    public EditUserMenu(string user_name)
    {
        m_user_name = user_name;
    }
    public IMenu Start()
    {
        bool exit = false;
        CLEAR();
        while (!exit)
        {
            List<string> groups = PermissionManager.GetUserGroups(m_user_name);

            string groups_separated = "";
            foreach (string g in groups)
            {
                groups_separated += $"{g} ";
            };
            PRINT_COLORS(
                (GRAY, "User: "),
                (MAGENTA, $"{m_user_name}\n"),
                (GRAY, "Member of: "),
                (groups_separated != "" ? YELLOW : GRAY, $"{(groups_separated != "" ? groups_separated : "None")}\n"),
                (GRAY, "Select Operation ("), (MAGENTA, $"->{m_user_name}"), (GRAY, "):\n"),
                (GRAY, $"List - Lists Permissions (L)\n"),
                (GRAY, $"Delete - Delete this User (D)\n"),
                (GRAY, $"Edit - Edit a Permission for this User (E)\n"),
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
                        PermissionManager.PrintUserPerms(m_user_name);
                        break;
                    case "D":
                        PRINT_COLORS(
                            (GRAY, $"Are you sure you want to "),
                            (RED, $"DELETE"),
                            (GRAY, $" user "),
                            (RED, $"{m_user_name}"),
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
                        if (0 == PermissionManager.DeleteUser(m_user_name))
                        {
                            PRINT_COLORS(
                                (GRAY, $"User: "),
                                (GREEN, $"{m_user_name}"),
                                (GRAY, " deleted.\n")
                            );
                        }
                        return new ExitMenu();
                    case "E":
                        return new EditUserPermissionMenu(m_user_name);
                    case "A":
                        PermissionManager.PrintGroups();
                        PRINT_COLORS(
                            (GRAY, "Enter group name: ")
                            );
                        Console.ForegroundColor = YELLOW;
                        string? group_to_join = Console.ReadLine();
                        Console.ResetColor();
                        if (null != group_to_join)
                        {
                            if (-1 != PermissionManager.AddUserToGroup(m_user_name, group_to_join))
                            {
                                PRINT_COLORS((GREEN, "Success\n"));
                            }
                            else
                            {
                                ERROR($"Error adding {m_user_name} to {group_to_join}");
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
                            (GRAY, "Enter group name: ")
                            );
                        Console.ForegroundColor = YELLOW;
                        string? group_to_leave = Console.ReadLine();
                        Console.ResetColor();
                        if (null != group_to_leave)
                        {

                            if (-1 != PermissionManager.RemoveUserFromGroup(m_user_name, group_to_leave))
                            {
                                PRINT_COLORS((GREEN, "Success\n"));
                            }
                            else
                            {
                                ERROR($"Error removing {m_user_name} from {group_to_leave}");
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
                            (GRAY, "Enter group name: ")
                            );
                        Console.ForegroundColor = YELLOW;
                        string? group_to_copy_perms_from = Console.ReadLine();
                        Console.ResetColor();
                        if (null != group_to_copy_perms_from)
                        {
                            if (-1 != PermissionManager.SetUserPermFromGroup(m_user_name, group_to_copy_perms_from))
                            {
                                PRINT_COLORS((GREEN, "Success\n"));
                            }
                            else
                            {
                                ERROR($"Error adding {group_to_copy_perms_from} perms to {m_user_name}");
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