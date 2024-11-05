using System.Runtime.CompilerServices;

namespace Northwind.Services.EntityFramework;
public static class ConfigureAwaitWrapper
{
    public static ConfiguredTaskAwaitable SetConfigureAwait(this Task task)
    {
        ExceptionHelper.ThrowArgumentNullException(task);

        return task.ConfigureAwait(false);
    }

    public static ConfiguredTaskAwaitable<T> SetConfigureAwait<T>(this Task<T> task)
    {
        ExceptionHelper.ThrowArgumentNullException(task);

        return task.ConfigureAwait(false);
    }

    public static ConfiguredValueTaskAwaitable<T> SetConfigureAwait<T>(this ValueTask<T> task)
    {
        return task.ConfigureAwait(false);
    }
}
