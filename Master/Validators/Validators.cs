// FutureDateAttribute.cs
using System.ComponentModel.DataAnnotations;

public class FutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        return value is DateTime date && date > DateTime.Now;
    }
}

// RequiredIfAttribute.cs
public class RequiredIfAttribute : ValidationAttribute
{
    private string PropertyName { get; set; }
    private object DesiredValue { get; set; }

    public RequiredIfAttribute(string propertyName, object desiredValue)
    {
        PropertyName = propertyName;
        DesiredValue = desiredValue;
    }

    protected override ValidationResult IsValid(object value, ValidationContext context)
    {
        var instance = context.ObjectInstance;
        var type = instance.GetType();
        var propertyValue = type.GetProperty(PropertyName)?.GetValue(instance, null);

        if (propertyValue?.ToString() == DesiredValue.ToString() && value == null)
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}

// ImageFileAttribute.cs
public class ImageFileAttribute : ValidationAttribute
{
    private readonly string[] _extensions = { ".jpg", ".jpeg", ".png" };
    private readonly long _maxSize = 5 * 1024 * 1024; // 5MB

    protected override ValidationResult IsValid(object value, ValidationContext context)
    {
        if (value is IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!_extensions.Contains(extension))
                return new ValidationResult("امتداد الملف غير مسموح به");

            if (file.Length > _maxSize)
                return new ValidationResult($"الحجم الأقصى المسموح: {_maxSize / 1024 / 1024}MB");
        }

        return ValidationResult.Success;
    }
}