namespace Cooklyn.Server.Domain.ShoppingLists;

using Ardalis.SmartEnum;
using Exceptions;

public partial class ShoppingListStatus : ValueObject
{
    private ShoppingListStatusEnum _status = null!;

    public string Value
    {
        get => _status.Name;
        private set
        {
            if (!ShoppingListStatusEnum.TryFromName(value, true, out var parsed))
                throw new ValidationException(nameof(ShoppingListStatus), $"Invalid shopping list status: {value}");

            _status = parsed;
        }
    }

    public ShoppingListStatus(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationException(nameof(ShoppingListStatus), "Shopping list status cannot be null or empty.");

        Value = value;
    }

    public static ShoppingListStatus Of(string value) => new(value);
    public static implicit operator string(ShoppingListStatus value) => value.Value;

    public static ShoppingListStatus Active() => new(ShoppingListStatusEnum.Active.Name);
    public static ShoppingListStatus Completed() => new(ShoppingListStatusEnum.Completed.Name);

    protected ShoppingListStatus() { } // EF Core

    private abstract class ShoppingListStatusEnum(string name, int value) : SmartEnum<ShoppingListStatusEnum>(name, value)
    {
        public static readonly ShoppingListStatusEnum Active = new ActiveType();
        public static readonly ShoppingListStatusEnum Completed = new CompletedType();

        private class ActiveType() : ShoppingListStatusEnum("Active", 0);
        private class CompletedType() : ShoppingListStatusEnum("Completed", 1);
    }
}
