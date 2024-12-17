/*******************************************************************
* Name: Anthony Verdi
* Date: 10/19/24
* 
* This menu is used to edit a permission's value on a 
* specific group.
*/
using PermissionManagerCore;
using static ConsoleExt;

class EditGroupPermissionMenu : IMenu
{
    private string m_group_name;
    public EditGroupPermissionMenu(string group_name)
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
                (GRAY, "Edit Permissions ("),
                (YELLOW, $"->{m_group_name}"),
                (GRAY, "):\n")
            );
            PermissionManager.PrintGroupPerms(m_group_name);
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
                    bool perm_value = PermissionManager.GetGroupPerm(m_group_name, perm_to_check);
                    PRINT_COLORS(
                        (GRAY, $"Current value of "),
                        (YELLOW, $"{perm_to_check}"),
                        (GRAY, ": "),
                        (perm_value ? GREEN : RED, $"{perm_value}\n"),
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
                            if (-1 == PermissionManager.SetGroupPerm(m_group_name, perm_to_check, new_value))
                            {
                                ERROR($"Error trying to set {perm_to_check} on {m_group_name}");
                            };
                            PRINT_COLORS(
                                (GRAY, $"{m_group_name} updated.\n"),
                                (MAGENTA, $"{m_group_name}"),
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