using PM = PermissionManagerCore.PermissionManager;
using static ConsoleExt;
using PermissionManagerCore;

/// <summary>
/// This is a demonstration program which demonstrates reads from the db.
/// </summary>
class Program
{
    public static int InitialSetup(string db){
        PM.Create(db);
        PM.Load(db);
        PRINT_COLORS((GREEN,"Successfully loaded test.db\n"));

        PM.AddPerm("canFly");
        PM.AddPerm("canChat");

        PM.AddUser("Anthony");
        PM.SetUserPerm("Anthony", "canFly", true);

        PM.AddGroup("operator");
        PM.SetGroupPerm("operator","canFly",true);
        PM.SetGroupPerm("operator", "canChat", true);

        PM.AddUserToGroup("Anthony","operator");
        return 0;
    }
    public static void Main(){

        string db = "test.db";
        PM.Init();
        LoadError load_result = PM.Load(db);
        if (load_result == LoadError.None){
            PRINT_COLORS((GREEN,"Successfully loaded test.db\n"));
        }
        else if (load_result == LoadError.DoesNotExist){
            PRINT_COLORS((YELLOW,"Creating database...\n"));
            InitialSetup(db);
        }
        else if (load_result == LoadError.SqliteError){
            PRINT_COLORS((RED,"Unrecoverable error, dumping log"));
            PM.WriteLog();
            return;
        }
        
        while (true){
            bool has_permission = PM.GetUserPerm("Anthony","canFly",false);
            if (has_permission){
                PRINT_COLORS((GREEN,$"Anthony can fly: {DateTime.Now}\n"));
            }
            else{
                PRINT_COLORS((RED,$"Anthony cannot fly: {DateTime.Now}\n"));
            }
            Thread.Sleep(1000);
        }
    }
}