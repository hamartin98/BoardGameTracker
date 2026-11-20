using System.Linq.Expressions;
using System.Reflection;
using BoardGameTracker.Models.Enums;

namespace BoardGameTracker.Models.Filters;

public static class QueryableOrderingExtensions
{
    private static readonly IReadOnlyDictionary<string, MethodInfo> OrderingMethods = new[]
    {
        nameof(Queryable.OrderBy),
        nameof(Queryable.OrderByDescending),
        nameof(Queryable.ThenBy),
        nameof(Queryable.ThenByDescending),
    }.ToDictionary(
        name => name,
        name =>
            typeof(Queryable)
                .GetMethods()
                .Single(method =>
                    method.Name == name
                    && method.IsGenericMethodDefinition
                    && method.GetGenericArguments().Length == 2
                    && method.GetParameters().Length == 2
                )
    );

    public static IOrderedQueryable<EntityType> OrderBy<EntityType>(
        this IQueryable<EntityType> source,
        string property,
        OrderByDirection direction = OrderByDirection.ASC
    )
    {
        string method =
            direction == OrderByDirection.ASC
                ? nameof(Queryable.OrderBy)
                : nameof(Queryable.OrderByDescending);
        return ApplyOrder(source, property, method);
    }

    public static IOrderedQueryable<EntityType> ThenBy<EntityType>(
        this IOrderedQueryable<EntityType> source,
        string property,
        OrderByDirection direction = OrderByDirection.ASC
    )
    {
        string method =
            direction == OrderByDirection.ASC
                ? nameof(Queryable.ThenBy)
                : nameof(Queryable.ThenByDescending);
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
            if (string.IsNullOrWhiteSpace(prop))
            {
                throw new InvalidOrderByException(property);
            }
            // use reflection (not ComponentModel) to mirror LINQ
            PropertyInfo propertyInfo =
                type.GetProperty(
                    prop,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase
                ) ?? throw new InvalidOrderByException(property);
            expression = Expression.Property(expression, propertyInfo);
            type = propertyInfo.PropertyType;
        }
        Type delegateType = typeof(Func<,>).MakeGenericType(typeof(EntityType), type);
        LambdaExpression lambda = Expression.Lambda(delegateType, expression, parameterExpression);

        object result =
            OrderingMethods[methodName]
                .MakeGenericMethod(typeof(EntityType), type)
                .Invoke(null, [source, lambda])
            ?? throw new InvalidOperationException($"Invalid ordering by {property} property.");
        return (IOrderedQueryable<EntityType>)result;
    }
}
