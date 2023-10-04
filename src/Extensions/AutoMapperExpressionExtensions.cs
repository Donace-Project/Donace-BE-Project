using AutoMapper;
using System.Linq.Expressions;

namespace Donace_BE_Project.Extensions;

public static class AutoMapperExpressionExtensions
{
    public static IMappingExpression<TDestination, TMember> Ignore<TDestination, TMember, TResult>(this IMappingExpression<TDestination, TMember> mappingExpression, Expression<Func<TMember, TResult>> destinationMember)
    {
        return mappingExpression.ForMember(destinationMember, opts => opts.Ignore());
    }
}
