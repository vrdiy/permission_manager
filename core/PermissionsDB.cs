/*******************************************************************
* Name: Anthony Verdi
* Date: 10/26/24
* 
* The PermissionContext class is database context class, which is used
* by the entity framework to create the model for the database.
*/

// Permission Management Library
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections;
namespace PermissionManagerCore
{

    internal class PermissionContext : DbContext
    {

        public DbSet<PermUser> Users => Set<PermUser>();
        public DbSet<PermGroup> Groups => Set<PermGroup>();
        public DbSet<Permission> Permissions => Set<Permission>();

        private byte[] ByteArrFromBitArray(BitArray b)
        {
            byte[] bytes = new byte[PermissionManager.MAX_PERMS / 8];
            b.CopyTo(bytes, 0);
            Array.Reverse(bytes);
            return bytes;
        }
        private BitArray BitArrFromByteArray(byte[] b)
        {
            Array.Reverse(b);
            return new BitArray(b);
        }
        private readonly string _db_path;
        // Needed by EF, but not used in app code
        public PermissionContext()
        {
            _db_path = "default.db";
        }
        public PermissionContext(string db_path)
        {
            _db_path = db_path;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var base_dir = AppDomain.CurrentDomain.BaseDirectory;
            var data_dir = Path.Combine(base_dir, "data");
            Directory.CreateDirectory(data_dir);
            var res = Path.Combine(data_dir, _db_path);
            optionsBuilder.UseSqlite($"Data Source={res}");
        }
        #region Required
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // These could be moved out using Entity configuration: 
            // https://learn.microsoft.com/en-us/ef/core/modeling/#grouping-configuration
            // but not really necessary as this is a single table database^

            var converter = new ValueConverter<BitArray, byte[]>(
                v => ByteArrFromBitArray(v),
                v => BitArrFromByteArray(v)
            );
            var converter1 = new StringToBoolConverter();

            modelBuilder.Entity<PermGroup>(
                group =>
                {
                    group.HasKey(group => group.Name);
                    group.Ignore(group => group.MaxPerms);
                    group.Property(group => group.PermMask)
                        .HasMaxLength((int)PermissionManager.MAX_PERMS)
                        .HasConversion(converter);

                });

            modelBuilder.Entity<PermUser>(
                user =>
                {
                    user.HasKey(user => user.Name);
                    user.Ignore(group => group.MaxPerms);
                    user.HasMany(user => user.Groups)
                        .WithMany(groups => groups.Users)
                        .UsingEntity(
                            "User_Groups",
                            l => l.HasOne(typeof(PermGroup)).WithMany().HasForeignKey("Group_Name").HasPrincipalKey(nameof(PermGroup.Name)),
                            r => r.HasOne(typeof(PermUser)).WithMany().HasForeignKey("User_Name").HasPrincipalKey(nameof(PermUser.Name)),
                            j => j.HasKey("User_Name", "Group_Name"));
                    user.Property(user => user.PermMask)
                        .HasMaxLength((int)PermissionManager.MAX_PERMS/8) 
                        //.len
                        .IsFixedLength()
                        .HasConversion(converter);
                }
            );

            modelBuilder.Entity<Permission>(
                perm =>
                {
                    perm.HasKey(perm => perm.Id);
                    perm.Property(perm => perm.Name);
                }
            );
        }
        #endregion
    }
}
