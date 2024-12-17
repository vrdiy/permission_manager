/*******************************************************************
* Name: Anthony Verdi
* Date: 10/19/24
* 
* The MainMenu class is the root menu for this application,
* all other menus are reachable through this menu.
*/
using PermissionManagerCore;
using static ConsoleExt;


class MainMenu : IMenu
{
    public MainMenu() { }
    public IMenu Start()
    {
        CLEAR();
        bool exit = false;
        while (!exit)
        {
            PRINT_COLORS(
                (GRAY, $"Active Database: "),
                (CYAN, $"{PermissionManager.GetActiveDatabase()}\n"),
                (GRAY, "(Users): ".PadRight(16)),
                (BLUE, $"{PermissionManager.GetNumberOfUsers()}\n"),
                (GRAY, "(Groups): ".PadRight(16)),
                (YELLOW, $"{PermissionManager.GetNumberOfGroups()}\n"),
                (GRAY, "(Permissions): ".PadRight(16)),
                (GREEN, $"{PermissionManager.GetNumberOfPerms()}\n"),

                (GRAY, "Select "),
                (BLUE, "Users"), (GRAY, "/"),
                (YELLOW, "Groups"), (GRAY, "/"),
                (GREEN, "Permissions"),
                (GRAY, " ("), (BLUE, "U"), (GRAY, "/"), (YELLOW, "G"), (GRAY, "/"), (GREEN, "P"), (GRAY, "):\n"),
                (GRAY, "Change Active Database - (C)\n"),
                (GRAY, "Settings - (S)\n"),
                (GRAY, "Exit - (X)\n"),
                (GRAY, "->")
            );
            Console.ForegroundColor = GREEN;
            string? choice = Console.ReadLine();
            Console.ResetColor();

            if (null != choice)
            {
                switch (choice.ToUpper())
                {
                    case "U":
                        return new UserMenu();
                    case "G":
                        return new GroupMenu();
                    case "P":
                        return new PermissionMenu();
                    case "C":
                        PRINT_COLORS(
                            (GRAY, "Enter the database filename (with extension):")
                        );
                        Console.ForegroundColor = CYAN;
                        string? db_name = Console.ReadLine();
                        Console.ResetColor();

                        if (null != db_name)
                        {
                            db_name = db_name.Replace(" ", "");
                            if (db_name == "")
                            {
                                PRINT_COLORS((RED, "Database name cannot be empty\n"));
                                break;
                            }
                            if (PermissionManager.Load(db_name) == LoadError.None)
                            {
                                PRINT_COLORS(
                                    (GRAY, "Successfully loaded "),
                                    (CYAN, $"{db_name}\n")
                                );
                            }
                            else
                            {
                                PRINT_COLORS(
                                    (RED, $"{db_name}"),
                                    (GRAY, " does not exist. Would you like to create it? (Y/N): ")
                                );
                                Console.ForegroundColor = GREEN;
                                string? create_db = Console.ReadLine();
                                Console.ResetColor();

                                if (null != create_db)
                                {
                                    switch (create_db.ToUpper())
                                    {
                                        case "Y":
                                            if (-1 == PermissionManager.Create(db_name))
                                            {
                                                PRINT_COLORS(
                                                    (RED, "Something went wrong creating database")
                                                );
                                                break;
                                            };
                                            PermissionManager.Load(db_name);
                                            PRINT_COLORS(
                                                (CYAN, $"{db_name}"),
                                                (GRAY, " created and loaded\n")
                                            );
                                            break;
                                        default:
                                            break;


                                    }
                                }
                            };
                        }
                        break;
                    case "S":
                        return new SettingsMenu();
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