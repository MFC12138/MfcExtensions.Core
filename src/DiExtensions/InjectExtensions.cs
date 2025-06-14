using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Mfc.DependencyInjection.Extensions;

public static class InjectExtensions
{
    /// <summary>
    /// 扫描并注册多个程序集中的服务类，将所有标注了 <see cref="MfcServiceAttribute"/> 的类型统一注册到依赖注入容器中
    /// </summary>
    /// <param name="serviceCollection">要注册服务的依赖注入容器</param>
    /// <param name="assemblies">要扫描的程序集集合</param>
    /// <returns>返回原始的 <see cref="IServiceCollection"/> 实例，以支持链式调用</returns>
    public static IServiceCollection AddMfcServicesFromAssemblies(this IServiceCollection serviceCollection, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            serviceCollection.AddMfcServicesFromAssembly(assembly);
        }
        return serviceCollection;
    }
    
    /// <summary>
    /// 扫描指定程序集中的公共类，查找标注了 <see cref="MfcServiceAttribute"/> 的类型，并根据特性配置将其注册到依赖注入容器中
    /// </summary>
    /// <param name="serviceCollection">要注册服务的依赖注入容器</param>
    /// <param name="assembly">要扫描的程序集</param>
    /// <returns>返回原始的 <see cref="IServiceCollection"/> 实例，以支持链式调用</returns>
    public static IServiceCollection AddMfcServicesFromAssembly(this IServiceCollection serviceCollection, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => t.IsClass)
            .Where(t => t.IsPublic)
            .ToList();
        foreach (var type in types)
        {
            var attribute = type.GetCustomAttribute<MfcServiceAttribute>();
            if (attribute is not null)
            {
                serviceCollection.InjectService(type, attribute);
            }
        }
        return serviceCollection;
    }

    /// <summary>
    /// 根据指定的服务生命周期与服务类型，将类注册到依赖注入容器中
    /// </summary>
    /// <param name="serviceCollection">要注册到的服务容器</param>
    /// <param name="type">要注册的实现类型</param>
    /// <param name="attribute">标注在类上的 <see cref="MfcServiceAttribute"/>，包含注册信息</param>
    /// <exception cref="ArgumentOutOfRangeException">当 <see cref="ServiceLifetime"/> 的值不是 Singleton、Scoped 或 Transient 之一时抛出</exception>
    private static void InjectService(this IServiceCollection serviceCollection, Type type, MfcServiceAttribute attribute)
    {
        switch (attribute.ServiceLifetime)
        {
            case ServiceLifetime.Singleton:
                if (attribute.ServiceType is null)
                {
                    serviceCollection.AddSingleton(type);
                }
                else
                {
                    serviceCollection.AddSingleton(attribute.ServiceType, type);
                }
                break;
            case ServiceLifetime.Scoped:
                if (attribute.ServiceType is null)
                {
                    serviceCollection.AddScoped(type);
                }
                else
                {
                    serviceCollection.AddScoped(attribute.ServiceType, type);
                }
                break;
            case ServiceLifetime.Transient:
                if (attribute.ServiceType is null)
                {
                    serviceCollection.AddTransient(type);
                }
                else
                {
                    serviceCollection.AddTransient(attribute.ServiceType, type);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}