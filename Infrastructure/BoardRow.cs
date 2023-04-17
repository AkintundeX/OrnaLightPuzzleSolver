using OneOf;
using OneOf.Types;
using Infrastructure.Errors;

namespace Infrastructure;

/// <summary>
/// Represents a single row of a puzzle board
/// </summary>
public sealed class BoardRow
{
    private int[] _columns { get; init; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="columns">The values in each column</param>
    public BoardRow(int[] columns)
    {
        ArgumentNullException.ThrowIfNull(columns, nameof(columns));
        _columns = columns;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="columns">The number of columns</param>
    /// <param name="value">The value in every column</param>
    public BoardRow(int columns, int value)
    {
        _columns = new int[columns];

        for (int i = 0; i < _columns.Length; i++)
        {
            _columns[i] = value;
        }
    }

    /// <summary>
    /// The number of columns
    /// </summary>
    public int Size => _columns.Length;

    public int this[int index]
    {
        get
        {
            if (index >= _columns.Length || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            return _columns[index];
        }
    }

    /// <summary>
    /// Inverts the value of a column as if it was clicked
    /// </summary>
    /// <param name="column">Which column we're in</param>
    /// <param name="wasThisRow">If this row is the row that was clicked</param>
    /// <returns></returns>
    public OneOf<None, InvalidColumn> Invert(int column, bool wasThisRow)
    {
        if (column < 0 || column >= _columns.Length)
            return new InvalidColumn();

        Swap(column);
        if (wasThisRow)
        {
            if (column > 0)
                Swap(column - 1);
            if (column < _columns.Length - 1) 
                Swap(column + 1);
        }
        return new None();
    }

    /// <summary>
    /// Determines if row is solved (all lights are out)
    /// </summary>
    /// <returns></returns>
    public OneOf<True, False> IsSolved()
    {
        foreach (var column in _columns)
        {
            if (column == 1)
                return new False();
        }
        return new True();
    }

    /// <summary>
    /// Inverts the value of a column
    /// </summary>
    /// <param name="column">The column to invert</param>
    private void Swap(int column)
    {
        _columns[column] = this[column] == 0 ? 1 : 0;
    }

    public override string ToString()
    {
        return string.Join(", ", _columns);
    }
}
