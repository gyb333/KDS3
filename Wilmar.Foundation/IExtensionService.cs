using System;
using System.Collections.Generic;
using System.Reflection;

namespace Wilmar.Foundation
{
    /// <summary>
    /// 平台扩展服务
    /// </summary>
    public interface IExtensionService
    {
        /// <summary>
        /// 初始化扩展服务
        /// </summary>
        void Initialize(params Assembly[] assemblys);
        /// <summary>
        /// 按指定的扩展类型检索指定的扩展程序集
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IEnumerable<Assembly> Assemblys(EExtensionType? type = null);
        /// <summary>
        /// 按指定的扩展类型检索指定的扩展程序集中指定基类的类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseType"></param>
        /// <returns></returns>
        IEnumerable<Type> Classs(EExtensionType? type, Type baseType = null);
        /// <summary>
        /// 按指定的扩展类型检索指定的扩展程序集中指定基类的类型，并且该类型具有指定特性
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="type"></param>
        /// <param name="action"></param>
        /// <param name="baseType"></param>
        void Classs<TAttribute>(EExtensionType? type, Action<Type, TAttribute> action, Type baseType = null) where TAttribute : Attribute;
    }
}
