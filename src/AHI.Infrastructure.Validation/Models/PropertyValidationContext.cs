using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AHI.Infrastructure.Validation.CustomAttribute;

namespace AHI.Infrastructure.Validation.Model
{
    public class PropertyValidationContext
    {
        #region Properties

        private readonly LinkedList<DynamicValidationAttribute> _attributes;

        #endregion

        #region Accessors

        public string PropertyName { get; }

        public object Value { get; }

        public Type ValueType { get; }

        #endregion

        #region Constructor

        public PropertyValidationContext(string name, object value, Type valueType)
        {
            PropertyName = name;
            Value = value;
            _attributes = new LinkedList<DynamicValidationAttribute>();
            ValueType = valueType;
        }

        public PropertyValidationContext(string name,
            object value,
            Type valueType,
            IEnumerable<DynamicValidationAttribute> attributes) : this(name, value, valueType)
        {
            if (attributes != null && attributes.Count() > 0)
            {
                foreach (var attribute in attributes)
                    _attributes.AddLast(attribute);
            }
        }

        #endregion

        #region Methods

        public void AddAttribute(DynamicValidationAttribute attribute)
        {
            _attributes.AddLast(attribute);
        }

        public void AddAttributes(IEnumerable<DynamicValidationAttribute> attributes)
        {
            foreach (var attribute in attributes)
                _attributes.AddLast(attribute);
        }

        public DynamicValidationAttribute[] GetAttributes()
        {
            return _attributes.ToArray();
        }

        #endregion
    }
}