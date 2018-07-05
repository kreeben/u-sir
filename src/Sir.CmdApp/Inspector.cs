using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sir.CmdApp
{
    public static class Inspector
    {
        public static MethodInfo Resolve(this IDictionary<string, MethodInfo> methods, 
            string[] input)
        {
            var command = input[0].ToLowerInvariant();
            MethodInfo method;
            if (!methods.TryGetValue(command, out method))
            {
                return null;
            }
            else
            {
                return method;
            }
        }

        public static IDictionary<string, MethodInfo> GetMethods(Type type)
        {
            var result = new Dictionary<string, MethodInfo>();

            var methods = type.GetMethods().Where(m => m.IsPublic).ToList();

            foreach (MethodInfo method in methods)
            {
                result.Add(method.Name.ToLowerInvariant(), method);
            }
            return result;
        }
    }
}
