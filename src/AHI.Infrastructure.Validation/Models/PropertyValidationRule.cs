namespace AHI.Infrastructure.Validation.Model
{
    public class PropertyValidationRule
    {
        #region Properties

        public string Key { get; set; }

        public bool IsRequired { get; set; }

        public string Regex { get; set; }

        public int? MaxLength { get; set; }

        public string Description { get; set; }

        #endregion
    }
}