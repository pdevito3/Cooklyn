namespace Cooklyn.UnitTests.Domain.BlobStorageKeys;

using FluentValidation;
using Cooklyn.Server.Domain.BlobStorageKeys;
using Shouldly;

public class BlobStorageKeyTests
{
    [Fact]
    public void can_create_valid_blob_storage_key()
    {
        // Arrange & Act
        var key = BlobStorageKey.Of("recipes/images/recipe-123.jpg");

        // Assert
        key.Value.ShouldBe("recipes/images/recipe-123.jpg");
    }

    [Fact]
    public void can_create_with_various_valid_file_extensions()
    {
        // Arrange & Act & Assert
        BlobStorageKey.Of("file.jpg").Value.ShouldBe("file.jpg");
        BlobStorageKey.Of("file.jpeg").Value.ShouldBe("file.jpeg");
        BlobStorageKey.Of("file.png").Value.ShouldBe("file.png");
        BlobStorageKey.Of("file.gif").Value.ShouldBe("file.gif");
        BlobStorageKey.Of("file.webp").Value.ShouldBe("file.webp");
        BlobStorageKey.Of("file.pdf").Value.ShouldBe("file.pdf");
        BlobStorageKey.Of("file.txt").Value.ShouldBe("file.txt");
    }

    [Fact]
    public void can_create_with_nested_paths()
    {
        // Arrange & Act
        var key = BlobStorageKey.Of("tenant-123/recipes/images/2024/01/recipe-abc.png");

        // Assert
        key.Value.ShouldBe("tenant-123/recipes/images/2024/01/recipe-abc.png");
    }

    [Fact]
    public void null_or_whitespace_returns_null_value()
    {
        // Arrange & Act
        var nullKey = BlobStorageKey.Of(null);
        var emptyKey = BlobStorageKey.Of("");
        var whitespaceKey = BlobStorageKey.Of("   ");

        // Assert
        nullKey.Value.ShouldBeNull();
        emptyKey.Value.ShouldBeNull();
        whitespaceKey.Value.ShouldBeNull();
    }

    [Fact]
    public void is_empty_returns_true_for_null_value()
    {
        // Arrange & Act
        var key = BlobStorageKey.Empty();

        // Assert
        key.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void is_empty_returns_false_for_valid_value()
    {
        // Arrange & Act
        var key = BlobStorageKey.Of("file.jpg");

        // Assert
        key.IsEmpty.ShouldBeFalse();
    }

    [Fact]
    public void invalid_key_without_extension_throws_validation_exception()
    {
        // Arrange & Act & Assert
        Should.Throw<ValidationException>(() => BlobStorageKey.Of("no-extension"));
        Should.Throw<ValidationException>(() => BlobStorageKey.Of("path/to/file"));
        Should.Throw<ValidationException>(() => BlobStorageKey.Of("file."));
    }

    [Fact]
    public void invalid_key_with_too_short_extension_throws_validation_exception()
    {
        // Extension must be at least 2 characters
        Should.Throw<ValidationException>(() => BlobStorageKey.Of("file.a"));
    }

    [Fact]
    public void invalid_key_with_too_long_extension_throws_validation_exception()
    {
        // Extension must be at most 5 characters
        Should.Throw<ValidationException>(() => BlobStorageKey.Of("file.toolongext"));
    }

    [Fact]
    public void key_can_be_implicitly_converted_to_string()
    {
        // Arrange
        var key = BlobStorageKey.Of("recipes/image.jpg");

        // Act
        string? keyString = key;

        // Assert
        keyString.ShouldBe("recipes/image.jpg");
    }

    [Fact]
    public void key_equality_works_correctly()
    {
        // Arrange
        var key1 = BlobStorageKey.Of("recipes/image.jpg");
        var key2 = BlobStorageKey.Of("recipes/image.jpg");
        var key3 = BlobStorageKey.Of("recipes/other.jpg");

        // Assert
        key1.Equals(key2).ShouldBeTrue();
        key1.Equals(key3).ShouldBeFalse();
    }

    [Fact]
    public void empty_keys_are_equal()
    {
        // Arrange
        var key1 = BlobStorageKey.Empty();
        var key2 = BlobStorageKey.Of(null);

        // Assert
        key1.Equals(key2).ShouldBeTrue();
    }
}
