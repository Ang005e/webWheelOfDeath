namespace webWheelOfDeath.Models;
using LibWheelOfDeath;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

public class CGameUser : CPlayerCredentials
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public void Register()
    {
        CPlayer newPlayer = BuildPlayer();

        newPlayer.Validate();

        newPlayer.Create();
    }

    public bool UsernameExists()
    {
        CPlayer newPlayer = BuildPlayer();
        return newPlayer.UsernameExists();
    }

    public CPlayer BuildPlayer()
    {
        return new CPlayer(
            FirstName,
            LastName,
            Username,
            Password
        );
    }
}
