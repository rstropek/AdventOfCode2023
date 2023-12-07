var handLines = File.ReadAllText("data.txt").Split('\n').Select(l => l.Split(' '));

var hands = handLines.Select(l => new { Hand = new Hand(l[0]), Bid = long.Parse(l[1]) }).ToList();
hands.Sort((a, b) => a.Hand.CompareTo(b.Hand));
Console.WriteLine(hands.Select((h, ix) => h.Bid * (ix + 1)).Sum());

var hands2 = handLines.Select(l => new { Hand = new Hand2(l[0]), Bid = long.Parse(l[1]) }).ToList();
hands2.Sort((a, b) => a.Hand.CompareTo(b.Hand));
Console.WriteLine(hands2.Select((h, ix) => h.Bid * (ix + 1)).Sum());

enum HandType { HighCard, OnePair, TwoPair, ThreeOfAKind, FullHouse, FourOfAKind, FiveOfAKind }

abstract class HandBase : IComparable<HandBase>
{
    protected abstract char[] Cards { get; }

    protected readonly int[] CardValues = new int[5];

    public HandBase(string hand)
    {
        for (int i = 0; i < 5; i++) { CardValues[i] = Array.IndexOf(Cards, hand[i]); }
    }

    protected record HandAnalysis(int[] ValueCounts, int PairCount, int ThreeCount, int FourCount, int FiveCount);

    protected HandAnalysis Analyze(int startAt)
    {
        var valueCounts = new int[Cards.Length];
        foreach (var value in CardValues)
        {
            valueCounts[value]++;
        }

        int pairCount = 0, threeCount = 0, fourCount = 0, fiveCount = 0;
        for (var i = startAt; i < valueCounts.Length; i++)
        {
            var count = valueCounts[i];
            if (count == 2) { pairCount++; }
            else if (count == 3) { threeCount++; }
            else if (count == 4) { fourCount++; }
            else if (count == 5) { fiveCount++; }
        }

        return new HandAnalysis(valueCounts, pairCount, threeCount, fourCount, fiveCount);
    }

    protected abstract HandType GetHandType();

    public int CompareTo(HandBase? other)
    {
        if (other == null) { return 1; }

        var thisType = GetHandType();
        var otherType = other.GetHandType();

        if (thisType != otherType) { return thisType.CompareTo(otherType); }

        for (var i = 0; i < CardValues.Length; i++)
        {
            if (CardValues[i] != other.CardValues[i])
            {
                return CardValues[i].CompareTo(other.CardValues[i]);
            }
        }

        return 0;
    }
}

class Hand(string hand) : HandBase(hand)
{
    protected override char[] Cards => ['2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A'];

    protected override HandType GetHandType()
    {
        var handAnalysis = Analyze(0);

        if (handAnalysis.FiveCount == 1) { return HandType.FiveOfAKind; }
        else if (handAnalysis.FourCount == 1) { return HandType.FourOfAKind; }
        else if (handAnalysis.ThreeCount == 1 && handAnalysis.PairCount == 1) { return HandType.FullHouse; }
        else if (handAnalysis.ThreeCount == 1) { return HandType.ThreeOfAKind; }
        else if (handAnalysis.PairCount == 2) { return HandType.TwoPair; }
        else if (handAnalysis.PairCount == 1) { return HandType.OnePair; }
        else { return HandType.HighCard; }
    }
}

class Hand2(string hand) : HandBase(hand)
{
    protected override char[] Cards => ['J', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'Q', 'K', 'A'];

    protected override HandType GetHandType()
    {
        var handAnalysis = Analyze(1);

        var jokers = handAnalysis.ValueCounts[0];
        if (jokers is 5 or 4
            || (handAnalysis.PairCount == 1 && jokers == 3)
            || (handAnalysis.ThreeCount == 1 && jokers == 2)
            || (handAnalysis.FourCount == 1 && jokers == 1)
            || handAnalysis.FiveCount == 1) { return HandType.FiveOfAKind; }
        else if (jokers == 3
            || (handAnalysis.PairCount == 1 && jokers == 2)
            || (handAnalysis.ThreeCount == 1 && jokers == 1)
            || handAnalysis.FourCount == 1) { return HandType.FourOfAKind; }
        else if ((handAnalysis.PairCount == 2 && jokers == 1)
            || (handAnalysis.ThreeCount == 1 && handAnalysis.PairCount == 1)) { return HandType.FullHouse; }
        else if (jokers == 2
            || (handAnalysis.PairCount == 1 && jokers == 1)
            || handAnalysis.ThreeCount == 1) { return HandType.ThreeOfAKind; }
        else if (handAnalysis.PairCount == 2) { return HandType.TwoPair; }
        else if (jokers == 1 || handAnalysis.PairCount == 1) { return HandType.OnePair; }
        else { return HandType.HighCard; }
    }
}