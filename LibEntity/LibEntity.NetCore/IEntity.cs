
namespace LibEntity
{
    public interface IEntity
    {
        long Id { get; set; }
        bool IsAddMode { get; }

        void Create();
        int Delete();
        void Read();
        void Save();
        List<IEntity> Search();
        int Update();
    }
}