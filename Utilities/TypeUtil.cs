using System;
using System.Collections.Generic;
using System.Linq;

namespace DevConsole.Utilities
{
    internal static class TypeUtil
    {
        /// <summary>
        /// Gets all sub types which aren't abstract from the given type.
        /// </summary>
        /// <param name="type">The parent or ancestor type.</param>
        /// <returns>List of resulting types.</returns>
        internal static List<Type> GetNonAbstractSubTypes(Type type)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(assemblyType => !assemblyType.IsAbstract && assemblyType.IsSubclassOf(type))
                .ToList();
        }
    }
}