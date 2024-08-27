using System;

namespace AHI.Infrastructure.Validation.CustomAttribute
{
    /// <summary>
    /// Mark a primitive attribute that can be validated.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DynamicValidationAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// Key for doing validation.
        /// </summary>
        public string Key { get; set; }

        #endregion

        #region Constructor

        public DynamicValidationAttribute(string key)
        {
            Key = key;
        }

        #endregion
    }
}