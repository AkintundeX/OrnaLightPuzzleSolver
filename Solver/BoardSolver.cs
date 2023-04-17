using Infrastructure;
using Infrastructure.Errors;
using OneOf;
using OneOf.Types;
using Solver.Errors;

namespace Solver;

/// <summary>
/// Generates a board solution
/// </summary>
public class BoardSolver
{
    private readonly LightsOutBoard _board;
    private readonly Solution _solution;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="board">The board to solve</param>
    public BoardSolver(LightsOutBoard board)
    {
        _board = board;
        var (rows, columns) = _board.Size();
        _solution = new Solution(rows, columns);
    }

    /// <summary>
    /// Chases the lights down from the top of the board
    /// This will solve the trivial case and prep a 5x5/6x6 to be solved
    /// </summary>
    /// <param name="Board"></param>
    /// <returns></returns>
    private OneOf<None> ChaseDown()
    {
        for (int row = 1; row < _board.Size().Rows; row++)
        {
            for (int column = 0; column < _board.Size().Columns; column++)
            {
                if (_board[row - 1, column] == 1)
                {
                    _solution.AddInteraction(row, column);
                    _board.CreateNextState(row, column);
                }
            }
        }

        return new None();
    }

    /// <summary>
    /// For the more complicated boards, there is a setup for lighting back up the first row based off which items are on in the bottom row
    /// </summary>
    /// <param name="board"></param>
    /// <param name="solution"></param>
    /// <returns></returns>
    private OneOf<Solution> SetupChaseDown()
    {
        var (Rows, _) = _board.Size();

        OneOf<None> setupResult;

        if (Rows == 5)
        {
            setupResult = SetupFiveByFive();
        }
        else if (Rows == 6)
        {
            setupResult = SetupSixBySix();
        }
        else
        {
            throw new IndexOutOfRangeException($"Board must be 5x5 or 6x6 to require set up, but the size is {Rows}");
        }
        setupResult.Switch(_ => { });


        return _solution;
    }

    /// <summary>
    /// Determines the moves necessary to solve the bottom row of a 6x6
    /// </summary>
    /// <returns></returns>
    private OneOf<None> SetupSixBySix()
    {
        var interactionsResult = SixBySixStrategy.CalculateMoves(_board);

        interactionsResult.Switch(interactions =>
        {
            foreach (var interaction in interactions)
            {
                _solution.AddInteraction(0, interaction);
                _board.CreateNextState(0, interaction);
            }
        },
        invalidBoard =>
        {
            // This isn't generated yet
        });

        return new None();
    }

    private OneOf<None> SetupFiveByFive()
    {
        return FiveByFiveStrategy.SetUpChaseDown(_board, _solution);
    }

    /// <summary>
    /// Generates a solution for a board if the board is valid
    /// </summary>
    public OneOf<(Solution, LightsOutBoard), InvalidBoard, NullBoard, UnsolveableBoard> Solve()
    {
        if (_board is null)
            return new NullBoard();

        var validityResult = _board.IsValid();
        
        return validityResult.Match<OneOf<(Solution, LightsOutBoard), InvalidBoard, NullBoard, UnsolveableBoard>>(valid =>
        {
            var copy = _board.Copy();
            var (rows, columns) = _board.Size();
            ChaseDown();

            var solvedResult = _board.IsSolved();

            return solvedResult.Match<OneOf<(Solution, LightsOutBoard), InvalidBoard, NullBoard, UnsolveableBoard>>(solved =>
            {
                return (_solution, copy);
            },
                notSolved =>
                {
                    var setupResult = SetupChaseDown();
                    setupResult.Switch(solution => ChaseDown());

                    return _board.IsSolved()
                                  .Match<OneOf<(Solution, LightsOutBoard), InvalidBoard, NullBoard, UnsolveableBoard>>(
                                    solved => (_solution, copy), 
                                    notSolved => new UnsolveableBoard());
                });
        },  
        invalid => new InvalidBoard());
    }
}

