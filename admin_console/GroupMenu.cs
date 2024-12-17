/*******************************************************************
* Name: Anthony Verdi
* Date: 10/19/24
* 
* This menu is the top level group menu.
*/
using PermissionManagerCore;
using static ConsoleExt;

class GroupMenu : IMenu
{
    public GroupMenu() { }
    public IMenu Start()
    {
        CLEAR();
        bool exit = false;
        while (!exit)
        {
            PRINT_COLORS(
                (GRAY, "Select Operation ("), (YELLOW, "->Groups"), (GRAY, "):\n"),
                (GRAY, "List - Lists all Groups (L)\n"),
                (GRAY, "Add - Add a Group (A)\n"),
                (GRAY, "Delete - Delete a Group (D)\n"),
                (GRAY, "Edit - Edit a Group (E)\n"),
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
                        PermissionManager.PrintGroups();
                        break;
                    case "A":
                        Console.Write("Enter a group name: ");
                        Console.ForegroundColor = GREEN;
                        string? group_to_add = Console.ReadLine();
                        Console.ResetColor();
                        if (null != group_to_add)
                        {
                            if (0 == PermissionManager.AddGroup(group_to_add))
                            {
                                PRINT_COLORS(
                                    (GRAY, $"Group: "),
                                    (YELLOW, $"{group_to_add}"),
                                    (GRAY, " added.\n")
                                );
                            }
                            else
                            {
                                PRINT_COLORS(
                                    (RED, $"Unable to add {group_to_add}\n")
                                );
                            }

                        }
                        break;
                    case "D":
                        Console.Write("Enter a group name: ");
                        Console.ForegroundColor = GREEN;
                        string? group_to_del = Console.ReadLine();
                        Console.ResetColor();
                        if (null != group_to_del)
                        {
                            if (!PermissionManager.GroupExists(group_to_del))
                            {
                                ERROR($"Group: {group_to_del} does not exist");
                                break;
                            }
                            PRINT_COLORS(
                                (GRAY, $"Are you sure you want to "),
                                (RED, $"DELETE"),
                                (GRAY, $" group "),
                                (RED, $"{group_to_del}"),
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
                            if (0 == PermissionManager.DeleteGroup(group_to_del))
                            {
                                PRINT_COLORS(
                                    (GRAY, $"Group: "),
                                    (GREEN, $"{group_to_del}"),
                                    (GRAY, " deleted.\n")
                                );
                            }
                            else
                            {
                                ERROR($"Group: {group_to_del} does not exist");
                            }
                        }
                        break;
                    case "E":
                        PermissionManager.PrintGroups();
                        Console.Write("Enter a group name: ");
                        Console.ForegroundColor = GREEN;
                        string? group_to_edit = Console.ReadLine();
                        Console.ResetColor();
                        if (null != group_to_edit)
                        {
                            if (!PermissionManager.GroupExists(group_to_edit))
                            {
                                ERROR($"Group: {group_to_edit} does not exist");
                                break;
                            }
                            return new EditGroupMenu(group_to_edit);
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
