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

    public class TempData
    {
        public int Day { get; set; }
        public int MaxTemp { get; set; }
        public int MinTemp { get; set; }
    }

    public class FootballData
    {
        public string Team { get; set; }
        public int For { get; set; }
        public int Against { get; set; }
    }

    public class DayCalculator
    {
        private readonly IDataProvider dataProvider;
        private readonly MinSpreadCalculator minSpreadCalculator;
        private readonly IParser<TempData> parser;

        public DayCalculator(IDataProvider dataProvider, MinSpreadCalculator minSpreadCalculator, IParser<TempData> parser)
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

        private static List<(int first, int second)> ToTuples(IEnumerable<TempData> items)
        {
            return items.Select(x => (first: x.MinTemp, second: x.MaxTemp)).ToList();
        }
    }

    public class TeamCalculator
    {
        private readonly IDataProvider dataProvider;
        private readonly MinSpreadCalculator minSpreadCalculator;
        private readonly IParser<FootballData> parser;

        public TeamCalculator(IDataProvider dataProvider, MinSpreadCalculator minSpreadCalculator, IParser<FootballData> parser)
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

        private static List<(int first, int second)> ToTuples(IEnumerable<FootballData> items)
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
