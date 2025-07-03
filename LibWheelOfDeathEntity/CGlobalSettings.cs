using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibEntity;
using LibEntity.NetCore.Infrastructure;
using Microsoft.Data.SqlClient;

namespace LibWheelOfDeath
{
    public class CGlobalSettings : CEntity
    {
        public CGlobalSettings() : base("tblGlobalSettings") { }

        public long HofRecordCount { get; set; } = 10;

        public static CGlobalSettings GetSettings()
        {
            var settings = new CGlobalSettings();
            settings.Read(1); // Always ID 1
            return settings;
        }

        public override IEntity Populate(SqlDataReader reader, IEntity? entity = null)
        {
            CGlobalSettings settings = (CGlobalSettings?)entity ?? new CGlobalSettings();

            settings.Id = (long)reader["Id"];
            settings.HofRecordCount = (long)reader["HofRecordCount"];

            return settings;
        }

        public override void Validate()
        {
            CValidator<CGlobalSettings> validator = new(this);

            if (HofRecordCount < 1)
            {
                validator.ManualAddFailure(
                    LibEntity.NetCore.Exceptions.EnumValidationFailure.OutOfRange,
                    $"{nameof(HofRecordCount)} must be at least 1"
                );
            }

            validator.Validate();
        }

        public override int Update()
        {
            CommandText = @"
            update tblGlobalSettings 
            set HofRecordCount = @pCount 
            where Id = 1";

            Parameters.AddWithValue("@pCount", HofRecordCount);
            return base.Update();
        }
    }
}
