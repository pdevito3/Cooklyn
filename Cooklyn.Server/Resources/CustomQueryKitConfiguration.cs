namespace Cooklyn.Server.Resources;

using Domain.Recipes;
using QueryKit.Configuration;

public class CustomQueryKitConfiguration(Action<QueryKitSettings>? configureSettings = null)
    : QueryKitConfiguration(settings =>
    {
        settings.Property<Recipe>(x => x.Rating.Value)
            .HasQueryName("rating");
        settings.Property<Recipe>(x => x.RecipeTags
            .Select(rt => rt.Tag.Name))
            .HasQueryName("tags");
        settings.Property<Recipe>(x => x.Flags
            .Select(f => f.Flag.Value))
            .HasQueryName("flags");
        settings.Property<Recipe>(x => x.Source)
            .HasQueryName("source");
        configureSettings?.Invoke(settings);
    })
{
}