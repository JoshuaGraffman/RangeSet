/* 
 * Copyright © 2022 Joshua Graffman
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the “Software”), to deal in the Software without
 * restriction, including without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the 
 * Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or
 * substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
 * BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Immutable set of doubles in the form of {[rangeMin1, rangeMax1), [rangeMin2, rangeMax2)...}
/// </summary>
public class RangeSet : IEnumerable<RangeSet.Range>
{
    public struct Range
    {
        /// <summary>
        /// Min value (inclusive) of this range
        /// </summary>
        public double Min { get; }
        /// <summary>
        /// Max value (exclusive) of this range
        /// </summary>
        public double Max { get; }

        public double Length => Max - Min;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min">min value(inclusive) of the range</param>
        /// <param name="max">max value(exclusive) of the range</param>
        public Range(double min, double max)
        {
            if (min >= max)
            {
                throw new Exception("Invalid Range");
            }
            Min = min;
            Max = max;
        }

        public void Deconstruct(out double min, out double max)
        {
            min = Min;
            max = Max;
        }

        public static implicit operator Range((double min, double max) range) => new Range(range.min, range.max);

        public static bool operator ==(Range left, Range right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Range left, Range right)
        {
            return !(left == right);
        }

        public override string ToString() => $"[{Min}..{Max})";

        public override bool Equals(object obj)
        {
            return obj is Range range &&
                   Min == range.Min &&
                   Max == range.Max;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Min, Max);
        }
    }

    private readonly List<Range> m_Ranges;

    public RangeSet()
    {
        m_Ranges = new List<Range>();
    }

    public RangeSet(params Range[] ranges)
    {
        m_Ranges = CombineRanges(ranges).m_Ranges;
    }

    /// <summary>
    /// Returns set formed by the intersection of this and the combined ranges
    /// </summary>
    public RangeSet Intersect(params Range[] ranges) => Intersect(CombineRanges(ranges));
    /// <summary>
    /// Returns set formed by the intersection of this and other
    /// </summary>
    public RangeSet Intersect(RangeSet other)
    {
        int i1 = 0;
        int i2 = 0;
        var result = new RangeSet();

        while (i1 < m_Ranges.Count && i2 < other.m_Ranges.Count)
        {
            var r1 = m_Ranges[i1];
            var r2 = other.m_Ranges[i2];
            if (r1.Max <= r2.Min)
            {
                i1++;
            }
            else if (r2.Max <= r1.Min)
            {
                i2++;
            }
            else
            {
                Range inter = new Range(Math.Max(r1.Min, r2.Min), Math.Min(r1.Max, r2.Max));
                result.m_Ranges.Add(inter);
                if (r2.Max > r1.Max)
                {
                    i1++;
                }
                else if (r1.Max > r2.Max)
                {
                    i2++;
                }
                else
                {
                    i1++;
                    i2++;
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Returns the set formed by subtracting combined ranges from this set
    /// </summary>
    public RangeSet Subtract(params Range[] ranges) => Subtract(CombineRanges(ranges));
    /// <summary>
    /// Returns the set formed by subtracting other from this set
    /// </summary>
    public RangeSet Subtract(RangeSet other)
    {
        if (IsNull)
        {
            return Null;
        }

        int i1 = 0;
        int i2 = 0;
        var result = new RangeSet();

        (double min, double max) = m_Ranges[0];

        while (i2 < other.m_Ranges.Count)
        {
            var r2 = other.m_Ranges[i2];
            if (max <= r2.Min)
            {
                if (min != max)
                {
                    result.m_Ranges.Add((min, max));
                }
                i1++;
                if (i1 == m_Ranges.Count)
                {
                    return result;
                }

                (min, max) = m_Ranges[i1];
            }
            else if (r2.Max <= min)
            {
                i2++;
            }
            else
            {
                if (min < r2.Min)
                {
                    result.m_Ranges.Add((min, r2.Min));
                }

                if (r2.Max > max)
                {
                    i1++;
                    if (i1 == m_Ranges.Count)
                    {
                        return result;
                    }
                    (min, max) = m_Ranges[i1];
                }
                else
                {
                    min = r2.Max;
                    i2++;
                }
            }
        }
        if (min != max)
        {
            result.m_Ranges.Add((min, max));
        }
        for (int i = i1 + 1; i < m_Ranges.Count; i++)
        {
            result.m_Ranges.Add(m_Ranges[i]);
        }

        return result;
    }

    private static RangeSet CombineRanges(Range[] ranges)
    {
        if (ranges.Length == 0)
        {
            return Null;
        }

        var result = new RangeSet();

        List<Range> sorted = new List<Range>(ranges);
        sorted.Sort((r1, r2) => Math.Sign(r1.Min - r2.Min));

        result.m_Ranges.Add(sorted[0]);
        for (int i = 1; i < sorted.Count; i++)
        {
            var last = result.m_Ranges[^1];
            var cur = sorted[i];
            if (last.Max < cur.Min)
            {
                result.m_Ranges.Add(cur);
            }
            else if (cur.Max > last.Max)
            {
                result.m_Ranges[^1] = (last.Min, cur.Max);
            }
        }


        return result;
    }

    /// <summary>
    /// Returns set union of this and combined ranges
    /// </summary>
    public RangeSet Union(params Range[] ranges) => Union(CombineRanges(ranges));
    /// <summary>
    /// Returns set union of this and other
    /// </summary>
    public RangeSet Union(RangeSet other)
    {
        int i1 = 0;
        int i2 = 0;
        var result = new RangeSet();

        while (i1 < m_Ranges.Count && i2 < other.m_Ranges.Count)
        {
            var r1 = m_Ranges[i1];
            var r2 = other.m_Ranges[i2];
            if (r1.Max < r2.Min)
            {
                result.m_Ranges.Add(r1);
                i1++;
            }
            else if (r2.Max < r1.Min)
            {
                result.m_Ranges.Add(other.m_Ranges[i2]);
                i2++;
            }
            else
            {
                double min = Math.Min(r1.Min, r2.Min);
                double max = Math.Max(r1.Max, r2.Max);
                i1++;
                i2++;
                while (true)
                {
                    if (i1 < m_Ranges.Count && m_Ranges[i1].Min <= max)
                    {
                        if (m_Ranges[i1].Max > max)
                        {
                            max = m_Ranges[i1].Max;
                        }
                        i1++;
                    }
                    else if (i2 < other.m_Ranges.Count && other.m_Ranges[i2].Min <= max)
                    {
                        if (other.m_Ranges[i2].Max > max)
                        {
                            max = other.m_Ranges[i2].Max;
                        }
                        i2++;
                    }
                    else
                    {
                        break;
                    }
                }
                result.m_Ranges.Add((min, max));
            }
        }

        while (i1 < m_Ranges.Count)
        {
            result.m_Ranges.Add(m_Ranges[i1++]);
        }

        while (i2 < other.m_Ranges.Count)
        {
            result.m_Ranges.Add(other.m_Ranges[i2++]);
        }

        return result;
    }

    /// <summary>
    /// Returns a set containing all real numbers not in this set
    /// </summary>
    public RangeSet Complement()
    {
        if (IsNull)
        {
            return Universal;
        }

        var result = new RangeSet();
        double cur;
        int startIndex;
        if (double.IsNegativeInfinity(m_Ranges[0].Min))
        {
            cur = m_Ranges[0].Max;
            startIndex = 1;
        }
        else
        {
            cur = double.NegativeInfinity;
            startIndex = 0;
        }
        for (int i = startIndex; i < m_Ranges.Count; i++)
        {
            var segment = m_Ranges[i];
            result.m_Ranges.Add((cur, segment.Min));
            cur = segment.Max;
        }
        if (!double.IsPositiveInfinity(cur))
        {
            result.m_Ranges.Add((cur, double.PositiveInfinity));
        }
        return result;
    }

    /// <summary>
    /// Returns trus if value is included in this set
    /// </summary>
    public bool Contains(double value)
    {
        foreach (var range in m_Ranges)
        {
            if (value >= range.Min && value < range.Max)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns the combined length of all the ranges that make up this set.  It may be infinite.
    /// </summary>
    public double RangeLength()
    {
        double result = 0;
        foreach (var range in m_Ranges)
        {
            result += range.Length;
        }
        return result;
    }

    public bool IsNull => m_Ranges.Count == 0;

    public static RangeSet operator -(RangeSet r1, RangeSet r2) => r1.Subtract(r2);

    public static readonly RangeSet Null = new RangeSet();
    public static readonly RangeSet Universal = new RangeSet((double.NegativeInfinity, double.PositiveInfinity));

    public override string ToString() => $"{{{string.Join(",", m_Ranges)}}}";

    public override bool Equals(object obj)
    {
        if (obj is RangeSet set)
        {
            if (m_Ranges.Count != set.m_Ranges.Count)
            {
                return false;
            }
            for (int i = 0; i < m_Ranges.Count; i++)
            {
                if (m_Ranges[i] != set.m_Ranges[i])
                {
                    return false;
                }
            }
        }
        else
        {
            return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        HashCode h = new HashCode();
        foreach (var r in m_Ranges)
        {
            h.Add(r);
        }
        return h.ToHashCode();
    }

    public IEnumerator<Range> GetEnumerator()
    {
        return m_Ranges.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return m_Ranges.GetEnumerator();
    }

    public static bool operator ==(RangeSet left, RangeSet right)
    {
        return EqualityComparer<RangeSet>.Default.Equals(left, right);
    }

    public static bool operator !=(RangeSet left, RangeSet right)
    {
        return !(left == right);
    }
}
