using System.Data;

namespace BetterRetainingSoils.Framework;

public static class Util
{
    /// <summary>
    /// 将已游玩的游戏天数转化为具体日期。 Convert the number of days played to a specific date.
    /// </summary>
    /// <returns>转化得到的具体日期。 A specific date. Format like: Y1,Summer,22</returns>
    /// <exception cref="DataException">How did you trigger that?</exception>
    public static string ConvertToDate(uint daysPlayed)
    {
        var totalDays = daysPlayed - 1;
        var year = totalDays / 112 + 1;
        var month = (totalDays % 112 / 28) switch
        {
            0 => "Spring",
            1 => "Summer",
            2 => "Fall",
            3 => "Winter",
            _ => throw new DataException()
        };
        var day = totalDays % 28 + 1;
        return $"Y{year},{month},{day}";
    }

    /// <summary>
    /// 将具体日期转化为已游玩的游戏天数。 Convert a specific date to the number of days played.
    /// </summary>
    /// <param name="date">Format like: Y1,Summer,22</param>
    /// <returns>当前已游玩的天数，应为一个大于 0 的整数。若返回 0 则代表解析错误。
    /// The number of days played, should be an integer that greater than 0.
    /// If an error occured when phasing, return 0.</returns>
    /// <exception cref="DataException">季节名称错误。 The name of the season is incorrect.</exception>
    public static uint ConvertToDaysPlayed(string date)
    {
        try
        {
            var args = date.Split(',');
            var year = uint.Parse(args[0][1..]) - 1;
            uint month = args[1] switch
            {
                "Spring" => 0,
                "Summer" => 1,
                "Fall" => 2,
                "Winter" => 3,
                _ => throw new DataException()
            };
            var day = uint.Parse(args[2]);
            return year * 112 + month * 28 + day;
        }
        catch
        {
            return 0;
        }
    }
}