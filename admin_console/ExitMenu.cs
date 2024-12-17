/*******************************************************************
* Name: Anthony Verdi
* Date: 10/19/24
* 
* This menu is used to signify the exit of a menu. Return an
* ExitMenu object to pop the menu off the stack.
*/
class ExitMenu : IMenu
{
    public IMenu Start() { return this; }
}