using System.Data.Common;

namespace Northwind.Orders.WebApi.Services;

public class SqlScriptException : DbException
{
    public SqlScriptException()
    {
    }

    public SqlScriptException(string message)
        : base(message)
    {
    }

    public SqlScriptException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
