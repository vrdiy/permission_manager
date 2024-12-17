/*******************************************************************
* Name: Anthony Verdi
* Date: 10/19/24
* 
* This menu is used to edit a specific user's permission.
*/
using PermissionManagerCore;
using static ConsoleExt;

class EditUserPermissionMenu : IMenu
{
    private string m_user_name;
    public EditUserPermissionMenu(string user_name)
    {
        m_user_name = user_name;
    }

    public IMenu Start()
    {
        bool exit = false;
        CLEAR();
        while (!exit)
        {
            PRINT_COLORS(
                (GRAY, "Edit Permissions ("),
                (MAGENTA, $"->{m_user_name}"),
                (GRAY, "):\n")
            );
            PermissionManager.PrintUserPerms(m_user_name);
            PRINT_COLORS(
                (GRAY, "Enter the permission name: ")
            );

            Console.ForegroundColor = YELLOW;
            string? perm_to_check = Console.ReadLine();
            Console.ResetColor();

            if (null != perm_to_check)
            {
                if (-1 == PermissionManager.GetPermId(perm_to_check))
                {
                    PRINT_COLORS(
                        (GRAY, "Permission "),
                        (RED, $"{perm_to_check}"),
                        (GRAY, " does not exist.\n")
                    );
                    exit = true;
                    CONTINUE();
                }
                else
                { // Perm is valid
                    bool user_value;
                    bool group_value = PermissionManager.GetUserPerm(m_user_name, perm_to_check, true);

                    PRINT_COLORS(
                        (GRAY, $"Current value of "),
                        (YELLOW, $"{perm_to_check}"),
                        (GRAY, ": "),
                        (group_value ? GREEN : RED, $"{group_value}\n"),
                        (GRAY, $"Enter the new value (True/False): ")
                    );

                    Console.ForegroundColor = DARK_MAGENTA;
                    string? new_value_str = Console.ReadLine();
                    Console.ResetColor();

                    if (null != new_value_str)
                    {
                        bool valid = new_value_str.ToLower() == "true" || new_value_str.ToLower() == "false";
                        if (!valid)
                        {
                            PRINT_COLORS((GRAY, "Invalid input\n"));
                            exit = true;
                            CONTINUE();
                        }
                        else
                        { // Input is valid
                            bool new_value = new_value_str.ToLower() == "true";
                            if (-1 == PermissionManager.SetUserPerm(m_user_name, perm_to_check, new_value))
                            {
                                ERROR($"User: {m_user_name} does not exist");
                            };
                            user_value = PermissionManager.GetUserPerm(m_user_name, perm_to_check, false);
                            group_value = PermissionManager.GetUserPerm(m_user_name, perm_to_check, true);
                            if (group_value && !user_value)
                            {
                                PRINT_COLORS(
                                    (MAGENTA, $"{m_user_name} "),
                                    (GRAY, $"is given "),
                                    (YELLOW, $"{perm_to_check} "),
                                    (GRAY, $"by a group.\n"),
                                    (GRAY, $"The user must be removed from the group in order to revoke this permission.\n")
                                );
                                CONTINUE();
                                break;
                            };
                            PRINT_COLORS(
                                (GRAY, $"{m_user_name} updated.\n"),
                                (MAGENTA, $"{m_user_name}"),
                                (GRAY, " -> "),
                                (YELLOW, $"{perm_to_check}"),
                                (GRAY, $" -> "),
                                (new_value ? GREEN : RED, $"{new_value}\n")
                            );
                            CONTINUE();
                            exit = true;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Input was null?");
                CONTINUE();
            }
        }
        return new ExitMenu();

    }
}