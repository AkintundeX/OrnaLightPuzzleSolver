using Infrastructure.Errors;
using OneOf;
using OneOf.Types;
using Solver.Errors;
using System.Text;

namespace Solver;

/// <summary>
/// Represents a board solution as a collection of <see cref="bool"/> indicating if a square should be interacted with
/// </summary>
public sealed class Solution
{
    public bool[][] Interactions { get; init; }

    public Solution(int rows, int columns)
    {
        if (rows <= 0)
            throw new ArgumentOutOfRangeException(nameof(rows));
        if (columns <= 0)
            throw new ArgumentOutOfRangeException(nameof (columns));

        Interactions = new bool[rows][];
        for (int i = 0; i < rows; i++)
        {
            Interactions[i] = new bool[columns];
        }
    }

    /// <summary>
    /// Sets a location where an interaction is required
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    public OneOf<None, InvalidRow, InvalidColumn>  AddInteraction(int row, int column)
    {
        if (row >= Interactions.Length || row < 0)
            return new InvalidRow();
        if (column >= Interactions.Length || column < 0)
            return new InvalidColumn();

        Interactions[row][column] = !Interactions[row][column];
        return new None();
    }

    /// <summary>
    /// Creates a string representation of the solution matrix
    /// </summary>
    /// <returns></returns>
    public string[] GenerateSolutionString()
    {
        var pretty = new string[Interactions.Length];

        for (int i = 0; i < Interactions.Length; i++)
        {
            pretty[i] = $"|{string.Join(',', Interactions[i].Select(interaction => interaction ? 1 : 0))}|";
        }

        return pretty;
    }

    public override string ToString()
    {
        StringBuilder sb = new();

        foreach(var row in Interactions)
        {
            sb.Append($"|{string.Join(", ", row)}|{Environment.NewLine}");
        }

        return sb.ToString();
    }
}
