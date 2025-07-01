using LibEntity;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibWheelOfDeath
{
//    public class CTemplate : CEntity
//    {
//        #region Constructors

//        public CTemplate() : base(/* ToDo */ "tblToDoTableName") { }

//        public CTemplate(long id) : this()
//        {
//            Read(id);
//        }

//        #endregion
//        #region Table Column Properties

//        public long ForiegnKey1 { get; set; } = 0L;
////         public long ForiegnKey2 { get; set; } = 0L;
////         public long ForiegnKey3 { get; set; } = 0L;
////         public long ForiegnKey4 { get; set; } = 0L;
////
//        public string RowName1 { get; set; } = string.Empty;

////        public short ShortNumericField1 { get; set; } = 0;
////
////        public short ShortNumericField2 { get; set; } = 0;
////
////        public long LongNumericField1 { get; set; } = 0L;
////
////        public short ShortNumericField3 { get; set; } = 0;
////        public short ShortNumericField4 { get; set; } = 0;

//        #endregion
//        #region Table Entity Properties


//        #endregion
//        #region Other Properties


//        #endregion
//        #region CRUDS

//        public override void Create()
//        {                                   /*
//            CommandText = $@"
//            insert into [tblToDoTableName]
//            (                               
//                [ForiegnKey1],              */
////                [ForiegnKey2],
////                [ForiegnKey3],
////                [ForiegnKey4],
//                                            /*
//                [Username],                 */
////                [ShortNumericField1],
////                [ShortNumericField2],
////                [LongNumericField1],
////                [ShortNumericField3],
////                [ShortNumericField4]
//                                            /*
//            )
//            values
//            (                               */
////                @pForiegnKey1,
////                @pForiegnKey2,
////                @pForiegnKey3,
////                @pForiegnKey4,
////                @pRowName1,
////                @pShortNumericField1Param,
////                @pShortNumericField2Param,
////                @pLongNumericField1Param,
////                @pShortNumericField3Param,
////                @pShortNumericField4Param
//                                            /*
//            );
//        ";                                  */

//            Parameters.AddWithValue("@pForiegnKey1", ForiegnKey1);
////            Parameters.AddWithValue("@pForiegnKey2", ForiegnKey2);
////            Parameters.AddWithValue("@pForiegnKey3", ForiegnKey3);
////            Parameters.AddWithValue("@pForiegnKey4", ForiegnKey4);

//            Parameters.AddWithValue("@pRowName1", RowName1);
////            Parameters.AddWithValue("@pShortNumericField1Param", ShortNumericField1);
////            Parameters.AddWithValue("@pShortNumericField2Param", ShortNumericField2);
////            Parameters.AddWithValue("@pLongNumericField1Param", LongNumericField1);
////            Parameters.AddWithValue("@pShortNumericField3Param", ShortNumericField3);
////            Parameters.AddWithValue("@pShortNumericField4Param", ShortNumericField4);

//            base.Create();
//        }

//        public override int Update()
//        {
//                                                                    /*
//            CommandText = $@" 
//            update 
//                [tblToDoTableName]
//            set
//                [ForiegnKey1] = @pForiegnKey1,
//				[Username] = @pRowName1,                            */
////				[ShortNumericField1] = @pShortNumericField1Param,
////				[ShortNumericField2] = @pShortNumericField2Param, 
////                [LongNumericField1] = @pLongNumericField1Param,
////                [ShortNumericField3] = @pShortNumericField3Param,
////                [ShortNumericField4] = @pShortNumericField4Param
//                                                                    /*
//            where
//                Id = @pId
//            ";                                                      

//            Parameters.AddWithValue("@pId", Id);
//            Parameters.AddWithValue("@pForiegnKey1", ForiegnKey1);
//            Parameters.AddWithValue("@pRowName1", Username);        */
////            Parameters.AddWithValue("@pShortNumericField1Param", ShortNumericField1);
////            Parameters.AddWithValue("@pShortNumericField2Param", ShortNumericField2);
////            Parameters.AddWithValue("@pLongNumericField1Param", LongNumericField1);
////            Parameters.AddWithValue("@pShortNumericField3Param", ShortNumericField3);
////            Parameters.AddWithValue("@pShortNumericField4Param", ShortNumericField4);

//            return base.Update();
//        }


//        public override List<IEntity> Search()
//        {

//            string fromClause = "[tblToDoTableName] T ";
//            string whereClause = "";


//            if (Id != 0L)                                                               /*
//            {
//                whereClause += @$"T.Id = @pId ";
//                Parameters.Add(new SqlParameter("@pId", this.Id));
//            }

//            if (ForiegnKey1 != 0)
//            {
//                whereClause += $"and T.ForiegnKey1 like @pForiegnKey1 ";
//                Parameters.AddWithValue("@pForiegnKey1", $"%{this.ForiegnKey1}%");
//            }

//            if (!string.IsNullOrWhiteSpace(Username))
//            {
//                whereClause += $"and T.Username like @pRowName1 ";
//                Parameters.Add(new SqlParameter("@pRowName1", $"%{this.Username}%"));
//            }                                                                           */

////            if (ForiegnKey2 != 0)
////            {
////                whereClause += $"and T.ForiegnKey2 like @pForiegnKey1 ";
////                Parameters.AddWithValue("@pForiegnKey1", $"%{this.ForiegnKey2}%");
////            }
////
////            if (ForiegnKey3 != 0)
////            {
////                whereClause += $"and T.ForiegnKey3 like @pForiegnKey1 ";
////                Parameters.AddWithValue("@pForiegnKey1", $"%{this.ForiegnKey3}%");
////            }
////
////            if (ShortNumericField1 > 0)
////            {
////                whereClause += $"and T.ShortNumericField1 like @pShortNumericField1Param ";
////                Parameters.Add(new SqlParameter("@pShortNumericField1Param", $"%{this.ShortNumericField1}%"));
////            }

//            // TODO: Criteria for other fields.


//            CommandText = @$"
//                select 
//                    T.* 
//                from
//                    {fromClause}
//                where
//                    {whereClause}
//            ";


//            return base.Search();
//        }


//        #endregion
//        #region Other Methods


//        public override void Reset()
//        {
//            Id = 0L;
//            ForiegnKey1 = 0L;
//            RowName1 = string.Empty;
////            ShortNumericField1 = 0;
////            ShortNumericField2 = 0;
////            LongNumericField1 = 0L;
////            ShortNumericField3 = 0;
////            ShortNumericField4 = 0;
//        }

//        public override LibEntity.IEntity Populate(SqlDataReader reader, LibEntity.IEntity? entity = null) //IEntity Populate(SqlDataReader reader, IEntity? entity = null)
//        {

//            CTemplate row = (CTemplate?)entity ?? new CTemplate();

//            row.Id = (long)reader["Id"];
//            row.ForiegnKey1 = (long)reader["ForiegnKey1"];
//            row.RowName1 = (string)reader["Username"];
////            row.ShortNumericField1 = (short)reader["ShortNumericField1"];
////            row.ShortNumericField2 = (short)reader["ShortNumericField2"];
////            row.LongNumericField1 = (long)reader["Duration"];
////            row.ShortNumericField3 = (short)reader["ShortNumericField3"];
////            row.ShortNumericField4 = (short)reader["ShortNumericField4"];

//            return row;
//        }

//        public override void Validate()
//        {
//            string message = "";

//            if (string.IsNullOrWhiteSpace(RowName1))
//            {
//                message += $"{nameof(RowName1)} must be provided\n";
//            }

//            if (Id < 0L)
//            {
//                message += $"{nameof(Id)} must be provided\n";
//            }

//            if (ForiegnKey1 < 0L)
//            {
//                message += $"{nameof(ForiegnKey1)} must be provided\n";
//            }

////            if (ShortNumericField1 > 0)
////            {
////                message += $"{nameof(ShortNumericField1)} must be provided\n";
////            }
////
////            if (ShortNumericField3 < 0)
////            {
////                message += $"{nameof(ShortNumericField3)} must be a value greater than zero\n";
////            }
////
////            if (ShortNumericField4 < 0)
////            {
////                message += $"{nameof(ShortNumericField4)} must be a value greater than zero\n";
////            }
////
////            if (ShortNumericField1 < 0)
////            {
////                message += $"{nameof(ShortNumericField1)} must be a value greater than zero";
////            }

//            throw new CWheelOfDeathException(message);
//        }

//        public override string ToString()
//        {
//            return $"{this.Id}: {this.RowName1}";
//        }

//        #endregion
//    }
}
