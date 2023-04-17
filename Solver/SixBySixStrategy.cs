using Infrastructure;
using Infrastructure.Errors;
using OneOf;

namespace Solver;

/// <summary>
/// Even boards have a symmetry that means there's no single-light invalid boards.
/// It further means that any board solution can be determined by a combination of moves that would solve each individual permutation
/// However, any move that is repeated an even number of times does not need to be clicked ever, as that just returns it back to the prior state
/// </summary>
public static class SixBySixStrategy
{
    /// <summary>
    /// For this to work, the board must be in a state where all lines, except the last, are solved
    /// </summary>
    public static OneOf<IEnumerable<int>, InvalidBoard> CalculateMoves(LightsOutBoard board)
    {

        // We want the lights that are OFF, which is indicated by a 1
        bool firstLight = board[5, 0] ==  1;
        bool secondLight = board[5, 1] == 1;
        bool thirdLight = board[5, 2] ==  1;
        bool fourthLight = board[5, 3] == 1;
        bool fifthLight = board[5, 4] ==  1;
        bool sixthLight = board[5, 5] ==  1;

        var items = new List<int>();

        if (firstLight)
        {
            AddOrRemove(items, 0, 2);
        }
        if (secondLight)
        {
            AddOrRemove(items, 3);
        }
        if (thirdLight)
        {
            AddOrRemove(items, 0, 4);
        }
        if (fourthLight)
        {
            AddOrRemove(items, 1, 5);
        }
        if (fifthLight)
        {
            AddOrRemove(items, 2);
        }
        if (sixthLight)
        {
            AddOrRemove(items, 3, 5);
        }

        return items;
    }

    private static List<int> AddOrRemove(List<int> items, params int[] additions)
    {
        foreach (var item in additions)
        {
            if (items.Contains(item))
                items.Remove(item);
            else
                items.Add(item);
        }
        return items;
    }
}

