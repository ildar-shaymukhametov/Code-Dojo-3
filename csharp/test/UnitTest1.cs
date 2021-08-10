using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using src;
using Xunit;

namespace test
{
    public class UnitTest1
    {
        [Fact]
        public void Temperature___GetDay___Calculates_day_of_min_temp_spread()
        {
            var parser = Substitute.For<IParser<TempData>>();
            parser.Parse(default).ReturnsForAnyArgs(new[] { new TempData { Day = 1, MaxTemp = 10, MinTemp = 5 } });
            var sut = new Temperature(Substitute.For<IDataProvider>(), new SpreadCalculator(), parser);
            var actual = sut.GetDay();
            Assert.Equal(1, actual);
        }

        [Fact]
        public void Football___GetTeam___Returns_team_with_min_goal_spread()
        {
            var parser = Substitute.For<IParser<FootballData>>();
            parser.Parse(default).ReturnsForAnyArgs(new[] { new FootballData { Team = "Ipswich", For = 10, Against = 5 } });
            var sut = new Football(Substitute.For<IDataProvider>(), new SpreadCalculator(), parser);
            var actual = sut.GetTeam();
            Assert.Equal("Ipswich", actual);
        }

        [Fact]
        public void MinSpreadCalculator___GetIndex___Returns_index_of_an_item_with_min_spread()
        {
            var list = new List<(int first, int second)>
            {
                (20, 10),
                (10, 5),
                (100, 50)
            };
            var index = new SpreadCalculator().GetIndex(list);
            Assert.Equal(1, index);
        }

        [Fact]
        public void Parser___Parses_footbal_data()
        {
            var data = @"       Team            P     W    L   D    F      A     Pts
    1. Arsenal         38    26   9   3    8  -  4    87
   -------------------------------------------------------
";
            var regex = @"\s+\d{1,2}\.\s([A-Za-z_]+)";
            var selector = new Func<string[], object>(x => new FootballData
            {
                Team = x[1],
                For = int.Parse(x[6]),
                Against = int.Parse(x[8])
            });

            var items = new Parser<FootballData>(regex, selector).Parse(data);
            Assert.Collection(items, x => Assert.True(x.Team == "Arsenal" && x.For == 8 && x.Against == 4));
        }

        [Fact]
        public void Parser___Parses_weather_data()
        {
            var data = @"  Dy MxT   MnT   AvT   HDDay  AvDP 1HrP TPcpn WxType PDir AvSp Dir MxS SkyC MxR MnR AvSLP

   1  88    59    74          53.8       0.00 F       280  9.6 270  17  1.6  93 23 1004.5
  mo  82.9  60.5  71.7    16  58.8       0.00              6.9          5.3";
            var regex = @"^\s+\d{1,2}\s.*$";
            var selector = new Func<string[], object>(x => new TempData
            {
                Day = int.Parse(x[0]),
                MaxTemp = int.Parse(new string(x[1].Where(Char.IsDigit).ToArray())),
                MinTemp = int.Parse(new string(x[2].Where(Char.IsDigit).ToArray()))
            });

            var items = new Parser<TempData>(regex, selector).Parse(data);
            Assert.Collection(items, x => Assert.True(x.Day == 1 && x.MaxTemp == 88 && x.MinTemp == 59));
        }
    }
}
