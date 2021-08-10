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

    public class Temperature
    {
        private readonly IDataProvider dataProvider;
        private readonly SpreadCalculator spreadCalculator;
        private readonly IParser<TempData> parser;

        public Temperature(IDataProvider dataProvider, SpreadCalculator spreadCalculator, IParser<TempData> parser)
        {
            this.dataProvider = dataProvider;
            this.spreadCalculator = spreadCalculator;
            this.parser = parser;
        }

        public int GetDay()
        {
            var items = parser.Parse(dataProvider.GetData());
            var index = spreadCalculator.GetIndex(ToTuples(items));
            var result = items.ElementAt(index).Day;

            return result;
        }

        private static List<(int first, int second)> ToTuples(IEnumerable<TempData> items)
        {
            return items.Select(x => (first: x.MinTemp, second: x.MaxTemp)).ToList();
        }
    }

    public class Football
    {
        private readonly IDataProvider dataProvider;
        private readonly SpreadCalculator spreadCalculator;
        private readonly IParser<FootballData> parser;

        public Football(IDataProvider dataProvider, SpreadCalculator spreadCalculator, IParser<FootballData> parser)
        {
            this.dataProvider = dataProvider;
            this.spreadCalculator = spreadCalculator;
            this.parser = parser;
        }

        public string GetTeam()
        {
            var items = parser.Parse(dataProvider.GetData());
            var index = spreadCalculator.GetIndex(ToTuples(items));
            var result = items.ElementAt(index).Team;

            return result;
        }

        private static List<(int first, int second)> ToTuples(IEnumerable<FootballData> items)
        {
            return items.Select(x => (first: x.For, second: x.Against)).ToList();
        }
    }

    public class SpreadCalculator
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
                .Split("\n")
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
