/*******************************************************************
* Name: Anthony Verdi
* Date: 10/19/24
* 
* PermObjects are the basis of permission management, they 
* consist of an identifier(Name) and a bitmask(PermMask).
*/

using System.Collections;
using static ConsoleExt;
namespace PermissionManagerCore
{

    internal abstract class PermObject
    {
        public string Name { get; private set; }
        // PermMask should be thought of as a 0-indexed right-to-left bit array
        // if m_max_perms was 16 and you set bit 3 to true, the resulting 
        // BitArray would look like: 0b00000000 00001000
        public BitArray PermMask { get; private set; } // Should be size of Permissions.MAX_PERMS
        public uint MaxPerms { get; private set; } = PermissionManager.MAX_PERMS; // Temp
        public PermObject(string name)
        {
            Name = name;
            PermMask = new BitArray((int)MaxPerms, false);
        }
        public BitArray GetPermMask()
        {
            return PermMask;
        }
        public int SetPerm(uint bit_position, bool value = true)
        {
            if (bit_position >= MaxPerms)
            {
                Console.WriteLine($"Out of bound bit position: {bit_position} >= {MaxPerms}");
                return -1;
            }
            else
            {
                PermMask.Set((int)bit_position, value);
                return 0;
            }
        }
        // This method completely overwrites the current permission mask
        // Should be used with careful consideration
        public int SetPermsFromMask(BitArray bit_mask)
        {
            if (bit_mask.Length != PermMask.Length)
            {
                ERROR($"Length of bit_mask is not the same as PermMask");
                return -1;
            }
            PermMask = bit_mask;
            return 0;
        }
        // ORs the values from the provided mask
        // To explicitly set the mask use SetPermsFromMask()
        public int AddPermsFromMask(BitArray bit_mask)
        {
            if (bit_mask.Length != PermMask.Length)
            {
                ERROR($"Length of bit_mask is not the same as PermMask");
                return -1;
            }
            PermMask.Or(bit_mask);
            return 0;
        }
        public void PrintInfo()
        {
            string perm_bits = "";
            for (int i = (int)MaxPerms - 1; i >= 0; i -= 8)
            {
                perm_bits += " ";
                perm_bits += Convert.ToInt32(PermMask[i]);
                perm_bits += Convert.ToInt32(PermMask[i - 1]);
                perm_bits += Convert.ToInt32(PermMask[i - 2]);
                perm_bits += Convert.ToInt32(PermMask[i - 3]);
                perm_bits += Convert.ToInt32(PermMask[i - 4]);
                perm_bits += Convert.ToInt32(PermMask[i - 5]);
                perm_bits += Convert.ToInt32(PermMask[i - 6]);
                perm_bits += Convert.ToInt32(PermMask[i - 7]);
            }
            Console.WriteLine($"{Name + ": ",-16}{perm_bits}");
        }

        public void PrintInfoColored(ConsoleColor color)
        { // Slower than PrintInfo(), but pretty
            Console.Write($"{Name + ": ",-16} ");
            bool last_val = PermMask[0];
            string zeros_or_ones = ""; // Used to write contiguous sections of the mask in same color
            for (int i = (int)MaxPerms - 1; i >= 0; i--)
            {
                if (PermMask[i] != last_val)
                {
                    if (last_val)
                    { // values in zeros_or_ones are 1s, should be drawn in color
                        Console.ForegroundColor = color;
                    }
                    Console.Write(zeros_or_ones);
                    zeros_or_ones = "";
                    Console.ResetColor();
                }
                zeros_or_ones += Convert.ToInt32(PermMask[i]);
                if (i % 8 == 0)
                { // Add a space between bytes
                    zeros_or_ones += " ";
                }
                last_val = PermMask[i];
            }
            // Write remainder of values
            if (last_val)
            { // values in zeros_or_ones are 1s, should be drawn in color
                Console.ForegroundColor = color;
            }
            Console.Write(zeros_or_ones);
            Console.ResetColor();
            Console.Write("\n");
        }
    }
}