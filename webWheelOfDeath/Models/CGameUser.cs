namespace webWheelOfDeath.Models;
using LibWheelOfDeath;


public class CGameUser : CCredentials
{
    public CGameUser(string username, string password) : base(username, password) {}

    public string txtPlayerFirstName { get; set; } = string.Empty;
    public string txtPlayerLastName { get; set; } = string.Empty;
}
