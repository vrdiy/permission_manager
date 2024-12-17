/*******************************************************************
* Name: Anthony Verdi
* Date: 10/19/24
* 
* The PermissionManager class is a static class that manages permissions,
* users, and groups.
*/

// Permission Management Library

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections;
using static ConsoleExt;
namespace PermissionManagerCore
{
    // TODO considering exceptions instead of errors
    public enum LoadError{
        None = 0,
        InvalidInput,
        DoesNotExist,
        SqliteError
    }
    public class PermissionManagerNotInitialized : Exception { }
    public static class PermissionManager
    {
        public const uint MAX_PERMS = 64; // Should be a power of 2 and >= 8
        private static string db = null!;
        private static MiniLogger logger = new MiniLogger(nameof(PermissionManager));
        private static bool hasInit = false;

        /// <summary>
        /// Currently just sets a flag, in future may be used to set up various 
        /// systems. 
        /// </summary>
        public static void Init()
        {
            hasInit = true;
        }

        public static void EnsureInit()
        {
            if (!hasInit)
            {
                throw new PermissionManagerNotInitialized();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db_path"></param>
        /// <returns>
        /// <para>0: Success</para>
        /// <para>-1: database does not exist</para>
        /// <para>-2: Sqlite Exception was thrown, check logs</para>
        /// <para>-3: db_path was empty</para>
        ///
        /// </returns>
        public static LoadError Load(string db_path)
        {
            EnsureInit();
            db_path = db_path.Replace(" ", "");
            if (string.IsNullOrWhiteSpace(db_path))
            {
                logger.LogError("Database name cannot be empty string");
                return LoadError.InvalidInput;
            }
            using (var context = new PermissionContext(db_path))
            {
                context.Database.SetCommandTimeout(new TimeSpan(0,0,5));
                bool can_connect = false;
                try
                {
                    can_connect = context.Database.CanConnect();
                    can_connect = can_connect && context.GetService<IRelationalDatabaseCreator>().HasTables();
                    if (can_connect)
                    {
                        db = db_path;
                        return LoadError.None;
                    }
                    else
                    {
                        logger.LogError($"Database {db_path} does not exist");
                        return LoadError.DoesNotExist;
                    }
                }
                catch (Microsoft.Data.Sqlite.SqliteException e) // TODO
                {
                    switch (e.SqliteErrorCode){
                        case 5: // BUSY
                            break;
                        default:
                            break;
                    }
                    logger.LogError(e.Message); 
                    return LoadError.SqliteError;
                }
            }
        }
        public static int Create(string db_path)
        {
            EnsureInit();
            db_path = db_path.Replace(" ", "");
            if (string.IsNullOrWhiteSpace(db_path))
            {
                logger.LogError("Database name cannot be empty string");
                return -1;
            }
            using (var context = new PermissionContext(db_path))
            {
                context.Database.Migrate();
            }
            return 0;
        }
        public static int AddPerm(string perm_name)
        {
            EnsureInit();
            if (GetPermId(perm_name) != -1)
            {
                logger.LogError($"Permissions.perms already contains {perm_name}");
                return -1;
            }
            using (var context = new PermissionContext(db))
            {
                var num_perms = (
                    from perms in context.Permissions
                    select perms.Name).Count();
                if (num_perms >= MAX_PERMS)
                {
                    logger.LogError($"Permissions.perms is full");
                    return -1;
                }
                else
                {
                    var all_possible_ids = Enumerable.Range(1,(int)MAX_PERMS).ToList();
                    var unavailable_ids = from perm in context.Permissions
                        select perm.Id;
                    var first_available_id = all_possible_ids.Except(unavailable_ids).First();
                    Permission new_perm = new Permission(first_available_id,perm_name);
                    context.Permissions.Add(new_perm);
                    context.SaveChanges();
                    return 0;
                }
            }
        }
        public static int DeletePerm(string perm_name)
        {
            EnsureInit();
            if (GetPermId(perm_name) == -1)
            {
                logger.LogError($"Permissions.perms has no permission {perm_name}");
                return -1;
            }
            using (var context = new PermissionContext(db))
            {
                var perm_to_del = (
                    from perm in context.Permissions
                    where perm.Name == perm_name
                    select perm).First();
                context.Permissions.Remove(perm_to_del);

                // Update Users and Groups to unset that permission
                var users = from user in context.Users
                            select user;
                foreach (PermUser user in users)
                {
                    user.SetPerm((uint)perm_to_del.Id - 1, false);
                    context.Entry(user).State = EntityState.Modified;
                }
                var groups = from g in context.Groups
                             select g;
                foreach (PermGroup group in groups)
                {
                    group.SetPerm((uint)perm_to_del.Id - 1, false);
                    context.Entry(group).State = EntityState.Modified;
                }
                context.SaveChanges();
                return 0;
            }
        }
        public static int AddUser(string user_name)
        {
            EnsureInit();
            // Need empty check because UserExists returns false on empty strings
            if (string.IsNullOrWhiteSpace(user_name))
            {
                logger.LogError($"User name can not be an emtpy string");
                return -1;
            }
            if (UserExists(user_name))
            {
                logger.LogError($"Permissions.users already contains {user_name}");
                return -1;
            }
            using (var context = new PermissionContext(db))
            {
                context.Users.Add(new PermUser(user_name));
                context.SaveChanges();
                return 0;
            }
        }
        public static int DeleteUser(string user_name)
        {
            EnsureInit();
            if (!UserExists(user_name))
            {
                logger.LogError($"User: {user_name} does not exist");
                return -1;
            }

            using (var context = new PermissionContext(db))
            {
                var user_to_del = (from user in context.Users
                                   where user.Name == user_name
                                   select user).Include(u => u.Groups).First();
                context.Users.Remove(user_to_del);
                context.SaveChanges();
                return 0;
            }
        }
        public static int AddGroup(string group_name)
        {
            EnsureInit();
            // Need empty check because GroupExists returns false on empty strings
            if (string.IsNullOrWhiteSpace(group_name))
            {
                logger.LogError($"Group name can not be an emtpy string");
                return -1;
            }
            if (GroupExists(group_name))
            {
                logger.LogError($"Permissions.groups already contains {group_name}");
                return -1;
            }
            using (var context = new PermissionContext(db))
            {
                context.Groups.Add(new PermGroup(group_name));
                context.SaveChanges();
                return 0;
            }
        }
        public static int DeleteGroup(string group_name)
        {
            EnsureInit();
            // Need empty check because GroupExists returns false on empty strings
            // Including this check provides clearer logging
            if (string.IsNullOrWhiteSpace(group_name))
            {
                logger.LogError($"Group name can not be an emtpy string");
                return -1;
            }
            if (!GroupExists(group_name))
            {
                logger.LogError($"Group: {group_name} does not exist");
                return -1;
            }

            using (var context = new PermissionContext(db))
            {
                var group_to_del = (from g in context.Groups
                                    where g.Name == group_name
                                    select g).First();
                context.Groups.Remove(group_to_del);
                context.SaveChanges();
                return 0;
            }
        }
        public static int SetUserPerm(string user_name, string perm_name, bool value)
        {
            EnsureInit();
            if (!UserExists(user_name))
            {
                logger.LogError($"User: {user_name} does not exist");
                return -1;
            }
            if (-1 == GetPermId(perm_name))
            {
                logger.LogError($"Permission: {perm_name} does not exist");
                return -1;
            }
            using (var context = new PermissionContext(db))
            {
                var user_to_update = (from user in context.Users
                                      where user.Name == user_name
                                      select user).Include(u => u.Groups).First();
                var perm_to_change = (from perm in context.Permissions
                                      where perm.Name == perm_name
                                      select perm).First();
                user_to_update.SetPerm((uint)perm_to_change.Id - 1, value);
                context.Entry(user_to_update).State = EntityState.Modified;
                context.SaveChanges();
                return 0;
            }
        }
        public static int SetGroupPerm(string group_name, string perm_name, bool value)
        {
            EnsureInit();
            if (!GroupExists(group_name))
            {
                logger.LogError($"Group: {group_name} does not exist");
                return -1;
            }
            if (-1 == GetPermId(perm_name))
            {
                logger.LogError($"Permission: {perm_name} does not exist");
                return -1;
            }
            using (var context = new PermissionContext(db))
            {
                var group_to_update = (from g in context.Groups
                                       where g.Name == group_name
                                       select g).First();
                var perm_to_change = (from perm in context.Permissions
                                      where perm.Name == perm_name
                                      select perm).First();
                group_to_update.SetPerm((uint)perm_to_change.Id - 1, value);
                context.Entry(group_to_update).State = EntityState.Modified;
                context.SaveChanges();
                return 0;
            }
        }
        /// <summary>
        ///  Set group permissions from a bit mask.
        /// This method should almost never be used as you may
        /// grant a permission's bit before a permission actually uses it.
        /// Proceed with absolute caution
        /// </summary>
        public static int SetGroupPermFromMask(string group_name, BitArray mask)
        {
            EnsureInit();
            if (!GroupExists(group_name))
            {
                logger.LogError($"Group: {group_name} does not exist");
                return -1;
            }
            using (var context = new PermissionContext(db))
            {
                var group_to_update = (from g in context.Groups
                                       where g.Name == group_name
                                       select g).First();
                group_to_update.SetPermsFromMask(mask);
                context.SaveChanges();
                return 0;
            }
        }
        public static int AddUserToGroup(string user_name, string group_name)
        {
            EnsureInit();
            if (!UserExists(user_name))
            {
                logger.LogError($"User: {user_name} does not exist");
                return -1;
            }
            if (!GroupExists(group_name))
            {
                logger.LogError($"Group: {group_name} does not exist");
                return -1;
            }
            using (var context = new PermissionContext(db))
            {
                var user_to_update = (from user in context.Users
                                      where user.Name == user_name
                                      select user).Include(u => u.Groups).First();
                var group_to_update = (from g in context.Groups
                                       where g.Name == group_name
                                       select g).First();
                user_to_update.Groups.Add(group_to_update);
                context.SaveChanges();
                return 0;
            }

        }
        public static int RemoveUserFromGroup(string user_name, string group_name)
        {
            EnsureInit();
            if (!UserExists(user_name))
            {
                logger.LogError($"User: {user_name} does not exist");
                return -1;
            }
            if (!GroupExists(group_name))
            {
                logger.LogError($"Group: {group_name} does not exist");
                return -1;
            }
            using (var context = new PermissionContext(db))
            {
                var user_to_update = (from user in context.Users
                                      where user.Name == user_name
                                      select user).Include(u => u.Groups).First();
                var group_to_update = (from g in context.Groups
                                       where g.Name == group_name
                                       select g).First();
                user_to_update.Groups.Remove(group_to_update);
                context.SaveChanges();
                return 0;
            }
        }
        // This function will grant the user the same permissions as the group
        // Note that this method does not add them to the group
        // Prefer AddUserToGroup() if you wish to add them to the group
        public static int SetUserPermFromGroup(string user_name, string group_name)
        {
            EnsureInit();
            if (!UserExists(user_name))
            {
                logger.LogError($"User: {user_name} does not exist");
                return -1;
            }
            if (!GroupExists(group_name))
            {
                logger.LogError($"Group: {group_name} does not exist");
                return -1;
            }
            using (var context = new PermissionContext(db))
            {
                var user_to_update = (from user in context.Users
                                      where user.Name == user_name
                                      select user).First();
                var group = (from g in context.Groups
                             where g.Name == group_name
                             select g).First();
                user_to_update.AddPermsFromMask(group.PermMask);
                context.Entry(user_to_update).State = EntityState.Modified;
                context.SaveChanges();
                return 0;
            }
        }

        // use_groups if false, means to not consider the user's groups in the check
        public static bool GetUserPerm(string user_name, string perm_name, bool use_groups = true)
        {
            EnsureInit();
            if (!UserExists(user_name))
            {
                logger.LogError($"User: {user_name} does not exist");
                return false;
            }
            int perm_id = GetPermId(perm_name);
            if (-1 == perm_id)
            {
                logger.LogError($"Permission: {perm_name} does not exist");
                return false;
            }
            using (var context = new PermissionContext(db))
            {
                var user_to_check = (from user in context.Users
                                     where user.Name == user_name
                                     select user).Include(u => u.Groups).First();
                return CheckUser(user_to_check, (uint)perm_id - 1, use_groups);
            }

        }
        public static bool GetGroupPerm(string group_name, string perm_name)
        {
            EnsureInit();
            if (!GroupExists(group_name))
            {
                logger.LogError($"Group: {group_name} does not exist");
                return false;
            }
            int perm_id = GetPermId(perm_name);
            if (-1 == perm_id)
            {
                logger.LogError($"Permission: {perm_name} does not exist");
                return false;
            }
            using (var context = new PermissionContext(db))
            {
                var group_to_check = (from g in context.Groups
                                      where g.Name == group_name
                                      select g).First();
                return CheckGroup(group_to_check, (uint)perm_id - 1);
            }
        }
        public static void PrintUser(string user_name, ConsoleColor color = GREEN)
        {
            EnsureInit();
            if (!UserExists(user_name))
            {
                logger.LogError($"User: {user_name} does not exist");
                return;
            }
            using (var context = new PermissionContext(db))
            {
                var user_to_check = (from user in context.Users
                                     where user.Name == user_name
                                     select user).Include(u => u.Groups).First();
                user_to_check.PrintInfoColored(color);
            }

        }
        private static bool CheckUser(PermUser perm_user, uint bit_position, bool use_groups = true)
        {
            EnsureInit();
            if (!use_groups)
            {
                return perm_user.PermMask.Get((int)bit_position);
            }
            BitArray mask = new BitArray((int)perm_user.MaxPerms, false);
            foreach (PermGroup g in perm_user.Groups)
            {
                mask.Or(g.GetPermMask());
            }
            mask.Or(perm_user.PermMask);

            return mask.Get((int)bit_position);
        }
        private static bool CheckGroup(PermGroup perm_group, uint bit_position)
        {
            EnsureInit();
            return perm_group.PermMask.Get((int)bit_position);
        }

        public static void PrintUserPerms(string user_name)
        {
            EnsureInit();
            if (!UserExists(user_name))
            {
                logger.LogError($"{user_name + ": ",-16} does not exist");
                return;
            }
            using (var context = new PermissionContext(db))
            {

                var user_to_check = (from user in context.Users
                                     where user.Name == user_name
                                     select user).Include(u => u.Groups).First();
                var perms = from perm in context.Permissions
                            select perm;
                string allowed_perms = "";
                string group_allowed_perms = "";
                string disallowed_perms = "";
                foreach (Permission perm in perms)
                {
                    bool allowed_only_by_group = CheckUser(user_to_check, (uint)perm.Id - 1, true) ^ CheckUser(user_to_check, (uint)perm.Id - 1, false);
                    if (allowed_only_by_group)
                    {
                        group_allowed_perms += $"{perm.Name} ";
                    }
                    else
                    {
                        if (CheckUser(user_to_check, (uint)perm.Id - 1, false))
                        {
                            allowed_perms += $"{perm.Name} ";
                        }
                        else
                        {
                            disallowed_perms += $"{perm.Name} ";
                        }
                    }
                }
                PRINT_COLORS(
                    (GREEN, $"{"via self:",-16}{allowed_perms}\n"),
                    (DARK_GREEN, $"{"via Group(s): ",-16}{group_allowed_perms}\n"),
                    (RED, $"{"Not Permitted:",-16}{disallowed_perms}\n")
                );
            }
            return;
        }
        public static void PrintGroupPerms(string group_name)
        {
            EnsureInit();
            if (!GroupExists(group_name))
            {
                logger.LogError($"Group: {group_name} does not exist");
                return;
            }
            string allowed_perms = "";
            string disallowed_perms = "";
            using (var context = new PermissionContext(db))
            {
                var perms = from perm in context.Permissions
                            select perm;
                var group_to_check = (from g in context.Groups
                                      where g.Name == group_name
                                      select g).First();
                foreach (Permission perm in perms)
                {
                    if (CheckGroup(group_to_check, (uint)perm.Id - 1))
                    {
                        allowed_perms += $"{perm.Name} ";
                    }
                    else
                    {
                        disallowed_perms += $"{perm.Name} ";
                    }
                }
                PRINT_COLORS(
                    (GREEN, $"{"Permitted:",-16}{allowed_perms}\n"),
                    (RED, $"{"Not Permitted:",-16}{disallowed_perms}\n")
                );
            }
            return;
        }
        public static List<string> GetUserGroups(string user_name)
        {
            EnsureInit();
            List<string> groups = new List<string>();

            if (!UserExists(user_name))
            {
                logger.LogError($"{user_name + ": ",-16} does not exist");
                return groups;
            }
            using (var context = new PermissionContext(db))
            {
                var user_to_check = (from user in context.Users
                                     where user.Name == user_name
                                     select user).Include(u => u.Groups).First();
                var group_names = (from g in user_to_check.Groups
                                   select g.Name).ToList();
                return group_names;
            }
        }
        public static void PrintUsers()
        {
            EnsureInit();
            using (var context = new PermissionContext(db))
            {
                var users = from user in context.Users
                            select user;
                foreach (PermUser user in users)
                {
                    user.PrintInfo();
                }
            }
        }
        public static void PrintUsersColored(ConsoleColor color)
        {
            EnsureInit();
            using (var context = new PermissionContext(db))
            {
                var users = from user in context.Users
                            select user;
                foreach (PermUser user in users)
                {
                    user.PrintInfoColored(color);
                }
            }
        }
        public static void PrintGroups()
        {
            EnsureInit();
            using (var context = new PermissionContext(db))
            {
                var groups = from g in context.Groups
                             select g;
                foreach (PermGroup group in groups)
                {
                    group.PrintInfoColored(GREEN);
                }
            }

        }
        public static void PrintPerms(uint columns = 8)
        {
            EnsureInit();
            using (var context = new PermissionContext(db))
            {
                var perms = from perm in context.Permissions
                            select perm;
                string perms_str = "";
                int count = 0;
                foreach (Permission perm in perms)
                {
                    perms_str += $"{perm.Name}".PadRight(16);
                    if (((count + 1) % columns) == 0) { perms_str += "\n"; }
                    count++;
                }
                PRINT_COLORS((GREEN, $"{perms_str}\n"));
            }
        }
        public static int GetNumberOfUsers()
        {
            EnsureInit();
            using (var context = new PermissionContext(db))
            {
                var user_count = (from user in context.Users
                                  select user).Count();
                return user_count;
            }
        }
        public static int GetNumberOfGroups()
        {
            EnsureInit();
            using (var context = new PermissionContext(db))
            {
                var group_count = (from g in context.Groups
                                   select g).Count();
                return group_count;
            }
        }
        public static int GetNumberOfPerms()
        {
            EnsureInit();
            using (var context = new PermissionContext(db))
            {
                var perm_count = (from perm in context.Permissions
                                  select perm).Count();
                return perm_count;
            }
        }
        public static string GetActiveDatabase()
        {
            EnsureInit();
            if (null != db)
            {
                return db;
            }
            else
            {
                return "None";
            }
        }
        // Returns index of PermUser in m_users, returns -1 if not found
        public static bool UserExists(string user_name)
        {
            EnsureInit();
            if (string.IsNullOrWhiteSpace(user_name))
            {
                logger.LogError($"User name can not be an emtpy string");
                return false;
            }
            using (var context = new PermissionContext(db))
            {
                try
                {
                    var user_to_check = (from user in context.Users
                                         where user.Name == user_name
                                         select user).First();
                    return true;
                }
                catch (InvalidOperationException e)
                {
                    logger.LogError(e.Message);
                    return false;
                }
            }
        }
        public static bool GroupExists(string group_name)
        {
            EnsureInit();
            if (string.IsNullOrWhiteSpace(group_name))
            {
                logger.LogError($"Group name can not be an emtpy string");
                return false;
            }
            using (var context = new PermissionContext(db))
            {
                try
                {
                    var group_to_check = (from g in context.Groups
                                          where g.Name == group_name
                                          select g).First();
                    return true;
                }
                catch (InvalidOperationException e)
                {
                    logger.LogError(e.Message);
                    return false;
                }
            }
        }
        public static int GetPermId(string perm_name)
        {
            EnsureInit();
            if (string.IsNullOrWhiteSpace(perm_name))
            {
                logger.LogError($"Perm name can not be an emtpy string");
                return -1;
            }
            using (var context = new PermissionContext(db))
            {
                try
                {
                    return (from perm in context.Permissions
                            where perm.Name == perm_name
                            select perm.Id).First();
                }
                catch (InvalidOperationException e)
                {
                    logger.LogError(e.Message);
                    return -1;
                }
            }
        }
        public static void WriteLog(){
            logger.ExportLog();
        }
    }
}