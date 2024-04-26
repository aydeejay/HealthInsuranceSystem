using System;
using System.Linq.Expressions;
using System.Reflection;

using AutoMapper;

using HealthInsuranceSystem.Core.Extensions;

namespace HealthInsuranceSystem.Core.Extensions
{
    public static class MapperExtensions
    {
        public static TTo MapTo<TTo>(this object from, IConfigurationProvider automapperConfiguration)
        {
            return new Mapper(automapperConfiguration).Map<TTo>(from);
        }
        //public static TTo MapTo<TTo>(this object from, IMapper mapper)
        //{
        //    return mapper.Map<TTo>(from);
        //}
        public static IMappingExpression<TSource, TDestination> Ignore<TSource, TDestination, TMember>(
            this IMappingExpression<TSource, TDestination> profile,
            Expression<Func<TDestination, TMember>> member)
        {
            return profile.ForMember(member, options => options.Ignore());
        }

        public static IMappingExpression<TSource, TDestination> UseDefaultFor<TSource, TDestination, TMember>(
            this IMappingExpression<TSource, TDestination> profile,
            Expression<Func<TDestination, TMember>> member)
        {
            return profile.ForMember(member, options =>
            {
                options.AllowNull();
                options.MapFrom(src => default(TMember));
            });
        }

        public static IMappingExpression<TSource, TDestination> MapMember<TSource, TDestination, TMember>(
            this IMappingExpression<TSource, TDestination> profile,
            Expression<Func<TDestination, TMember>> member)
        {
            var memberExpr = member.Body as MemberExpression
                ?? throw new ArgumentException($"Expression '{member}' refers to a method, not a property.");

            var propInfo = memberExpr.Member as PropertyInfo
                ?? throw new ArgumentException($"Expression '{member}' refers to a field, not a property.");

            return profile.ForMember(member, options => options.MapFrom(propInfo.Name));
        }
    }
}
