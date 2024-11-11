using System.ComponentModel.DataAnnotations;

namespace ScientificEdition.Mvc.Filters.Validation
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] extensions;
        private readonly string[] mimeTypes;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            this.extensions = extensions;
            mimeTypes = [];
        }

        public AllowedExtensionsAttribute(string[] extensions, string[] mimeTypes)
        {
            this.extensions = extensions;
            this.mimeTypes = mimeTypes;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!extensions.Contains(extension))
                    return new ValidationResult($"Дозволені типи файлів: {string.Join(", ", extensions)}.");

                if (mimeTypes.Any() && !mimeTypes.Contains(file.ContentType))
                    return new ValidationResult($"Дозволені типи файлів: {string.Join(", ", extensions)}.");
            }

            return ValidationResult.Success!;
        }
    }
}
