public class Dice
{
    private static Random rand = new Random();
    public int NumberOfDice { get; set; }
    public int SidesPerDice { get; set; }
    public int Modifier { get; set; }

    public Dice(int numberOfDice, int sidesPerDice, int modifier)
    {
        NumberOfDice = numberOfDice;
        SidesPerDice = sidesPerDice;
        Modifier = modifier;
    }

    public int Throw()
    {
        int result = 0;

        for (int i = 0; i < NumberOfDice; i++)
        {
            result += rand.Next(1, SidesPerDice+1);
        }
        result += Modifier;

        return result;
    }

    public override string ToString()
    {
        return $"{NumberOfDice}d{SidesPerDice}+{Modifier}";
    }
}