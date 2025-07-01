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

namespace LibWheelOfDeath
{
    public class CGameRecord : CEntity
    {

        #region Constructors

        public CGameRecord() : base("tblGameRecord") { }

        public CGameRecord(long id) : this()
        {
            Read(id);
        }

        #endregion

        #region Table Column Properties

        [DataProp]
        public long FkGameId { get; set; }

        [DataProp]
        public long FkPlayerId { get; set; }

        [DataProp]
        public long FkResultId { get; set; }

        [DataProp]
        public DateTime? Date { get; set; }

        [DataProp]
        public long ElapsedTime { get; set; }

        [DataProp]
        public short BalloonsPopped { get; set; }

        [DataProp]
        public short Misses { get; set; }

        #endregion

        #region Table Entity Properties


        #endregion

        #region Other Properties


        #endregion

        #region CRUDS

        public override void Create()
        {
            CommandText = $@"
                insert into [tblGameRecord]
                (
                    [FkGameId],
                    [FkPlayerId],
                    [FkResultId],
                    [Date],
                    [ElapsedTime],
                    [BalloonsPopped],
                    [Misses]
                )
                values
                (
                    @pFkGameId,
                    @pFkPlayerId,
                    @pFkResultId,
                    @pDate,
                    @pElapsedTime,
                    @pBalloonsPopped,
                    @pMisses
                );
            ";

            Parameters.AddWithValue("@pFkGameId", FkGameId);
            Parameters.AddWithValue("@pFkPlayerId", FkPlayerId);
            Parameters.AddWithValue("@pFkResultId", FkResultId);
            Parameters.AddWithValue("@pDate", Date);
            Parameters.AddWithValue("@pElapsedTime", ElapsedTime);
            Parameters.AddWithValue("@pBalloonsPopped", BalloonsPopped);
            Parameters.AddWithValue("@pMisses", Misses);

            base.Create();
        }

        public override int Update()
        {

            CommandText = $@" 
            update 
                [tblGameRecord]
            set
                [FkGameId] = @pFkGameId,
				[FkPlayerId] = @pFkPlayerId,
				[FkResultId] = @pFkResultId,
				[Date] = @pDate,
                [ElapsedTime] = @pElapsedTime,
                [BalloonsPopped] = @pBalloonsPopped,
                [Misses] = @pMisses
            where
                Id = @pId
            ";

            Parameters.AddWithValue("@pId", Id);
            Parameters.AddWithValue("@pFkGameId", FkGameId);
            Parameters.AddWithValue("@pFkPlayerId", FkPlayerId);
            Parameters.AddWithValue("@pFkResultId", FkResultId);
            Parameters.AddWithValue("@pDate", Date);
            Parameters.AddWithValue("@pElapsedTime", ElapsedTime);
            Parameters.AddWithValue("@pBalloonsPopped", BalloonsPopped);
            Parameters.AddWithValue("@pMisses", Misses);

            return base.Update();
        }


        public override List<IEntity> Search()
        {

            string fromClause = "[tblGameRecord] R ";
            string whereClause = "(1=1) ";


            if (this.NotDefaultValue(Id))
            {
                whereClause += @$"and R.Id = @pId ";
                Parameters.AddWithValue("@pId", this.Id);
            }
            if (this.NotDefaultValue(FkGameId))
            {
                whereClause += $"and R.FkGameId like @pFkGameId ";
                Parameters.AddWithValue("@pFkGameId", $"{this.FkGameId}");
            }
            if (this.NotDefaultValue(FkPlayerId))
            {
                whereClause += $"and R.FkPlayerId like @pFkPlayerId ";
                Parameters.AddWithValue("@pFkPlayerId", $"{this.FkPlayerId}");
            }
            if (this.NotDefaultValue(FkResultId))
            {
                whereClause += $"and R.FkResultId like @pFkResultId ";
                Parameters.AddWithValue("@pFkResultId", $"{this.FkResultId}");
            }
            if (Date.HasValue) // date is nullable, so no use of NotDefault()
            {
                whereClause += $"and R.Date = @pDate ";
                Parameters.AddWithValue("@pDate", this.Date.Value);
            }
            if (this.NotDefaultValue(ElapsedTime))
            {
                whereClause += $"and R.ElapsedTime = @pElapsedTime ";
                Parameters.AddWithValue("@pElapsedTime", this.ElapsedTime);
            }
            if (this.NotDefaultValue(BalloonsPopped))
            {
                whereClause += $"and R.BalloonsPopped = @pBalloonsPopped ";
                Parameters.AddWithValue("@pBalloonsPopped", this.BalloonsPopped);
            }
            if (this.NotDefaultValue(Misses))
            {
                whereClause += $"and R.Misses = @pMisses ";
                Parameters.AddWithValue("@pMisses", this.Misses);
            }


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


        public override void Reset()
        {
            Id = 0L;
            FkGameId = 0L;
            FkResultId = 0L;
            FkPlayerId = 0L;
            Date = null;
            ElapsedTime = 0L;
            BalloonsPopped = 0;
            Misses = 0;
        }


        public override LibEntity.IEntity Populate(SqlDataReader reader, LibEntity.IEntity? entity = null) //IEntity Populate(SqlDataReader reader, IEntity? entity = null)
        {

            CGameRecord FkPlayerId = (CGameRecord?)entity ?? new CGameRecord();

            FkPlayerId.Id = (long)reader["Id"];
            FkPlayerId.FkGameId = (long)reader["FkGameId"];
            FkPlayerId.FkPlayerId = (long)reader["FkPlayerId"];
            FkPlayerId.FkResultId = (short)reader["FkResultId"];
            FkPlayerId.Date = (DateTime)reader["Date"];
            FkPlayerId.ElapsedTime = (long)reader["ElapsedTime"];
            FkPlayerId.BalloonsPopped = (short)reader["BalloonsPopped"];
            FkPlayerId.Misses = (short)reader["Misses"];

            return FkPlayerId;
        }
        
        public static CGameRecord PopulateDataRow(DataRow row)
        {
            CGameRecord record = new();
            record.Id = (long)row["Id"];
            record.FkGameId = (long)row["FkGameId"];
            record.FkPlayerId = (long)row["FkPlayerId"];
            record.FkResultId = (long)row["FkResultId"];
            record.Date = (DateTime)row["Date"];
            record.ElapsedTime = (long)row["ElapsedTime"];
            record.BalloonsPopped = (short)row["BalloonsPopped"];
            record.Misses = (short)row["Misses"];
            return record;
        }


        public override void Validate()
        {
            CValidator<CGameRecord> validator = new(this);

            // validator.NoDefaultsExcept(nameof(Misses), nameof(Id));

            validator.Validate();

        }

        public override string ToString()
        {
            return $"{this.Id}: {this.FkPlayerId}";
        }
        #endregion
    }
}
