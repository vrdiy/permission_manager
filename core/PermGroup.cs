/*******************************************************************
* Name: Anthony Verdi
* Date: 10/19/24
* 
* The PermGroup class is a subclass of PermObject that serves to 
* distinguish itself from PermUsers, although their use is very similar.
*/
namespace PermissionManagerCore
{

    internal class PermGroup : PermObject
    {
        public virtual List<PermUser> Users { get; private set; }

        public PermGroup(string name)
            : base(name)
        {
            Users = new List<PermUser>();
        }
    }
}