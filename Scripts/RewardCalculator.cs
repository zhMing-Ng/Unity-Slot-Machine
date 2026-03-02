using UnityEngine;
using System.Collections.Generic;

public class RewardCalculator : MonoBehaviour
{
    public enum Symbol
    {
        SEVEN,
        WATERMELON,
        DIAMOND,
        CHERRY,
        GRAPE
    }

    private readonly Dictionary<Symbol, int> PAYOUTS = new Dictionary<Symbol, int>()
    {
        { Symbol.SEVEN, 100 },
        { Symbol.WATERMELON, 10 },
        { Symbol.DIAMOND, 25 },
        { Symbol.CHERRY, 10 },
        { Symbol.GRAPE, 10 }
    };

    private readonly Dictionary<Symbol, int> WEIGHTS = new Dictionary<Symbol, int>()
    {
        { Symbol.SEVEN, 1 },
        { Symbol.WATERMELON, 10 },
        { Symbol.DIAMOND, 4 },
        { Symbol.CHERRY, 10 },
        { Symbol.GRAPE, 10 }
    };

    private readonly Dictionary<Symbol, int> OFFSET = new Dictionary<Symbol, int>()
    {
        { Symbol.SEVEN, -4 },
        { Symbol.WATERMELON, -3 },
        { Symbol.DIAMOND, -2 },
        { Symbol.CHERRY, -1 },
        { Symbol.GRAPE, 0 }
    };

    private int totalWeight;

    void Awake()
    {
        foreach (var weight in WEIGHTS.Values)
        {
            totalWeight += weight;
        }
    }

    public Symbol RollSymbol()
    {
        int r = Random.Range(1, totalWeight + 1);
        int cumulative = 0;

        foreach (var pair in WEIGHTS)
        {
            cumulative += pair.Value;
            if (r <= cumulative)
                return pair.Key;
        }

        return Symbol.GRAPE;
    }

    public List<Symbol> RollResult(int reelCount = 3)
    {
        List<Symbol> result = new List<Symbol>();
        for (int i = 0; i < reelCount; i++)
        {
            result.Add(RollSymbol());
        }
        return result;
    }

    public int CalculateReward(List<Symbol> values)
    {
        HashSet<Symbol> uniqueSymbols = new HashSet<Symbol>(values);
        if (uniqueSymbols.Count == 1)
        {
            return PAYOUTS[values[0]];
        }
        if (values[0] == values[1] || values[1] == values[2])
        {
            return Mathf.RoundToInt(PAYOUTS[values[1]] / 2f);
        }
        return 0;
    }

    public int GetOffset(Symbol symbol)
    {
        return OFFSET[symbol];
    }
}

