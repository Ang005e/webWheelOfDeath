using System.Security.AccessControl;
using LibWheelOfDeath;
using LibEntity;
namespace webWheelOfDeath.Models
{
    public abstract class CEntityModel<TEntity> where TEntity : CEntity, new()
    {
        private TEntity _entity { get; set; } = new();

    }
}
