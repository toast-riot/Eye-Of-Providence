using System;
using System.Linq;
using System.Reflection;

namespace BepInExHelpers;

[AttributeUsage(AttributeTargets.Method)]
internal class PluginAwakeAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
internal class PluginDestroyAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
internal class PluginUpdateAttribute : Attribute
{
}

internal static class PluginEvents
{
    static readonly MethodInfo[] UpdateMethods = GetMethods(typeof(PluginUpdateAttribute));

    static MethodInfo[] GetMethods(Type attributeType)
    {
        return [..Assembly.GetExecutingAssembly().GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            .Where(m => m.GetCustomAttribute(attributeType) != null)];
    }

    internal static void Awake()
    {
        foreach (var method in GetMethods(typeof(PluginAwakeAttribute))) method.Invoke(null, null);
    }

    internal static void Destroy()
    {
        foreach (var method in GetMethods(typeof(PluginDestroyAttribute))) method.Invoke(null, null);
    }

    internal static void Update()
    {
        foreach (var method in UpdateMethods) method.Invoke(null, null);
    }
}