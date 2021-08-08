using System.Collections.Generic;
using src;
using Xunit;

namespace test
{
    public class UnitTest1
    {
        [Fact]
        public void MinTempSpreadDayCalculator___GetDay___Calculates_day_of_min_temp_spread()
        {
            var sut = new MinTempSpreadDayCalculator(new MinSpreadIndexCalculator());
            var content = @"  Dy MxT   MnT   AvT   HDDay  AvDP 1HrP TPcpn WxType PDir AvSp Dir MxS SkyC MxR MnR AvSLP

   1  10    5    74          53.8       0.00 F       280  9.6 270  17  1.6  93 23 1004.5
   2  8    1    71          46.5       0.00         330  8.7 340  23  3.3  70 28 1004.5
   3  12    10    66          39.6       0.00         350  5.0 350   9  2.8  59 24 1016.8
  mo  82.9  60.5  71.7    16  58.8       0.00              6.9          5.3";
            var actual = sut.GetDay(content);
            Assert.Equal(3, actual);
        }

        [Fact]
        public void MinGoalSpreadTeamCalculator___GetTeam___Returns_team_with_min_goal_spread()
        {
            var sut = new MinGoalSpreadTeamCalculator(new MinSpreadIndexCalculator());
            var content = @"       Team            P     W    L   D    F      A     Pts
    1. Arsenal         38    26   9   3    8  -  4    87
    2. Liverpool       38    24   8   6    17  -  3    80
    3. Manchester_U    38    24   5   9    11  -  5    77
   -------------------------------------------------------
   18. Ipswich         38     9   9  20    4  -  6    36
   19. Derby           38     8   6  24    13  -  6    30
   20. Leicester       38     5  13  20    33  -  6    28
";
            var actual = sut.GetTeam(content);
            Assert.Equal("Ipswich", actual);
        }

        [Fact]
        public void MinSpreadIndexCalculator___GetIndex___Returns_index_of_an_item_with_min_spread()
        {
            var list = new List<(int first, int second)>
            {
                (20, 10),
                (10, 5),
                (100, 50)
            };
            var index = new MinSpreadIndexCalculator().GetIndex(list);
            Assert.Equal(1, index);
        }
    }
}
