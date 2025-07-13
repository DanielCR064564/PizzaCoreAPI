using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaCoreAPI.Models
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _dependentProperty;
        private readonly object _targetValue;

        public RequiredIfAttribute(string dependentProperty, object targetValue)
        {
            _dependentProperty = dependentProperty;
            _targetValue = targetValue;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var field = validationContext.ObjectType.GetProperty(_dependentProperty);
            if (field != null)
            {
                var dependentValue = field.GetValue(validationContext.ObjectInstance, null);
                var contextValue = validationContext.ObjectInstance?.GetType()?.GetProperty(_dependentProperty)?.GetValue(validationContext.ObjectInstance, null);
                if (dependentValue != null && contextValue != null && dependentValue.Equals(_targetValue))
                {
                    if (value == null || string.IsNullOrEmpty(value.ToString()))
                    {
                        return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} es requerido cuando {_dependentProperty} es {_targetValue}");
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
}
