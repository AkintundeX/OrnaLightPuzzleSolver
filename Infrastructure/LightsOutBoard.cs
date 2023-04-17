using Infrastructure.Errors;
using OneOf;
using OneOf.Types;
using System.Text;

namespace Infrastructure;

/// <summary>
/// Representation of a board
/// </summary>
public sealed class LightsOutBoard
{
    private readonly int[] _validSizes = { 4, 5, 6 };
    private readonly bool _sqaureOnly = true;

    private BoardRow[] _rows { get; init; }

    /// <summary>
    /// </summary>
    /// <param name="rows">The rows on the board</param>
    public LightsOutBoard(BoardRow[] rows)
    {
        ArgumentNullException.ThrowIfNull(rows, nameof(rows));

        _rows = rows;
    }

    /// <summary>
    /// Validates that all rows have the same number of columns, that the size of each are valid, and if it is square.
    /// </summary>
    /// <returns></returns>
    public OneOf<True, False> IsValid()
    {
        if (!_validSizes.Contains(_rows.Length))
            return new False();

        foreach (var row in _rows)
        {
            var rowSize = row.Size;

            if (_sqaureOnly && row.Size != _rows.Length)
                return new False();

            if (!_validSizes.Contains(rowSize))
                return new False();
        }

        return new True();
    }

    /// <summary>
    /// The number of rows and columns on the board
    /// </summary>
    /// <returns></returns>
    public (int Rows, int Columns) Size()
    {
        return (_rows.Length, _rows.First().Size);
    }

    public int this[int row, int column]
    {
        get
        {
            if (row >= _rows.Length || row < 0)
                throw new ArgumentOutOfRangeException(nameof(row));
            return _rows[row][column];
        }
    }

    /// <summary>
    /// Simulates pressing a button and the side-effects it would create
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    public OneOf<None, InvalidRow, InvalidColumn> CreateNextState(int row, int column)
    {
        if (row >= _rows.Length || row < 0)
            return new InvalidRow();
        if (column >= _rows[row].Size || column < 0)
            return new InvalidColumn();

        _rows[row].Invert(column, true);
        if (row > 0)
            _rows[row-1].Invert(column, false);
        if (row  < _rows.Length - 1)
            _rows[row + 1].Invert(column, false);

        return new None();
    }

    /// <summary>
    /// Determines if all rows (and thus, the board) are solved
    /// </summary>
    /// <returns></returns>
    public OneOf<True, False> IsSolved()
    {
        foreach (var row in  _rows)
        {
            var solveResult = row.IsSolved();

            if (solveResult.IsT1)
                return new False();
        }

        return new True();
    }

    public override string ToString()
    {
        StringBuilder sb = new();

        foreach (var row in  _rows)
        {
            sb.Append($"|{row.ToString()}|{Environment.NewLine}");
        }

        return sb.ToString();
    }

    public LightsOutBoard? Copy()
    {
        var (rowSize, _) = Size();
        var boardRows = new BoardRow[rowSize];
        for (int j = 0; j < rowSize; j++)
        {
            var row = _rows[j];
            var rowList = new int[rowSize];
            for (int i = 0; i < row.Size; i++)
            {
                rowList[i] = row[i];
            }
            boardRows[j] = new BoardRow(rowList);
        }

        return new LightsOutBoard(boardRows);
    }
}
