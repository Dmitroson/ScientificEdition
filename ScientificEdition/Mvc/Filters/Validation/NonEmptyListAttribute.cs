using System.ComponentModel.DataAnnotations;

namespace ScientificEdition.Mvc.Filters.Validation
{
    public class NonEmptyListAttribute(uint minLength = 1) : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not IEnumerable<object> list || !list.Any())
            {
                var emptyListErrorMessage = $"Список не може бути порожнім.";
                return new ValidationResult(emptyListErrorMessage);
            }

            if (list.Count() >= minLength)
                return ValidationResult.Success!;

            var minLengthErrorMessage = $"Список повинен містити щонайменше {minLength} елемент(и).";
            return new ValidationResult(minLengthErrorMessage);
        }
    }
}
