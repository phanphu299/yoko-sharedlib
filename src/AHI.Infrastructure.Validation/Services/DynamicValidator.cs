using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using AHI.Infrastructure.Validation.Abstraction;
using AHI.Infrastructure.Validation.CustomAttribute;
using AHI.Infrastructure.Validation.Model;
using AHI.Infrastructure.Exception;

namespace AHI.Infrastructure.Validation.Services
{
    public class DynamicValidator : IDynamicValidator
    {

        private readonly ITypeValidator _validator;

        private static readonly ConcurrentDictionary<Type, bool> _primitiveTypes = new ConcurrentDictionary<Type, bool>();


        public DynamicValidator(ITypeValidator validator)
        {
            _validator = validator;
        }


        public virtual async Task ValidateAsync(object requestObject, CancellationToken cancellationToken = default)
        {
            var validationFailures = await GetValidationFailuresAsync(requestObject, requestObject, cancellationToken);
            if (validationFailures?.Any() == true)
            {
                throw new EntityValidationException(failures: validationFailures.ToList());
            }
        }

        /// <summary>
        /// Get validation failures from an object.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="sourceObject"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected virtual async Task<IEnumerable<ValidationFailure>> GetValidationFailuresAsync(object instance, object sourceObject, CancellationToken cancellationToken = default)
        {
            if (instance == null)
                return Array.Empty<ValidationFailure>();

            var validationFailures = new List<ValidationFailure>();

            IList<PropertyValidationContext> validationContexts = new List<PropertyValidationContext>();

            // Get instance properties.
            GetInstanceValidationContexts(instance, null, sourceObject, validationContexts);

            foreach (var validationContext in validationContexts)
            {
                // Build a list of validation failures
                var localValidationFailures = new List<ValidationFailure>();

                var attributeValidationFailures =
                    await _validator.ValidateAsync(validationContext, cancellationToken);

                if (attributeValidationFailures?.Any() == true)
                    localValidationFailures.AddRange(attributeValidationFailures);

                if (localValidationFailures.Any())
                    validationFailures.AddRange(localValidationFailures);
            }

            return validationFailures;
        }

        /// <summary>
        /// Get validation context from an instance.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="memberInfo">Null if the instance is the root object. If the instance is a property of another object, the PropertyInfo will consist of property meta data of the instance.</param>
        /// <param name="sourceObject"></param>
        /// <param name="contexts"></param>
        private void GetInstanceValidationContexts(object instance, MemberInfo memberInfo, object sourceObject, IList<PropertyValidationContext> contexts)
        {
            if (instance == null && memberInfo == null)
                return;

            // Get instance type.
            PropertyInfo propertyInfo = memberInfo as PropertyInfo;

            var instanceType = propertyInfo?.PropertyType ?? instance?.GetType();
            if (instanceType == null)
                return;

            // If the property is marked with attribute.
            // The context must be added to the list and we do not dig deeper into it anymore.
            Attribute[] attributes;

            Func<Attribute, bool> attributeFilter =
                x => x is DynamicValidationAttribute || x is NestedValidationAttribute;

            if (propertyInfo != null)
            {
                attributes = propertyInfo.GetCustomAttributes()
                    .Where(attributeFilter).ToArray();
            }
            else
            {
                attributes = instanceType.GetCustomAttributes()
                    .Where(attributeFilter)
                    .ToArray();
            }


            // If the source object is primitive value. Its child properties will be ignored.
            if (IsPrimitive(instanceType))
            {
                var dynamicValidationAttributes = attributes.Where(x => x is DynamicValidationAttribute)
                    .OfType<DynamicValidationAttribute>()
                    .ToArray();
                if (dynamicValidationAttributes.Any())
                {
                    // Build the validation context.
                    var context = new PropertyValidationContext(memberInfo?.Name ?? string.Empty, instance, instanceType);
                    context.AddAttributes(dynamicValidationAttributes);
                    contexts.Add(context);
                }

                return;
            }

            // Instance is a dictionary.
            if (IsInstanceOfDictionary(instanceType))
            {
                // For now, dictionary is not supported for validation.
                return;
            }

            // Instance is a collection.
            if (typeof(IEnumerable).IsAssignableFrom(instanceType))
            {
                // For now, collection is not supported for validation.
                return;
            }

            // Instance is an object. If it is marked with NestedValidationAttribute.
            var nestedValidationAttribute = attributes.FirstOrDefault(x => x is NestedValidationAttribute);
            if (nestedValidationAttribute == null && instance != sourceObject)
                return;

            // Get child object properties.
            var childObjectProperties = instanceType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var childObjectProperty in childObjectProperties)
            {
                var childPropertyValue = childObjectProperty.GetValue(instance);
                GetInstanceValidationContexts(childPropertyValue, childObjectProperty, instance, contexts);
            }
        }

        /// <summary>
        /// To check whether a type is language built-in primitive type or user defined one.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsPrimitive(Type type)
        {
            return _primitiveTypes.GetOrAdd(type, t =>
                type.IsPrimitive ||
                type.IsEnum ||
                type == typeof(string) ||
                type == typeof(decimal) ||
                type == typeof(DateTime) ||
                type == typeof(DateTimeOffset) ||
                type == typeof(TimeSpan) ||
                type == typeof(Guid) ||
                IsNullableSimpleType(type));

            static bool IsNullableSimpleType(Type t)
            {
                var underlyingType = Nullable.GetUnderlyingType(t);
                return underlyingType != null && IsPrimitive(underlyingType);
            }
        }

        /// <summary>
        /// Check whether instance is a dictionary or not.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool IsInstanceOfDictionary(Type type)
        {
            if (type == typeof(IDictionary))
                return true;

            if (type.IsGenericType)
            {
                var genericTypeDefinition = type.GetGenericTypeDefinition();
                return typeof(IDictionary<,>).IsAssignableFrom(genericTypeDefinition);
            }

            return false;
        }

    }
}
