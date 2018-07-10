using System;

namespace Wilmar.Foundation
{
    /// <summary>
    /// 特性获取的元数据
    /// </summary>
    public class AttrbuteMetadata<TAttrbute> where TAttrbute : Attribute
    {
        /// <summary>
        /// 创建特性获取的元数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attrbute"></param>
        public AttrbuteMetadata(Type type, TAttrbute attrbute)
        {
            this.InstanceType = type;
            this.Attribute = attrbute;
        }
        /// <summary>
        /// 特性对象
        /// </summary>
        public TAttrbute Attribute { get; private set; }

        /// <summary>
        /// 特性实例化对象类型
        /// </summary>
        public Type InstanceType { get; private set; }
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <returns></returns>
        public object CreateInstance(params object[] args)
        {
            return Activator.CreateInstance(this.InstanceType, args);
        }
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T CreateInstance<T>(params object[] args) where T : class
        {
            return Activator.CreateInstance(this.InstanceType, args) as T;
        }
    }
}
