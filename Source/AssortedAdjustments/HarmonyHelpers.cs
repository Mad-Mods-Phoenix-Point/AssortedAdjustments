using System;
using System.Reflection;
using Harmony;

namespace AssortedAdjustments
{
    internal static class HarmonyHelpers
    {
        internal static void PatchGetter(HarmonyInstance harmony, Type targetClass, string methodName, Type patchClass, string prefix, string postfix = null)
        {
            HarmonyMethod methodPrefix = prefix == null ? null : GetTargetMethod(patchClass, prefix);
            HarmonyMethod methodPostfix = postfix == null ? null : GetTargetMethod(patchClass, postfix);

            harmony.Patch(targetClass.GetProperty(methodName).GetGetMethod(), methodPrefix, methodPostfix);
        }



        internal static void Patch(HarmonyInstance harmony, Type targetClass, string methodName, Type patchClass, string prefix, string postfix = null)
        {
            HarmonyMethod methodPrefix = prefix == null ? null : GetTargetMethod(patchClass, prefix);
            HarmonyMethod methodPostfix = postfix == null ? null : GetTargetMethod(patchClass, postfix);

            harmony.Patch(targetClass.GetMethod(methodName, AccessTools.all), methodPrefix, methodPostfix);
        }

        internal static void Patch(HarmonyInstance harmony, string fullyQualifiedTargetMethod, string fullyQualifiedPrefix, string fullyQualifiedPostfix = null)
        {
            HarmonyMethod methodPrefix = fullyQualifiedPrefix == null ? null : GetTargetMethod(fullyQualifiedPrefix);
            HarmonyMethod methodPostfix = fullyQualifiedPostfix == null ? null : GetTargetMethod(fullyQualifiedPostfix);

            harmony.Patch(AccessTools.Method(fullyQualifiedTargetMethod), methodPrefix, methodPostfix);
        }



        internal static HarmonyMethod GetTargetMethod(Type patchClass, string method)
        {
            MethodInfo mi = patchClass.GetMethod(method);
            if (mi == null)
            {
                throw new NullReferenceException(method + " is null");
            }
            return new HarmonyMethod(mi);
        }

        internal static HarmonyMethod GetTargetMethod(string fullyQualifiedMethod)
        {
            MethodInfo mi = AccessTools.Method(fullyQualifiedMethod);
            if (mi == null)
            {
                throw new NullReferenceException(fullyQualifiedMethod + " is null");
            }
            return new HarmonyMethod(mi);
        }
    }
}
