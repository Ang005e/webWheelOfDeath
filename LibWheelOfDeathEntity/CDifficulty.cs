using System.Data;
using LibEntity;
using LibEntity.NetCore.Infrastructure;
using LibWheelOfDeath;
using LibWheelOfDeath.Exceptions;
using Microsoft.Data.SqlClient;

namespace LibWheelOfDeath
{
    /// <summary>
    /// Represents a difficulty level for the game.
    /// </summary>
    public class CDifficulty : CEntity
    {

        #region Constructors

        public CDifficulty() : base("tblDifficulty") { }

        public CDifficulty(long id) : this()
        {
            Read(id);
        }

        #endregion

        #region Table Column Properties

        public string Difficulty { get; set; }

        #endregion

        #region Table Entity Properties


        #endregion

        #region Other Properties


        #endregion

        #region CRUDS

        public override void Create()
        {
            CommandText = $@"
                insert into [tblDifficulty]
                (
                    [Difficulty]
                )
                values
                (
                    @pDifficulty
                );
            ";

            Parameters.AddWithValue("@pDifficulty", Difficulty);

            base.Create();
        }

        public override int Update()
        {

            CommandText = $@" 
            update 
                [tblDifficulty]
            set
				[Difficulty] = @pDifficulty
            where
                Id = @pId
            ";

            Parameters.AddWithValue("@pId", Id);
            Parameters.AddWithValue("@pDifficulty", Difficulty);

            return base.Update();
        }


        public override List<IEntity> Search()
        {

            string fromClause = "[tblDifficulty] D ";
            string whereClause = "(1=1) ";


            if (this.NotDefaultValue(Id))
            {
                whereClause += @$"and D.Id = @pId ";
                Parameters.AddWithValue("@pId", this.Id);
            }

            if (this.NotDefaultValue(Difficulty))
            {
                whereClause += @$"and D.Difficulty like @pDifficulty ";
                Parameters.AddWithValue("@pDifficulty", $"{this.Difficulty}");
            }


            CommandText = @$"
                select 
                    D.* 
                from
                    {fromClause}
                where
                    {whereClause}
            ";


            return base.Search();
        }


        #endregion

        #region Other Methods


        /// <summary>
        /// Retrieves a list of difficulties from the database.
        /// </summary>
        /// <returns>A list of CDifficulty objects.</returns>
        public List<CDifficulty> GetDifficulties()
        {
            string sql = @"
                select
                    [Id], [Difficulty]
                from
                    tblDifficulty
                order by
                    [Id]
            ";
            DataTable table = sql.Fetch<DataTable>();
            List<CDifficulty> difficulties = new List<CDifficulty>();
            foreach (DataRow row in table.Rows)
            {
                CDifficulty difficulty = new()
                {
                    Id = (long)row["Id"],
                    Difficulty = (string)row["Difficulty"],
                };
                difficulties.Add(difficulty);
            }
            return difficulties;
        }


        public override void Reset()
        {
            Id = 0L;
            Difficulty = string.Empty;
        }

        public override LibEntity.IEntity Populate(SqlDataReader reader, LibEntity.IEntity? entity = null) //IEntity Populate(SqlDataReader reader, IEntity? entity = null)
        {

            CDifficulty difficulty = (CDifficulty?)entity ?? new CDifficulty();

            difficulty.Id = (long)reader["Id"];
            difficulty.Difficulty = reader["Difficulty"] as string ?? string.Empty;

            return difficulty;
        }

        public override void Validate()
        {
            CValidator<CDifficulty> validator = new(this);

            validator.NoDefaults();
            validator.Validate();
        }

        public override string ToString()
        {
            return $"{Id}: {Difficulty}";
        }
        #endregion

    }
}
