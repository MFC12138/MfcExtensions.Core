using Microsoft.Extensions.DependencyInjection;

namespace Mfc.DependencyInjection.Extensions;

/// <summary>
/// 用于标注需要自动注入到依赖注入容器中的类
/// </summary>
/// <param name="serviceLifetime">服务的生命周期，默认为 Scoped</param>
/// <param name="serviceType">服务的注册类型（通常为接口或抽象类），如果为 null，则默认使用当前类类型</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class MfcServiceAttribute(ServiceLifetime serviceLifetime = ServiceLifetime.Scoped, Type? serviceType = null) : Attribute
{
    public ServiceLifetime ServiceLifetime { get; } = serviceLifetime;
    public Type? ServiceType { get; } = serviceType;
}