﻿using System;
using System.Collections.Generic;
using Nelibur.ObjectMapper.Bindings;
using Nelibur.ObjectMapper.Core;
using Nelibur.ObjectMapper.Core.DataStructures;
using Nelibur.ObjectMapper.Core.Extensions;
using Nelibur.ObjectMapper.Mappers;
using Nelibur.ObjectMapper.Reflection;

namespace Nelibur.ObjectMapper
{
    public static class TinyMapper
    {
        private static readonly Dictionary<TypePair, Mapper> _mappers = new Dictionary<TypePair, Mapper>();
        private static readonly TargetMapperBuilder _targetMapperBuilder;

        static TinyMapper()
        {
            IDynamicAssembly assembly = DynamicAssemblyBuilder.Get();
            _targetMapperBuilder = new TargetMapperBuilder(assembly);
        }

        public static void Bind<TSource, TTarget>()
        {
            TypePair typePair = TypePair.Create<TSource, TTarget>();

            _mappers[typePair] = _targetMapperBuilder.Build(typePair);
        }

        public static void Bind<TSource, TTarget>(Action<IBindingConfig<TTarget>> config)
        {
            TypePair typePair = TypePair.Create<TSource, TTarget>();

            var bindingConfig = new BindingConfigOf<TTarget>();
            config(bindingConfig);

            _mappers[typePair] = _targetMapperBuilder.Build(typePair, bindingConfig);
        }

        public static TTarget Map<TSource, TTarget>(TSource source, TTarget target = default(TTarget))
        {
            if (source.IsNull())
            {
                throw Error.ArgumentNull("source");
            }

            TypePair typePair = TypePair.Create<TSource, TTarget>();

            Mapper mapper = GetMapper(typePair);
            var result = (TTarget)mapper.Map(source, target);

            return result;
        }

        public static TTarget Map<TTarget>(object source)
        {
            if (source.IsNull())
            {
                throw Error.ArgumentNull("source");
            }

            TypePair typePair = TypePair.Create(source.GetType(), typeof(TTarget));

            Mapper mapper = GetMapper(typePair);
            var result = (TTarget)mapper.Map(source);

            return result;
        }

        private static Mapper GetMapper(TypePair typePair)
        {
            Mapper mapper;
            if (_mappers.TryGetValue(typePair, out mapper) == false)
            {
                mapper = _targetMapperBuilder.Build(typePair);
                _mappers[typePair] = mapper;
            }
            return mapper;
        }
    }
}
