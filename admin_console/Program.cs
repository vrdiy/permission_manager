/*******************************************************************
* Name: Anthony Verdi
* Date: 10/19/24
* 
* The Program class is the driver of the application, it 
* sets up some permissions, users, and groups for demonstration 
* purposes, and then starts the menuing system with IMenu.Run()
*/
using PermissionManagerCore;
using System.Collections;
using static ConsoleExt;
class Program
{

    // Code to run on Ctrl+C to revert console color
    protected static void handleCancel(object? sender, ConsoleCancelEventArgs args)
    {
        CleanUp();
    }
    private static void CleanUp()
    {
        Console.ResetColor();
        CLEAR();
        PRINT_COLORS((GREEN, "\nGoodbye!\n"));

    }

    // Helper functions for creating masks
    public static BitArray MASK(params byte[] bytes)
    {
        return new BitArray(bytes.Reverse().ToArray());
    }
    public static BitArray MASK(params uint[] bytes)
    {
        int[] arr = Array.ConvertAll(bytes, item => (int)item);
        return new BitArray(arr.Reverse().ToArray());
    }

    public static Action PrintBanner()
    {
        return delegate ()
        {
            PRINT_COLORS(
                (WHITE, "======= "),
                (MAGENTA, "Permission Manager"),
                (WHITE, " =======\n")
            );
        };
    }
    public static void Main(string[] args)
    {

        // Setup header banner
        g_ClearFunc = PrintBanner();
        g_AllowClear = true;
        g_Seperator = "======= Permission Manager =======";

        // Setup cleanup on Ctrl+C
        Console.CancelKeyPress += new ConsoleCancelEventHandler(handleCancel);
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.ResetColor();

        
        string db = "test.db";
        PermissionManager.Init();
        LoadError load_result = PermissionManager.Load(db);
        if (load_result == LoadError.None){
            PRINT_COLORS((GREEN,"Successfully loaded test.db\n"));
        }
        else if (load_result == LoadError.DoesNotExist){
            PRINT_COLORS((YELLOW,"Creating database...\n"));
            PermissionManager.Create(db);
            PermissionManager.Load(db);
        }
        else if (load_result == LoadError.SqliteError){
            PRINT_COLORS((RED,"Unrecoverable error, dumping log"));
            PermissionManager.WriteLog();
            return;
        }
        PermissionManager.AddUser("Anthony");
        PermissionManager.AddUser("Bob");

        PermissionManager.AddPerm("canFly");
        PermissionManager.AddPerm("canEditBlocks");
        PermissionManager.AddPerm("canChat");
        PermissionManager.AddPerm("canLevelUp");
        PermissionManager.AddPerm("canDig");
        PermissionManager.AddPerm("canClimb");

        PermissionManager.AddGroup("operator");
        PermissionManager.SetGroupPermFromMask("operator", MASK(0b00000000_00000000_00000000_00000000, 0b11111111_11111111_11111111_11111111));
        PermissionManager.AddGroup("admin");
        PermissionManager.SetGroupPermFromMask("admin", MASK(0b00000000_00000000_00000000_00000000, 0b00000000_11111111_11111111_11111111));
        PermissionManager.AddGroup("moderator");
        PermissionManager.SetGroupPermFromMask("moderator", MASK(0b00000000_00000000_00000000_00000000, 0b00000000_00000000_11111111_11111111));

        PermissionManager.SetUserPerm("Anthony", "canFly", true);
        PermissionManager.SetUserPerm("Anthony", "canEditBlocks", true);
        PermissionManager.SetUserPerm("Anthony", "canChat", true);

        PermissionManager.AddUserToGroup("Anthony", "moderator");

        PermissionManager.SetUserPerm("Bob", "canChat", true);
        PermissionManager.SetUserPerm("Bob", "canFly", true);

        // Start the menus
        IMenu.Run(new MainMenu());

        CleanUp();
    }
}