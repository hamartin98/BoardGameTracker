using System.Linq.Expressions;
using System.Reflection;
using BoardGameTracker.Models.Enums;

namespace BoardGameTracker.Models.Filters;

public static class IOrderedQueryable
{
    public static IOrderedQueryable<EntityType> OrderBy<EntityType>(
        this IQueryable<EntityType> source,
        string property,
        OrderByDirection direction = OrderByDirection.ASC
    )
    {
        string method = direction == OrderByDirection.ASC ? "OrderBy" : "OrderByDescending";
        return ApplyOrder(source, property, method);
    }

    public static IOrderedQueryable<EntityType> ThenBy<EntityType>(
        this IOrderedQueryable<EntityType> source,
        string property,
        OrderByDirection direction = OrderByDirection.ASC
    )
    {
        string method = direction == OrderByDirection.ASC ? "ThenBy" : "ThenByDescending";
        return ApplyOrder(source, property, method);
    }

    static IOrderedQueryable<EntityType> ApplyOrder<EntityType>(
        IQueryable<EntityType> source,
        string property,
        string methodName
    )
    {
        string[] props = property.Split('.');
        Type type = typeof(EntityType);
        ParameterExpression parameterExpression = Expression.Parameter(type, "x");
        Expression expression = parameterExpression;
        foreach (string prop in props)
        {
            // use reflection (not ComponentModel) to mirror LINQ
            string propertyName = GetPropertyName(prop);
            PropertyInfo propertyInfo =
                type.GetProperty(propertyName) ?? throw new ArgumentException(property);
            expression = Expression.Property(expression, propertyInfo);
            type = propertyInfo.PropertyType;
        }
        Type delegateType = typeof(Func<,>).MakeGenericType(typeof(EntityType), type);
        LambdaExpression lambda = Expression.Lambda(delegateType, expression, parameterExpression);

        object result =
            typeof(Queryable)
                .GetMethods()
                .Single(method =>
                    method.Name == methodName
                    && method.IsGenericMethodDefinition
                    && method.GetGenericArguments().Length == 2
                    && method.GetParameters().Length == 2
                )
                .MakeGenericMethod(typeof(EntityType), type)
                .Invoke(null, [source, lambda])
            ?? throw new InvalidOperationException($"Invalid ordering by {property} property.");
        return (IOrderedQueryable<EntityType>)result;
    }

    private static string GetPropertyName(string propertyName)
    {
        return string.Concat(propertyName[0].ToString().ToUpper(), propertyName.AsSpan(1));
    }
}
