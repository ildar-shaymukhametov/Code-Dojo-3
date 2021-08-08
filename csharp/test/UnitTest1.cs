using src;
using Xunit;

namespace test
{
    public class UnitTest1
    {
        [Fact]
        public void MinTempSpreadCalculator___GetDay___Calculates_day_of_min_temp_spread()
        {
            var sut = new MinTempSpreadCalculator();
            var content = @"  Dy MxT   MnT   AvT   HDDay  AvDP 1HrP TPcpn WxType PDir AvSp Dir MxS SkyC MxR MnR AvSLP

   1  10    5    74          53.8       0.00 F       280  9.6 270  17  1.6  93 23 1004.5
   2  8    1    71          46.5       0.00         330  8.7 340  23  3.3  70 28 1004.5
   3  12    10    66          39.6       0.00         350  5.0 350   9  2.8  59 24 1016.8
  mo  82.9  60.5  71.7    16  58.8       0.00              6.9          5.3";
            var actual = sut.GetDay(content);
            Assert.Equal(3, actual);
        }
    }
}
