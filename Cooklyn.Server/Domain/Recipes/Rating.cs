namespace Cooklyn.Server.Domain.Recipes;

using Ardalis.SmartEnum;
using Exceptions;

public class Rating : ValueObject
{
    private RatingEnum _rating = null!;

    public string Value
    {
        get => _rating.Name;
        private set
        {
            if (!RatingEnum.TryFromName(value, true, out var parsed))
                throw new ValidationException(nameof(Rating), $"Invalid rating: {value}");

            _rating = parsed;
        }
    }

    public int SortOrder => _rating.Value;

    public Rating(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationException(nameof(Rating), "Rating cannot be null or empty.");

        Value = value;
    }

    public bool IsRated() => _rating != RatingEnum.NotRated;

    public static Rating Of(string value) => new(value);
    public static implicit operator string(Rating value) => value.Value;
    public static List<string> ListNames() => RatingEnum.List.Select(x => x.Name).ToList();

    public static Rating NotRated() => new(RatingEnum.NotRated.Name);
    public static Rating LovedIt() => new(RatingEnum.LovedIt.Name);
    public static Rating LikedIt() => new(RatingEnum.LikedIt.Name);
    public static Rating ItWasOk() => new(RatingEnum.ItWasOk.Name);
    public static Rating NotGreat() => new(RatingEnum.NotGreat.Name);
    public static Rating HatedIt() => new(RatingEnum.HatedIt.Name);

    protected Rating() { } // EF Core

    private class RatingEnum(string name, int value) : SmartEnum<RatingEnum>(name, value)
    {
        public static readonly RatingEnum NotRated = new("Not Rated", 0);
        public static readonly RatingEnum LovedIt = new("Loved It", 5);
        public static readonly RatingEnum LikedIt = new("Liked It", 4);
        public static readonly RatingEnum ItWasOk = new("It Was Ok", 3);
        public static readonly RatingEnum NotGreat = new("Not Great", 2);
        public static readonly RatingEnum HatedIt = new("Hated It", 1);
    }
}
