using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlLib
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Python風の配列スライス、source[startIndex]からsource[endIndex-1]までを返す
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex">-1の場合はsource.Lengthが指定されたとみなす</param>
        /// <returns></returns>
        public static T[] Slice<T>(this T[] source, int startIndex, int endIndex = -1)
            where T : new()
        {
            var count = (endIndex != -1) ?
                endIndex - startIndex : source.Length - startIndex;
            var destination = new T[count];
            Array.Copy(source, startIndex, destination, 0, count);
            return destination;
        }
    }
}
