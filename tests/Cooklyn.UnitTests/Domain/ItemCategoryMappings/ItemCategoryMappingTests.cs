namespace Cooklyn.UnitTests.Domain.ItemCategoryMappings;

using Cooklyn.Server.Domain.ItemCategoryMappings;
using Cooklyn.Server.Domain.ItemCategoryMappings.Models;
using Cooklyn.Server.Exceptions;
using Shouldly;

public class ItemCategoryMappingTests
{
    [Fact]
    public void can_create_valid_mapping()
    {
        // Arrange
        var forCreation = new ItemCategoryMappingForCreation
        {
            NormalizedName = "chicken breast",
            StoreSectionId = "ssec_456",
            Source = "Seed"
        };

        // Act
        var mapping = ItemCategoryMapping.Create(forCreation);

        // Assert
        mapping.NormalizedName.ShouldBe("chicken breast");
        mapping.StoreSectionId.ShouldBe("ssec_456");
        mapping.Source.Value.ShouldBe("Seed");
    }

    [Fact]
    public void missing_normalized_name_throws_validation_exception()
    {
        var forCreation = new ItemCategoryMappingForCreation
        {
            NormalizedName = "",
            StoreSectionId = "ssec_456",
            Source = "Seed"
        };

        Should.Throw<ValidationException>(() => ItemCategoryMapping.Create(forCreation));
    }

    [Fact]
    public void missing_store_section_throws_validation_exception()
    {
        var forCreation = new ItemCategoryMappingForCreation
        {
            NormalizedName = "chicken",
            StoreSectionId = "",
            Source = "Seed"
        };

        Should.Throw<ValidationException>(() => ItemCategoryMapping.Create(forCreation));
    }

    [Fact]
    public void update_section_changes_section_and_source()
    {
        // Arrange
        var mapping = ItemCategoryMapping.Create(new ItemCategoryMappingForCreation
        {
            NormalizedName = "egg",
            StoreSectionId = "ssec_original",
            Source = "Seed"
        });

        // Act
        mapping.UpdateSection("ssec_new", "User");

        // Assert
        mapping.StoreSectionId.ShouldBe("ssec_new");
        mapping.Source.Value.ShouldBe("User");
    }

    [Fact]
    public void update_section_with_empty_id_throws()
    {
        var mapping = ItemCategoryMapping.Create(new ItemCategoryMappingForCreation
        {
            NormalizedName = "egg",
            StoreSectionId = "ssec_original",
            Source = "Seed"
        });

        Should.Throw<ValidationException>(() => mapping.UpdateSection("", "User"));
    }
}
