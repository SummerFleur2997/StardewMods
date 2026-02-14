namespace SummerFleursBetterHats.HatWithEffects;

public partial class HatWithEffects
{
    private static readonly Random R = new();

    /// <summary>
    /// Action for the Cat Ears: play the sound of a cat randomly. 
    /// </summary>
    private static void Action_CatEars()
    {
        var j = R.Next(8);
        for (var i = 0; i < j; i++) R.Next();

        if (R.NextDouble() < 0.167)
            Game1.playSound("cat");
    }
}