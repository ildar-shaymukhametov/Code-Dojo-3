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

    public class MinGoalSpreadCalculator
    {
        public string GetTeam(string fileContents)
        {
            var result = fileContents
                .Split("\r\n")
                .Skip(1)
                .Where(x => !x.Trim().All(c => c == '-'))
                .Select(x => x
                    .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                    .ToArray())
                .Select(x => new
                {
                    Team = x[1],
                    For = int.Parse(x[6]),
                    Against = int.Parse(x[8])
                })
                .Aggregate((acc, v) =>
                {
                    var prevSpread = Math.Abs(acc.Against - acc.For);
                    var currSpread = Math.Abs(v.Against - v.For);
                    if (prevSpread < currSpread)
                    {
                        return acc;
                    }
                    return v;
                })
                .Team;

            return result;
        }
    }
}
