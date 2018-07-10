using System;

namespace Wilmar.Foundation.Attributes
{
    /// <summary>
    /// 程序集扩展特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyExtensionAttribute : Attribute
    {
        /// <summary>
        /// 初始化程序集扩展特性
        /// </summary>
        /// <param name="extensionType">程序集扩展的类型</param>
        public AssemblyExtensionAttribute(EExtensionType extensionType)
        {
            this.ExtensionType = extensionType;
        }
        /// <summary>
        /// 标识当前程序集扩展的类型
        /// </summary>
        public EExtensionType ExtensionType { get; private set; }
    }
}
