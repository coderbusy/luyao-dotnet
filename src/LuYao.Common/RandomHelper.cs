using System;
using System.Collections.Generic;
#if !NET6_0_OR_GREATER
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endif

namespace LuYao;

/// <summary>
/// 随机数辅助类，提供常用的随机数生成与数据洗牌方法。
/// </summary>
public class RandomHelper
{
#if !NET6_0_OR_GREATER
    [ThreadStatic]
    private static Random _local;
    private static readonly Random Global = new Random();
#endif
    private static Random Instance
    {
        get
        {
#if NET6_0_OR_GREATER
            return Random.Shared;
#else
            if (_local is null)
            {
                int seed;
                lock (Global)
                {
                    seed = Global.Next();
                }

                _local = new Random(seed);
            }

            return _local;
#endif
        }
    }

    /// <summary>
    /// 返回一个非负随机整数。
    /// </summary>
    public static int Next() => Instance.Next();

    /// <summary>
    /// 返回一个小于所指定最大值的非负随机整数。
    /// </summary>
    /// <param name="maxValue">返回的随机数的上限（不含该值）。</param>
    public static int Next(int maxValue) => Instance.Next(maxValue);

    /// <summary>
    /// 返回一个指定范围内的随机整数。
    /// </summary>
    /// <param name="minValue">返回的随机数的下限（包含该值）。</param>
    /// <param name="maxValue">返回的随机数的上限（不含该值）。</param>
    public static int Next(int minValue, int maxValue) => Instance.Next(minValue, maxValue);

    /// <summary>
    /// 用随机字节填充指定的字节数组。
    /// </summary>
    /// <param name="buffer">要填充的字节数组。</param>
    public static void NextBytes(byte[] buffer) => Instance.NextBytes(buffer);

    /// <summary>
    /// 返回一个介于 0.0 和 1.0 之间的随机双精度浮点数。
    /// </summary>
    public static double NextDouble() => Instance.NextDouble();

    /// <summary>
    /// 对数组进行洗牌（随机排序）。
    /// </summary>
    /// <typeparam name="T">数组元素类型。</typeparam>
    /// <param name="array">要洗牌的数组。</param>
    public static void Shuffle<T>(T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            int k = Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }

    /// <summary>
    /// 对集合进行洗牌（随机排序）。
    /// </summary>
    /// <typeparam name="T">集合元素类型。</typeparam>
    /// <param name="array">要洗牌的集合。</param>
    public static void Shuffle<T>(IList<T> array)
    {
        int n = array.Count;
        while (n > 1)
        {
            int k = Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }
}
