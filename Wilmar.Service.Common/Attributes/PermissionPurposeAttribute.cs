using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Foundation;

namespace Wilmar.Service.Common.Attributes
{
    /// <summary>
    /// 权限目的扩展特性，该特性用于定义权限控制的声明及控制行为
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class PermissionPurposeAttribute : Attribute
    {
        /// <summary>
        /// 创建权限目的扩展特性
        /// </summary>
        /// <param name="compileId">编译器ID</param>
        /// <param name="purposeId">目的ID</param>
        /// <param name="title">标题</param>
        public PermissionPurposeAttribute(int compileId, short purposeId, string title)
        {
            CompileId = compileId;
            PurposeId = purposeId;
            Title = title;
        }
        /// <summary>
        /// 编译器ID，某种目的的权限需要与指定的编译关联，在设置权限时会验证当前编译器与设置权限是否匹配
        /// </summary>
        public int CompileId { get; private set; }
        /// <summary>
        /// 权限目的ID
        /// </summary>
        public short PurposeId { get; private set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; private set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 权限值数据类型
        /// </summary>
        public EDataBaseType DataType { get; set; } = EDataBaseType.Boolean;
        /// <summary>
        /// 是否缓存权限数据
        /// </summary>
        public bool IsCache { get; set; } = true;
    }
}
