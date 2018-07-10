using System;
using System.Globalization;

namespace Wilmar.Foundation.Common
{
    /// <summary>
    /// 常用<see cref="string"/>工具类
    /// </summary>
    public static class UtilityString
    {
        /// <summary>
        /// 将指定字符串中的格式项替换为指定数组中相应对象的字符串表示形式。
        /// </summary>
        /// <param name="source">复合格式字符串。</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns>格式化后的字符串。</returns>
        public static string Format(string source, params object[] args)
        {
            return String.Format(CultureInfo.CurrentCulture, source, args);
        }
        /// <summary>
        /// 根据指定区域性的大小写规则返回此字符串转换为小写形式的副本。 <see cref="M:CultureInfo.CurrentCulture"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToLower(string value)
        {
            return value.ToLower(CultureInfo.CurrentCulture);
        }
        /// <summary>
        /// 根据指定区域性的大小写规则返回此字符串转换为大写形式的副本。 <see cref="M:CultureInfo.CurrentCulture"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToUpper(string value)
        {
            return value.ToUpper(CultureInfo.CurrentCulture);
        }
    }
}
