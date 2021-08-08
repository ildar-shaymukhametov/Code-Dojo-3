using System;
using System.IO;
using System.Linq;

namespace src
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
    
    public class MinTempSpreadCalculator
    {
        public int GetDay(string fileContents)
        {
            var result = fileContents
                .Split("\r\n")
                .Skip(2)
                .SkipLast(1)
                .Select(x => x
                    .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                    .ToArray())
                .Select(x => new
                {
                    Day = int.Parse(x[0]),
                    MaxTemp = int.Parse(new string(x[1].Where(Char.IsDigit).ToArray())),
                    MinTemp = int.Parse(new string(x[2].Where(Char.IsDigit).ToArray()))
                })
                .Aggregate((acc, v) =>
                {
                    var prevSpread = acc.MaxTemp - acc.MinTemp;
                    var currSpread = v.MaxTemp - v.MinTemp;
                    if (prevSpread < currSpread)
                    {
                        return acc;
                    }
                    return v;
                })
                .Day;

            return result;
        }
    }
}
