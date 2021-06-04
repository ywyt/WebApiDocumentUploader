using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiDocumentUploader.Helper
{
    public class AutoMapHelper
    {
        public static T Map<S, T>(S s)
        {
            return CreateMapper<S, T>().Map<S, T>(s);
        }

        public static void Map<S, T>(S s, T t)
        {
            CreateMapper<S, T>().Map(s, t);
        }

        private static IMapper CreateMapper<S, T>()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<S, T>();
                IgnoreSourceWhenNull(cfg);
            });
            IMapper iMapper = config.CreateMapper();

            return iMapper;
        }

        /// <summary>
        /// Condition when mapping from Nullable<T> to T
        /// </summary>
        /// <remarks>
        /// https://github.com/AutoMapper/AutoMapper/issues/2999
        /// </remarks>
        /// <param name="cfg"></param>
        public static void IgnoreSourceWhenNull(IMapperConfigurationExpression cfg)
        {
            cfg.ForAllPropertyMaps(pm =>
            {
                if (pm.SourceType != pm.DestinationType)
                    return false;
                if (pm.SourceMember != null && (pm.DestinationMember.Name != pm.SourceMember.Name))
                    return false;
                if (pm.SourceType == null)
                    return false;
                var isNullable = (pm.SourceType.IsGenericType && (pm.SourceType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                                || pm.SourceType == typeof(string);// string不是Nullable类型，但也可能为null
                return isNullable || pm.SourceType.IsValueType || pm.SourceType.IsPrimitive;
            }, (pm, c) =>
            {
                c.MapFrom<Resolver, object>(pm.SourceMember.Name);
            });
        }
    }

    class Resolver : IMemberValueResolver<object, object, object, object>
    {
        public object Resolve(object source, object destination, object sourceMember, object destinationMember, ResolutionContext context)
        {
            return sourceMember ?? destinationMember;
        }
    }
}
