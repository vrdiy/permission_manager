/*******************************************************************
* Name: Anthony Verdi
* Date: 10/19/24
* 
* The menu interface enforces the implementation of Start().
* IMenu implementors should return an ExitMenu object to remove
* themselves from the menu stack that IMenu.Run() manages.
*/

// IMenus 
interface IMenu
{
    public static void Run(IMenu starting_menu)
    {
        Stack<IMenu> menus = new() { };
        menus.Push(starting_menu);
        while (menus.Count != 0)
        {
            // Check result of Start() on top menu
            IMenu new_menu = menus.First().Start();
            if (new_menu.GetType() != typeof(ExitMenu))
            {
                menus.Push(new_menu);
            }
            else
            { // Pop current menu if it returned an ExitMenu
                menus.Pop();
            }
        }
    }
    public IMenu Start();
}