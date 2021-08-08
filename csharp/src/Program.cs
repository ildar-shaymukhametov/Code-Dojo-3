using System;
using System.Collections.Generic;
using System.Linq;

namespace src
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
    
    public class MinTempSpreadDayCalculator
    {
        private readonly MinSpreadCalculator minSpreadCalculator;

        public MinTempSpreadDayCalculator(MinSpreadCalculator minSpreadCalculator)
        {
            this.minSpreadCalculator = minSpreadCalculator;
        }

        public int GetDay(string fileContents)
        {
            var items = fileContents
                .Split("\r\n")
                .Skip(2)
                .SkipLast(1)
                .Select(x => x
                    .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                    .ToArray())
                .Select(x =>
                (
                    Day: int.Parse(x[0]),
                    MaxTemp: int.Parse(new string(x[1].Where(Char.IsDigit).ToArray())),
                    MinTemp: int.Parse(new string(x[2].Where(Char.IsDigit).ToArray()))
                ))
                .ToArray();

            var index = minSpreadCalculator.GetIndex(ToTuples(items));
            var result = items.ElementAt(index).Day;

            return result;
        }
        
        private static List<(int first, int second)> ToTuples(IEnumerable<(int Day, int MaxTemp, int MinTemp)> items)
        {
            return items.Select(x => (first: x.MinTemp, second: x.MaxTemp)).ToList();
        }
    }

    public class MinGoalSpreadTeamCalculator
    {
        private readonly MinSpreadCalculator minSpreadCalculator;

        public MinGoalSpreadTeamCalculator(MinSpreadCalculator minSpreadCalculator)
        {
            this.minSpreadCalculator = minSpreadCalculator;
        }

        public string GetTeam(string fileContents)
        {
            var items = fileContents
                .Split("\r\n")
                .Skip(1)
                .Where(x => !x.Trim().All(c => c == '-'))
                .Select(x => x
                    .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                    .ToArray())
                .Select(x =>
                (
                    Team: x[1],
                    For: int.Parse(x[6]),
                    Against: int.Parse(x[8])
                ))
                .ToArray();
                
            var index = minSpreadCalculator.GetIndex(ToTuples(items));
            var result = items.ElementAt(index).Team;

            return result;
        }

        private static List<(int first, int second)> ToTuples(IEnumerable<(string Team, int For, int Against)> items)
        {
            return items.Select(x => (first: x.For, second: x.Against)).ToList();
        }
    }

    public class MinSpreadCalculator
    {
        public int GetIndex(List<(int first, int second)> items)
        {
            var item = items.Aggregate((acc, v) =>
            {
                var prevSpread = Math.Abs(acc.first - acc.second);
                var currSpread = Math.Abs(v.first - v.second);
                if (prevSpread < currSpread)
                {
                    return acc;
                }
                return v;
            });

            return items.IndexOf(item);
        }
    }
}
