/*******************************************************************
* Name: Anthony Verdi
* Date: 10/24/24
* 
*/
using System.Security.Cryptography.X509Certificates;

namespace PermissionManagerCore
{

    internal class Permission
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public Permission(string name)
        {
            Name = name;
        }

        public Permission(int id, string name){
            Id = id;
            Name = name;
        }

    }
}
