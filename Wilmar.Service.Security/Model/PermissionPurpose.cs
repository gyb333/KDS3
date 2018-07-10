using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Foundation;
using Wilmar.Service.Common.Attributes;

namespace Wilmar.Service.Security.Model
{
    /// <summary>
    /// 权限目的定义
    /// </summary>
    public class PermissionPurpose
    {
        public PermissionPurpose(PermissionPurposeAttribute attr)
        {
            Attrbute = attr;
        }
        private PermissionPurposeAttribute Attrbute;

        public int CompileId { get { return Attrbute.CompileId; } }
        /// <summary>
        /// 权限目的ID
        /// </summary>
        public short PurposeId { get { return Attrbute.PurposeId; } }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get { return Attrbute.Title; } }
        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get { return Attrbute.Description; } }
        /// <summary>
        /// 权限值数据类型
        /// </summary>
        public EDataBaseType DataType { get { return Attrbute.DataType; } }
        /// <summary>
        /// 是否缓存权限数据
        /// </summary>
        public bool IsCache { get { return Attrbute.IsCache; } }
    }
}
