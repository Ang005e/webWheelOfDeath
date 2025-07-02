namespace webWheelOfDeath.Models.Infrastructure
{
    public interface IAdminAccountData : IAccountData
    {
        long AdminTypeId { get; set; }
    }
}
