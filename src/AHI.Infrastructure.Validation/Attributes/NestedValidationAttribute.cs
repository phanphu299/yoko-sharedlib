using System;

namespace AHI.Infrastructure.Validation.CustomAttribute
{
    /// <summary>
    /// Mark 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NestedValidationAttribute : Attribute
    {
    }
}