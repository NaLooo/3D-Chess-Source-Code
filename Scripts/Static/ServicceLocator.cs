using System.Collections.Generic;
using System;

public static class ServiceLocator
{
    private static Dictionary<Type, object> services = new Dictionary<Type, object>();

    public static void RegisterService<T>(T instance)
    {
        services[typeof(T)] = instance;
    }

    public static T AccessService<T>()
    {
        if (services.ContainsKey(typeof(T)))
        {
            return (T)services[typeof(T)];
        }
        else
        {
            return default;
        }
    }
}
