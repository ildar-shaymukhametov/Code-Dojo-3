using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace src
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
    
    public interface IDataProvider
    {
        string GetData();
    }
    
    public class MinTempSpreadDayCalculator
    {
        private readonly IDataProvider dataProvider;
        private readonly MinSpreadIndexCalculator minSpreadCalculator;
        private readonly IParser<(int Day, int MaxTemp, int MinTemp)> parser;

        public MinTempSpreadDayCalculator(IDataProvider dataProvider, MinSpreadIndexCalculator minSpreadCalculator, IParser<(int Day, int MaxTemp, int MinTemp)> parser)
        {
            this.dataProvider = dataProvider;
            this.minSpreadCalculator = minSpreadCalculator;
            this.parser = parser;
        }

        public int GetDay()
        {
            var items = parser.Parse(dataProvider.GetData());
            var index = minSpreadCalculator.GetIndex(ToTuples(items));
            var result = items.ElementAt(index).Day;

            return result;
        }

        private static int ExtractNumber(string str)
        {
            return int.Parse(new string(str.Where(Char.IsDigit).ToArray()));
        }

        private static List<(int first, int second)> ToTuples(IEnumerable<(int Day, int MaxTemp, int MinTemp)> items)
        {
            return items.Select(x => (first: x.MinTemp, second: x.MaxTemp)).ToList();
        }
    }

    public class MinGoalSpreadTeamCalculator
    {
        private readonly IDataProvider dataProvider;
        private readonly MinSpreadIndexCalculator minSpreadCalculator;
        private readonly IParser<(string Team, int For, int Against)> parser;

        public MinGoalSpreadTeamCalculator(IDataProvider dataProvider, MinSpreadIndexCalculator minSpreadCalculator, IParser<(string Team, int For, int Against)> parser)
        {
            this.dataProvider = dataProvider;
            this.minSpreadCalculator = minSpreadCalculator;
            this.parser = parser;
        }

        public string GetTeam()
        {
            var items = parser.Parse(dataProvider.GetData());
            var index = minSpreadCalculator.GetIndex(ToTuples(items));
            var result = items.ElementAt(index).Team;

            return result;
        }

        private static List<(int first, int second)> ToTuples(IEnumerable<(string Team, int For, int Against)> items)
        {
            return items.Select(x => (first: x.For, second: x.Against)).ToList();
        }
    }

    public class MinSpreadIndexCalculator
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

    public interface IParser<T>
    {
        T[] Parse(string data);
    }

    public class Parser<T> : IParser<T>
    {
        private string regex;
        private Func<string[], object> selector;

        public Parser(string regex, Func<string[], object> selector)
        {
            this.regex = regex;
            this.selector = selector;
        }

        public T[] Parse(string data)
        {
            var result = data
                .Split("\r\n")
                .Where(x => Regex.IsMatch(x, regex))
                .Select(x => x
                    .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                    .ToArray())
                .Select(selector)
                .Cast<T>()
                .ToArray();

            return result;
        }
    }
}
