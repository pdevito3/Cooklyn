namespace Cooklyn.Server.Domain.BlobStorageKeys;

using FluentValidation;
using Serilog.Core;
using Serilog.Events;
using System.Text.RegularExpressions;

/// <summary>
/// Value object representing a blob storage key (file path/identifier).
/// Keys must end with a valid file extension.
/// </summary>
public sealed partial class BlobStorageKey : ValueObject
{
    public string? Value { get; private set; }

    public BlobStorageKey(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Value = null;
            return;
        }
        new BlobStorageKeyValidator().ValidateAndThrow(value);
        Value = value;
    }

    public static BlobStorageKey Of(string? value) => new(value);
    public static BlobStorageKey Empty() => new(null);
    public static implicit operator string?(BlobStorageKey? value) => value?.Value;

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    private BlobStorageKey() { } // EF Core

    private sealed partial class BlobStorageKeyValidator : AbstractValidator<string>
    {
        public BlobStorageKeyValidator()
        {
            RuleFor(x => x)
                .Must(HaveFileExtension)
                .WithMessage("The BlobStorage key must end with a valid file extension (e.g., .jpg, .png, .pdf).");
        }

        private static bool HaveFileExtension(string value)
        {
            return FileExtensionRegex().IsMatch(value);
        }

        [GeneratedRegex(@"\.\w{2,5}$")]
        private static partial Regex FileExtensionRegex();
    }
}

/// <summary>
/// Serilog destructuring policy that masks BlobStorageKey values in logs.
/// </summary>
public sealed class BlobStorageKeyDestructuringPolicy : IDestructuringPolicy
{
    public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue? result)
    {
        if (value is BlobStorageKey blobStorageKey)
        {
            result = new ScalarValue(blobStorageKey.IsEmpty ? "[empty]" : "[MASKED]");
            return true;
        }
        result = null;
        return false;
    }
}
