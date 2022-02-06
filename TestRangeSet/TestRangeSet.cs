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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace TestRangeSet
{
    [TestClass]
    public class TestRangeSet
    {
        [TestMethod]
        public void Intersection()
        {
            {
                var r1 = new RangeSet((2, 3));
                var r2 = new RangeSet((4, 5));
                Assert.IsTrue(r1.Intersect(r2).IsNull);
                Assert.IsTrue(r2.Intersect(r1).IsNull);
            }

            {
                var r1 = new RangeSet((2, 3));
                var r2 = new RangeSet((1, 5));
                Assert.IsTrue(r1.Intersect(r2) == r1);
                Assert.IsTrue(r2.Intersect(r1) == r1);
            }

            {
                var r1 = new RangeSet((1, 3));
                var r2 = new RangeSet((2, 4));
                var result = new RangeSet((2, 3));
                Assert.IsTrue(r1.Intersect(r2) == result);
                Assert.IsTrue(r2.Intersect(r1) == result);
            }


            {
                var r1 = new RangeSet((2, 4)).Union((6, 8));
                var r2 = new RangeSet((4, 6));
                Assert.IsTrue(r1.Intersect(r2).IsNull);
                Assert.IsTrue(r2.Intersect(r1).IsNull);
            }


            {
                var r1 = new RangeSet((0, 10));
                var r2 = new RangeSet((-1, 1), (5, 7), (9, 11));
                var result = new RangeSet((0, 1), (5, 7), (9, 10));
                Assert.IsTrue(r1.Intersect(r2) == result);
                Assert.IsTrue(r2.Intersect(r1) == result);
            }
        }

        [TestMethod]
        public void Subtraction()
        {
            {
                var r1 = new RangeSet((2, 3));
                var r2 = new RangeSet((4, 5));
                Assert.IsTrue(r1.Subtract(r2) == r1);
                Assert.IsTrue(r2.Subtract(r1) == r2);
            }

            {
                var r1 = new RangeSet((2, 3));
                var r2 = new RangeSet((1, 5));
                Assert.IsTrue(r1.Subtract(r2).IsNull);
                var result = new RangeSet((1, 2), (3, 5));
                Assert.IsTrue(r2.Subtract(r1) == result);
            }

            {
                var r1 = new RangeSet((1, 3));
                var r2 = new RangeSet((2, 4));
                var result1 = new RangeSet((1, 2));
                var result2 = new RangeSet((3, 4));
                Assert.IsTrue(r1.Subtract(r2) == result1);
                Assert.IsTrue(r2.Subtract(r1) == result2);
            }

            {
                var r1 = new RangeSet((2, 4), (6, 8));
                var r2 = new RangeSet((4, 6));
                Assert.IsTrue(r1 - r2 == r1);
                Assert.IsTrue(r2 - r1 == r2);
            }

            {
                var r1 = new RangeSet((0, 10));
                var r2 = new RangeSet((-1, 1), (5, 7), (9, 11));
                var result1 = new RangeSet((1, 5), (7, 9));
                var result2 = new RangeSet((-1, 0), (10, 11));
                Assert.IsTrue(r1.Subtract(r2) == result1);
                Assert.IsTrue(r2.Subtract(r1) == result2);
            }
        }

        [TestMethod]
        public void Union()
        {
            {
                var r1 = new RangeSet((2, 3));
                var r2 = new RangeSet((4, 5));
                var result = new RangeSet((2, 3), (4, 5));
                Assert.IsTrue(r1.Union(r2) == result);
                Assert.IsTrue(r2.Union(r1) == result);
            }

            {
                var r1 = new RangeSet((2, 3));
                var r2 = new RangeSet((1, 5));
                var result = new RangeSet((1, 5));
                Assert.IsTrue(r1.Union(r2) == result);
                Assert.IsTrue(r2.Union(r1) == result);
            }

            {
                var r1 = new RangeSet((1, 3));
                var r2 = new RangeSet((2, 4));
                var result = new RangeSet((1, 4));
                Assert.IsTrue(r1.Union(r2) == result);
                Assert.IsTrue(r2.Union(r1) == result);
            }

            {
                var r1 = new RangeSet((2, 4), (6, 8));
                var r2 = new RangeSet((4, 6));
                var result = new RangeSet((2, 8));
                Assert.IsTrue(r1.Union(r2) == result);
                Assert.IsTrue(r2.Union(r1) == result);
            }

            {
                var r1 = new RangeSet((0, 10));
                var r2 = new RangeSet((-1, 1), (5, 7), (9, 11));
                var result = new RangeSet((-1, 11));
                Assert.IsTrue(r1.Union(r2) == result);
                Assert.IsTrue(r2.Union(r1) == result);
            }
        }


        private static RangeSet RandomSet(Random r)
        {
            const int SegCount = 10;
            var parts = new List<double>();
            for (int i = 0; i < SegCount * 2; i++)
            {
                parts.Add(r.NextDouble());
            }
            parts.Sort();

            RangeSet result = RangeSet.Null;
            for (int i = 0; i < SegCount; i++)
            {
                result = result.Union((parts[i * 2], parts[i * 2 + 1]));
            }

            return result;
        }

        [TestMethod]
        public void SetProperties()
        {
            var r = new Random(1337);
            for (int i = 0; i < 100; i++)
            {
                var A = RandomSet(r);
                var B = RandomSet(r);
                var C = RandomSet(r);

                Assert.IsTrue(A.Intersect(B) == B.Intersect(A)); // Commutativity
                Assert.IsTrue(B.Union(C) == C.Union(B)); // Commutativity
                Assert.IsTrue(A.Intersect(B.Intersect(C)) == A.Intersect(B).Intersect(C)); // Associativity
                Assert.IsTrue(A.Union(B.Union(C)) == A.Union(B).Union(C)); // Associativity
                Assert.IsTrue(A.Intersect(B.Union(C)) == (A.Intersect(B)).Union(A.Intersect(C))); // Distributivity
                Assert.IsTrue(A.Intersect(A) == A); // Idempotent Law
                Assert.IsTrue(B.Union(B) == B); // Idempotent Law
                Assert.IsTrue(C.Intersect(RangeSet.Null) == RangeSet.Null);
                Assert.IsTrue(A.Intersect(RangeSet.Universal) == A);
                Assert.IsTrue(B.Union(RangeSet.Null) == B);
                Assert.IsTrue(RangeSet.Universal.Union(C) == RangeSet.Universal);
                Assert.IsTrue(C - C == RangeSet.Null);
                Assert.IsTrue(B - A == B.Intersect(A.Complement()));
                Assert.IsTrue(B - A == B - (A.Intersect(B)));
                Assert.IsTrue((A - B).Intersect(C) == A.Intersect(C) - B.Intersect(C));
                Assert.IsTrue(B.Complement().Complement() == B);                
                Assert.IsTrue(C.Union(C.Complement()) == RangeSet.Universal);
                Assert.IsTrue(A.Intersect(A.Complement()) == RangeSet.Null);
                Assert.IsTrue(B.Complement() == RangeSet.Universal - B);
                Assert.IsTrue(A.Union(B).Complement() == A.Complement().Intersect(B.Complement())); // De Morgan's Law
                Assert.IsTrue(C.Intersect(B).Complement() == C.Complement().Union(B.Complement())); // De Morgan's Law
            }

            Assert.IsTrue(RangeSet.Universal.Complement() == RangeSet.Null);
            Assert.IsTrue(RangeSet.Null.Complement() == RangeSet.Universal);
        }

        [TestMethod]
        public void Contains()
        {
            var A = new RangeSet((3, 8));
            Assert.IsTrue(A.Contains(3));
            Assert.IsTrue(A.Contains(6));
            Assert.IsFalse(A.Contains(8));

            var r = new Random(2491);
            for (int i = 0; i < 100; i++)
            {
                A = RandomSet(r);
                var value = r.NextDouble();
                Assert.IsTrue(A.Contains(value) == !A.Complement().Contains(value));
            }
        }

        [TestMethod]
        public void Combine()
        {
            var r = new Random(1999);
            for (int i = 0; i < 100; i++)
            {
                List<RangeSet.Range> ranges = new List<RangeSet.Range>();
                for (int j = 0; j < 10; j++)
                {
                    var min = r.NextDouble();
                    var max = r.NextDouble();
                    if (min >= max)
                    {
                        continue;
                    }
                    ranges.Add((min, max));
                }

                var slowRanges = RangeSet.Null;
                foreach (var range in ranges)
                {
                    slowRanges = slowRanges.Union(range);
                }

                Assert.IsTrue(new RangeSet(ranges.ToArray()) == slowRanges);
            }
        }
    }    
}
