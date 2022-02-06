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
using System.Linq;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var random = new Random();
            const string UnitText = "meter";
            const double WallLength = 20;
            const double HalfGap = 0.5;

            while (true)
            {
                Console.WriteLine($"There is a {WallLength} {UnitText} long wall in front of you and you have a cannon.");
                Console.WriteLine($"When you fire your cannon a random spot along this length is targeted and a {HalfGap} {UnitText} space is blown out on either side of the target.");
                Console.WriteLine($"I will tell you the largest combined gap produced by your cannonade."); 
                Console.WriteLine($"How many times do you wish to fire your cannon? (0 to quit)");

                int fireCount;

                while (!int.TryParse(Console.ReadLine(), out fireCount)) ;                

                if (fireCount == 0)
                {
                    break;
                }                

                RangeSet originalWall = new RangeSet((0, WallLength));
                RangeSet curWall = originalWall;
                for (int i = 0; i < fireCount; i++)
                {
                    double randomSpot = random.NextDouble() * WallLength;
                    curWall = curWall.Subtract((randomSpot - HalfGap, randomSpot + HalfGap));
                }

                Console.WriteLine();
                if (curWall.IsNull)
                {
                    Console.WriteLine("Congratulations!  You destoyed the entire wall!");
                }
                else
                {
                    var largestGap = curWall.Complement().Intersect(originalWall).Max(range => range.Length);
                    Console.WriteLine($"You have created a {largestGap:0.##} {UnitText} gap in the wall");
                }
                Console.ReadKey(true);
                Console.WriteLine();
                Console.WriteLine();

            }
        }
    }
}
