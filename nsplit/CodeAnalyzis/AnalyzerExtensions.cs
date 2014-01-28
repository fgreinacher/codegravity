﻿#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ClrTest.Reflection;
using nsplit.CodeAnalyzis.DataStructures.DependencyGraph;

#endregion

namespace nsplit.CodeAnalyzis
{
    public static class AnalyzerExtensions
    {
        public static IEnumerable<Type> Types(this Assembly assembly)
        {
            return
                assembly
                    .GetTypes()
                    .Where(t => !t.IsCompilerGenerated());
        }

        public static IEnumerable<Dependency> Dependecies(this Type type)
        {
            Console.WriteLine("START: [{0}] ...", type);
            IEnumerable<Dependency> result =
                type.Implements()
                    .Concat(type.Uses())
                    .Concat(type.Calls());
            Console.WriteLine("END: [{0}] ...", type);
            return result;
        }

        public static IEnumerable<Dependency> Calls(this Type type)
        {
            Console.WriteLine("Calls of type [{0}] ...", type);
            return
                type
                    .Methods()
                    .SelectMany(method => method.MethodCalls())
                    .Select(methodCall => new MethodCall(type, methodCall.ReflectedType, methodCall))
                    .Where(call => call.Target != type && !call.Target.IsNestedPrivate);
        }

        public static IEnumerable<Dependency> Uses(this Type type)
        {
            Console.WriteLine("Uses of type [{0}] ...", type);
            IEnumerable<Type> fieldUses =
                type
                    .Fields()
                    .Select(field => field.FieldType)
                    .SelectMany(x => x.Unroll());

            IEnumerable<Type> methodUses =
                type
                    .Methods()
                    .SelectMany(method => method.UsedTypes());

            return
                fieldUses
                    .Concat(methodUses)
                    .Select(to => new Uses(type, to));
        }

        private static IEnumerable<MethodBase> MethodCalls(this MethodInfo methodInfo)
        {
            return
                MethodBodyInfo
                    .Create(methodInfo)
                    .Calls();
        }

        private static IEnumerable<Dependency> Implements(this Type type)
        {
            Console.WriteLine("Implements of type [{0}] ...", type);
            return
                Once(type.BaseType)
                    .Concat(type.GetInterfaces())
                    .Select(baseType => new Implements(type, baseType));
        }

        private static IEnumerable<T> Once<T>(T element) where T : class
        {
            return
                element == null
                    ? Enumerable.Empty<T>()
                    : Enumerable.Repeat(element, 1);
        }

        private static IEnumerable<MethodInfo> Methods(this Type type)
        {
            const BindingFlags flags =
                BindingFlags.DeclaredOnly |
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.Public |
                BindingFlags.NonPublic;

            return type.GetMethods(flags);
        }

        private static IEnumerable<FieldInfo> Fields(this Type type)
        {
            const BindingFlags flags =
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.Public |
                BindingFlags.NonPublic;

            return type.GetFields(flags);
        }

        private static IEnumerable<Type> Unroll(this Type type)
        {
            return type.IsConstructedGenericType
                ? Once(type.GetGenericTypeDefinition()).Concat(type.GenericTypeArguments)
                : Once(type);
        }

        private static IEnumerable<Type> UsedTypes(this MethodInfo method)
        {
            IEnumerable<Type> paramTypes = method
                .GetParameters()
                .SelectMany(param => param.ParameterType.Unroll());

            Type[] genericArguments = method
                .GetGenericArguments();

            return 
                Once(method.ReturnType)
                    .SelectMany(returnType => returnType.Unroll())
                    .Concat(paramTypes)
                    .Concat(genericArguments);
        }

        private static IEnumerable<MethodBase> Calls(this MethodBodyInfo methodBody)
        {
            return methodBody
                .Instructions
                .Select(instr => instr as InlineMethodInstruction)
                .Where(instr => instr != null)
                .Select(instr => instr.Method)
                .Where(call => !call.IsPrivate);
        }

        private static bool IsCompilerGenerated(this Type type)
        {
            return type.GetCustomAttribute<CompilerGeneratedAttribute>() != null;
        }
    }
}