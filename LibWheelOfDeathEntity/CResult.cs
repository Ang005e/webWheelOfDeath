using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibEntity.NetCore.Annotations;
using LibEntity.NetCore.Infrastructure;
using LibEntity;
using Microsoft.Data.SqlClient;
using LibEntity.NetCore.Exceptions;

namespace LibWheelOfDeath
{

    public enum EnumResultType
    {
        Default,
        // Stopped,
        // Running,
        Won,
        Killed,
        Timed_Out,
        Exceeded_Throws
    }

    public class CResult : CEntity
    {
        #region Constructors

        public CResult() : base("tblResult") { }

        public CResult(long id) : this()
        {
            Read(id);
        }

        #endregion

        #region Table Column Properties

        // [DataProp]
        public bool? IsWin { get; set; }

        [DataProp]
        public EnumResultType ResultType { get; set; }

        #endregion

        #region Table Entity Properties


        #endregion

        #region Other Properties

        #endregion

        #region CRUDS

        public override void Create()
        {
            CommandText = $@"
                insert into [tblResult]
                (
                    [IsWin],
                    [ResultType]
                )
                values
                (
                    @pIsWin,
                    @ResultType
                );
            ";

            Parameters.AddWithValue("@pIsWin", IsWin);
            Parameters.AddWithValue("@ResultType", ResultType);

            base.Create();
        }

        public override int Update()
        {

            CommandText = $@" 
            update 
                [tblResult]
            set
                [IsWin] = @pIsWin,
				[ResultType] = @ResultType
            where
                Id = @pId
            ";

            Parameters.AddWithValue("@pId", Id);
            Parameters.AddWithValue("@pIsWin", IsWin);
            Parameters.AddWithValue("@ResultType", ResultType);

            return base.Update();
        }


        public override List<IEntity> Search()
        {

            string fromClause = "[tblResult] R ";
            string whereClause = "(1=1) ";

            if (this.NotDefaultValue(Id))
            {
                whereClause += @$"and R.Id = @pId ";
                Parameters.AddWithValue("@pId", this.Id);
            }

            if (this.NotDefaultValue(IsWin))
            {
                whereClause += $"and R.IsWin = @pIsWin ";
                Parameters.AddWithValue("@pIsWin", $"{this.IsWin}");
            }

            if (ResultType != EnumResultType.Default)
            {
                whereClause += $"and R.ResultType = @ResultType ";
                Parameters.AddWithValue("@ResultType", $"{this.ResultType}");
            }

            // TODO: Criteria for duration, minballoons, maxballoons.


            CommandText = @$"
                select 
                    R.* 
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
        /// Get a CResult entity based on the resultType field (this field is unique).
        /// </summary>
        /// <param name="rt"></param>
        /// <returns></returns>
        public CResult GetWithResultType(EnumResultType rt)
        {
            CResult res = new() { ResultType = rt };
            List<IEntity> resultType = res.Search();
            return (CResult)resultType.First();
        }

        public override void Reset()
        {
            Id = 0L;
            IsWin = null;
            ResultType = EnumResultType.Default;
        }


        public override LibEntity.IEntity Populate(SqlDataReader reader, LibEntity.IEntity? entity = null) //IEntity Populate(SqlDataReader reader, IEntity? entity = null)
        {

            CResult result = (CResult?)entity ?? new CResult();

            result.Id = (long)reader["Id"];
            result.IsWin = (bool)reader["IsWin"];
            result.ResultType = (EnumResultType)reader["ResultType"];

            return result;
        }


        public override void Validate()
        {
            CValidator<CResult> validator = new(this);

            validator.NoDefaults();

            if (ResultType == EnumResultType.Default) // added rule manually because Enums are tricky.
                validator.ManualAddFailure(EnumValidationFailure.NotSet, $"{nameof(ResultType)} must be set");

            validator.Validate();

        }

        public override string ToString()
        {
            return $"{this.Id}: {this.ResultType}";
        }
        #endregion
    }
}