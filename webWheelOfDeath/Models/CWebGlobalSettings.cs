using LibWheelOfDeath;
using webWheelOfDeath.Models.Infrastructure;

namespace webWheelOfDeath.Models
{
    public class CWebGlobalSettings : CEntityModel<CGlobalSettings>
    {
        private long? _hofRecordCount;

        public long HofRecordCount
        {
            get => _hofRecordCount ?? 10;
            set => _hofRecordCount = value;
        }

        public CWebGlobalSettings() : base()
        {
            // Load the single settings record
            if (!IsNew)
            {
                Id = 1; // Always ID 1
                Refresh();
            }
        }

        protected override void MapFromEntity(CGlobalSettings entity)
        {
            HofRecordCount = entity.HofRecordCount;
        }

        protected override void MapToEntity(CGlobalSettings entity)
        {
            entity.HofRecordCount = HofRecordCount;
        }

        protected override void ValidateRequiredFields(bool isUpdate)
        {
            if (HofRecordCount < 1)
                throw new InvalidOperationException("Hall of Fame count must be at least 1");
        }

        public static CWebGlobalSettings Load()
        {
            var settings = new CWebGlobalSettings();
            settings.Id = 1;
            settings.Refresh();
            return settings;
        }
    }
}