using LibEntity;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using LibWheelOfDeath.Exceptions;
using LibEntity.NetCore.Exceptions;
using System.Reflection;
using LibEntity.NetCore.Annotations;
using static LibEntity.ExtensionMethods;
using LibEntity.NetCore.Infrastructure;
using static LibEntity.NetCore.Infrastructure.CValidatorComponents;
using System.Reflection.Metadata.Ecma335;

namespace LibWheelOfDeath
{
    public class CGame : CEntity
    {
        #region Constructors

        public CGame() : base("tblGame") { }

        public CGame(long id) : this()
        {
            // CDefaultSetter.Apply(this);
            Read(id);
        }

        #endregion

        #region Table Column Properties

        [DataProp]
        public long FkDifficultyId { get; set; }

        [DataProp]
        public string Game { get; set; } = null!;

        [DataProp]
        public short Attempts { get; set; }

        [DataProp]
        public short Misses { get; set; }

        [DataProp]
        public long DurationMilliseconds { get; set; }

        [DataProp]
        public short MinBalloons { get; set; }
        
        [DataProp]
        public short MaxBalloons { get; set; }
        
        // [DataProp]
        public bool IsActiveFlag { get; set; }

        #endregion

        #region Table Entity Properties


        #endregion

        #region Other Properties


        #endregion

        #region CRUDS

        public override void Create()
        {
            CommandText = $@"
                insert into [tblGame]
                (
                    [FkDifficultyId],
                    [Game],
                    [Attempts],
                    [Misses],
                    [Duration],
                    [MinBalloons],
                    [IsActiveFlag],
                    [MaxBalloons]
                )
                values
                (
                    @pFkDifficultyId,
                    @pGame,
                    @pAttempts,
                    @pMisses,
                    @pDuration,
                    @pMinBalloons,
                    @pIsActiveFlag,
                    @pMaxBalloons
                );
            ";

            Parameters.AddWithValue("@pFkDifficultyId", FkDifficultyId);
            Parameters.AddWithValue("@pGame", Game);
            Parameters.AddWithValue("@pAttempts", Attempts);
            Parameters.AddWithValue("@pMisses", Misses);
            Parameters.AddWithValue("@pDuration", DurationMilliseconds);
            Parameters.AddWithValue("@pMinBalloons", MinBalloons);
            Parameters.AddWithValue("@pIsActiveFlag", IsActiveFlag);
            Parameters.AddWithValue("@pMaxBalloons", MaxBalloons);

            base.Create();
        }

        public override int Update()
        {

            CommandText = $@" 
            update 
                [tblGame]
            set
                [FkDifficultyId] = @pFkDifficultyId,
				[Game] = @pGame,
				[Attempts] = @pAttempts,
				[Misses] = @pMisses,
                [Duration] = @pDuration,
                [MinBalloons] = @pMinBalloons,
                [IsActiveFlag] = @pIsActiveFlag,
                [MaxBalloons] = @pMaxBalloons
            where
                Id = @pId
            ";

            Parameters.AddWithValue("@pId", Id);
            Parameters.AddWithValue("@pFkDifficultyId", FkDifficultyId);
            Parameters.AddWithValue("@pGame", Game);
            Parameters.AddWithValue("@pAttempts", Attempts);
            Parameters.AddWithValue("@pMisses", Misses);
            Parameters.AddWithValue("@pDuration", DurationMilliseconds);
            Parameters.AddWithValue("@pMinBalloons", MinBalloons);
            Parameters.AddWithValue("@pIsActiveFlag", IsActiveFlag);
            Parameters.AddWithValue("@pMaxBalloons", MaxBalloons);

            return base.Update();
        }


        public override List<IEntity> Search()
        {

            string fromClause = "[tblGame] G ";
            string whereClause = "(1=1) ";


            if (Id != 0L)
            {
                whereClause += @$"and G.Id = @pId ";
                Parameters.AddWithValue("@pId", this.Id);
            }

            if (FkDifficultyId != 0)
            {
                whereClause += $"and G.FkDifficultyId like @pFkDifficultyId ";
                Parameters.AddWithValue("@pFkDifficultyId", $"{this.FkDifficultyId}");
            }

            if (!string.IsNullOrWhiteSpace(Game))
            {
                whereClause += $"and G.Game like @pGame ";
                Parameters.AddWithValue("@pGame", $"{this.Game}");
            }

            if (Attempts > 0)
            {
                whereClause += $"and G.Attempts like @pAttempts ";
                Parameters.AddWithValue("@pAttempts", $"{this.Attempts}");
            }

            // TODO: Criteria for duration, minballoons, maxballoons.


            CommandText = @$"
                select 
                    G.* 
                from
                    {fromClause}
                where
                    {whereClause}
            ";


            return base.Search();
        }


        #endregion

        #region Other Methods

        //public List<CGame> GetAllGames()
        //{
        //    CommandText = @$"select * from tblGame";
        //    List<IEntity> entities = base.Search();

        //    return entities.ConvertAll(game => (CGame)game);
        //}

        /// <summary>
        /// Retrieves games associated with this difficulty.
        /// </summary>
        /// <returns>A list of CGame objects.</returns>
        public static List<CGame> GetGamesByDifficulty()
        {

            string sql = @"
                select
                    G.*, D.[Difficulty], D.[Id]
                from
                    tblGame G inner join
                    tblDifficulty D on D.[Id] = G.[FkDifficultyId]
                where
                    (1=1)
                order by
                    D.[Id],
                    G.[Game]
            ";

            DataTable table = sql.Fetch<DataTable>();
            List<CGame> games = new List<CGame>();
            foreach (DataRow row in table.Rows)
            {
                CGame game = PopulateDataRow(row);
                games.Add(game);
            }
            return games;
        }

        public static List<CGame> GetActiveGames()
        {
            CGame searchGame = new() { IsActiveFlag = true };
            List<IEntity> games = searchGame.Search();
            List<CGame> cGameCast = games.ConvertAll(game => (CGame)game);
            return cGameCast;
        }


        public override void Reset()
        {
            Id = 0L;
            FkDifficultyId = 0L;
            Attempts = 0;
            Game = string.Empty;
            Misses = 0;
            DurationMilliseconds = 0L;
            MinBalloons = 0;
            MaxBalloons = 0;
            IsActiveFlag = true;
        }


        public override LibEntity.IEntity Populate(SqlDataReader reader, LibEntity.IEntity? entity = null) //IEntity Populate(SqlDataReader reader, IEntity? entity = null)
        {

            CGame game = (CGame?)entity ?? new CGame();

            game.Id = (long)reader["Id"];
            game.FkDifficultyId = (long)reader["FkDifficultyId"];
            game.Game = (string)reader["Game"];
            game.Attempts = (short)reader["Attempts"];
            game.Misses = (short)reader["Misses"];
            game.DurationMilliseconds = (long)reader["Duration"];
            game.MinBalloons = (short)reader["MinBalloons"];
            game.IsActiveFlag = (bool)reader["IsActiveFlag"];
            game.MaxBalloons = (short)reader["MaxBalloons"];

            return game;
        }

        // because string.Fetch<>() is easier to use than "using new sqlconnection conn..." (proceeds for 500 lines)
        public static CGame PopulateDataRow(DataRow row)
        {
            CGame game = new()
            {
                Id = (long)row["Id"],
                Game = (string)row["Game"],
                FkDifficultyId = (long)row["FkDifficultyId"],
                Attempts = (short)row["Attempts"],
                Misses = (short)row["Misses"],
                DurationMilliseconds = (long)row["Duration"],
                MinBalloons = (short)row["MinBalloons"],
                IsActiveFlag = (bool)row["IsActiveFlag"],
                MaxBalloons = (short)row["MaxBalloons"]
            };
            return game;
        }


        public override void Validate()
        {

            // ################################################################
            // ################   Bow to my ingenuity,  #######################
            // ################         Majidi         ########################
            // ################################################################


            CValidator<CGame> validator = new(this);



            //CValidatorFailureBuilder builder = new();
            // Check if any properties are still the default value
            // builder = IdentifyDefaultProperties(this, builder);

            //string valueOutOfRangeWarnings = string.Empty;
            //if (MinBalloons < 0)
            //{
            //    builder.AddFailure(EnumValidationFailure.OutOfRange, $"{nameof(MinBalloons)} must be a value greater than zero\n");
            //}

            //if (MaxBalloons < 0)
            //{
            //    builder.AddFailure(EnumValidationFailure.OutOfRange, $"{nameof(MaxBalloons)} must be a value greater than zero\n");
            //}

            //if (Attempts < 0)
            //{
            //    builder.AddFailure(EnumValidationFailure.OutOfRange, $"{nameof(Attempts)} must be a value greater than zero\n");
            //}

            //if (Misses < 0)
            //{
            //    builder.AddFailure(EnumValidationFailure.OutOfRange, $"{nameof(Misses)} must be a value greater than zero\n");
            //}

            //if (MinBalloons > MaxBalloons)
            //{
            //    builder.AddFailure(EnumValidationFailure.InvalidValue, $"{nameof(MinBalloons)} must be less than or equal to {nameof(MaxBalloons)}\n");
            //}
            //if (Attempts < Misses)
            //{
            //    builder.AddFailure(EnumValidationFailure.InvalidValue, $"{nameof(Attempts)} must be greater than or equal to {nameof(Misses)}\n");
            //}
            validator.Less(MinBalloons, MaxBalloons);
            validator.Greater(Attempts, Misses);
            validator.Greater(Attempts, MaxBalloons);

            validator.GreaterConst(MinBalloons, 0);
            validator.GreaterConst(MaxBalloons, 0);
            validator.GreaterConst(Attempts, 0);
            validator.GreaterConst(Misses, 0);

            // validator.NoDefaultsExcept(nameof(IsActiveFlag));
            
            validator.Validate(); //if (builder.ShouldThrow()) throw builder.ValidationException();

        }

        public void Deactivate()
        {
            IsActiveFlag = false;
            Update();
        }

        public void Activate()
        {
            IsActiveFlag = true;
            Update();
        }

        public override string ToString()
        {
            return $"{this.Id}: {this.Game}";
        }
        #endregion
    }
}
