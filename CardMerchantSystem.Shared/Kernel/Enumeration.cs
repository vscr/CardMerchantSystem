using System.Reflection;

namespace CardMerchantSystem.Shared.Kernel;

/// <summary>
/// Smart Enumeration pattern.
/// Normal enum yerine bu kullanılır çünkü:
/// - Behavior eklenebilir
/// - Validation yapılabilir
/// - Display name tutulabilir
/// </summary>
public abstract class Enumeration : IComparable
{
    public int Id { get; }
    public string Name { get; }
    public string DisplayName { get; }

    protected Enumeration(int id, string name, string? displayName = null)
    {
        Id = id;
        Name = name;
        DisplayName = displayName ?? name;
    }

    public override string ToString() => Name;

    public static IEnumerable<T> GetAll<T>() where T : Enumeration
    {
        return typeof(T)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>();
    }

    public static T? FromId<T>(int id) where T : Enumeration
    {
        return GetAll<T>().FirstOrDefault(x => x.Id == id);
    }

    public static T? FromName<T>(string name) where T : Enumeration
    {
        return GetAll<T>().FirstOrDefault(x =>
            x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Enumeration other)
            return false;

        var typeMatches = GetType() == obj.GetType();
        var valueMatches = Id.Equals(other.Id);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode() => Id.GetHashCode();

    public int CompareTo(object? other) => Id.CompareTo(((Enumeration)other!).Id);

    public static bool operator ==(Enumeration? left, Enumeration? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(Enumeration? left, Enumeration? right)
    {
        return !(left == right);
    }
}