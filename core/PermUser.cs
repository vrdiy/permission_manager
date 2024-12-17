/*******************************************************************
* Name: Anthony Verdi
* Date: 10/19/24
* 
* PermUsers maintain a list of PermGroup objects, and PermGroup objects
* maintain a list of PermUser objects, forming a many-to-many relationship. 
*/
namespace PermissionManagerCore
{

    internal class PermUser : PermObject
    {
        public virtual List<PermGroup> Groups { get; private set; }
        public PermUser(string name)
            : base(name)
        {
            Groups = new List<PermGroup>();
        }
    }
}