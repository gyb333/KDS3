using System;

namespace Wilmar.Foundation
{
    /// <summary>
    /// 程序集扩展类型
    /// </summary>
    [Flags]
    public enum EExtensionType : int
    {
        /// <summary>
        /// 设计器ViewModel定义
        /// </summary>
        ViewModel = 0x1,
        /// <summary>
        /// 设计元素Model定义
        /// </summary>
        Model = 0x2,
        /// <summary>
        /// 项目编译器
        /// </summary>
        Compile = 0x4,
        /// <summary>
        /// 项目元素生成器
        /// </summary>
        Build = 0x8
    }
}
