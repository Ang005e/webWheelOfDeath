namespace webWheelOfDeath.Models;
using LibWheelOfDeath;


public class CGameUser : CPlayerCredentials
{
    public string txtPlayerFirstName { get; set; } = string.Empty;
    public string txtPlayerLastName { get; set; } = string.Empty;

    public void Register()
    {

        CPlayer newPlayer = new(
            txtPlayerFirstName,
            txtPlayerLastName,
            txtPlayerUsername,
            txtPlayerPassword
        );
    }
}
