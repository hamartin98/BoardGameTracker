using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;

namespace BoardGameTracker.Api.Setup;

/// <summary>
/// Documents the ProblemDetails status codes each action can return
/// </summary>
internal sealed class ProblemResponsesConvention : IActionModelConvention
{
    public void Apply(ActionModel action)
    {
        bool isGet = action
            .Attributes.OfType<IActionHttpMethodProvider>()
            .SelectMany(e => e.HttpMethods)
            .All(e => e == HttpMethods.Get);
        bool hasId = action.Selectors.Any(e =>
            e.AttributeRouteModel?.Template?.Contains("{id") == true
        );
        bool takesIfMatch = action.Parameters.Any(e =>
            e.BindingInfo?.BindingSource == BindingSource.Header
            && e.BindingInfo.BinderModelName == "If-Match"
        );

        List<int> statusCodes = [StatusCodes.Status400BadRequest];
        if (hasId || !isGet)
        {
            statusCodes.Add(StatusCodes.Status404NotFound);
        }
        if (takesIfMatch)
        {
            statusCodes.Add(StatusCodes.Status412PreconditionFailed);
        }
        if (!isGet)
        {
            statusCodes.Add(StatusCodes.Status422UnprocessableEntity);
        }
        if (takesIfMatch)
        {
            statusCodes.Add(StatusCodes.Status428PreconditionRequired);
        }
        statusCodes.Add(StatusCodes.Status500InternalServerError);

        foreach (int statusCode in statusCodes)
        {
            action.Filters.Add(
                new ProducesResponseTypeAttribute(typeof(ProblemDetails), statusCode)
            );
        }
    }
}
