namespace Northwind.Orders.WebApi;

internal static class ExceptionHelper
{
    public static void ThrowArgumentNullException<T>(T? obj)
        where T : class
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj), $"Type {typeof(T).Name} is null!");
        }
    }
}
