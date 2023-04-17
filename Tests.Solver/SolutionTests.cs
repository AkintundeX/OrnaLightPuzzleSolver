using Infrastructure;
using Solver;

namespace Tests.Solver;

public class SolutionTests
{
    [Test]
    public void SolvesFiveByFive()
    {
        var rows = new BoardRow[5];

        rows[0] = new BoardRow(5, 0);
        rows[1] = new BoardRow(5, 0);
        rows[2] = new BoardRow(5, 0);
        rows[3] = new BoardRow(5, 0);
        rows[4] = new BoardRow(new int[] { 1, 1, 1, 0, 0 });

        var board = new LightsOutBoard(rows);
        var solver = new BoardSolver(board);

        var solverResult = solver.Solve();

        solverResult.Switch(solution =>
        {
            board.IsSolved().Switch(solved =>
            {

            },
            notSolved =>
            {
                Assert.Fail("The board isn't solved.");
            });
            Console.WriteLine(board.ToString());
            Console.WriteLine(solution.ToString());
        },
        invalidBoard => Assert.Fail("Board is definitely valid"),
        nullBoard => Assert.Fail("And also definitely not null"),
        unsolvable => Assert.Fail("This board has a solution"));
    }
    [Test]
    public void SolvesFiveByFive2()
    {
        var rows = new BoardRow[5];

        rows[0] = new BoardRow(new int[] { 0,1,1,0,1 });
        rows[1] = new BoardRow(new int[] { 1,0,0,0,1 });
        rows[2] = new BoardRow(new int[] { 1,0,1,1,1 });
        rows[3] = new BoardRow(new int[] { 1,0,1,1,0 });
        rows[4] = new BoardRow(new int[] { 0, 0, 0, 1, 0 });

        var board = new LightsOutBoard(rows);
        var solver = new BoardSolver(board);

        var solverResult = solver.Solve();

        solverResult.Switch(solution =>
        {
            board.IsSolved().Switch(solved =>
            {

            },
            notSolved =>
            {
                Assert.Fail("The board isn't solved.");
            });
            Console.WriteLine(board.ToString());
            Console.WriteLine(solution.ToString());
        },
        invalidBoard => Assert.Fail("Board is definitely valid"),
        nullBoard => Assert.Fail("And also definitely not null"),
        unsolvable => Assert.Fail("This board has a solution"));
    }

    [Test]
    public void CanSolveAnyFourByFour()
    {
        var rows = new BoardRow[4];
        rows[0] = new BoardRow(4, 0);
        rows[1] = new BoardRow(4, 0);
        rows[2] = new BoardRow(4, 0);

        int solutions = 0;
        for (int i = 0; i <= 1; i++)
        {
            for (int j = 0; j <= 1; j++)
            {
                for (int k = 0; k <= 1; k++)
                {
                    for (int l = 0; l <= 1; l++)
                    {
                        rows[3] = new BoardRow(new int[] { i, j, k, l });
                        solutions++;

                        var board = new LightsOutBoard(rows);
                        var solver = new BoardSolver(board);

                        solver.Solve().Switch(solution =>
                        {

                        },
                        invalidBoard =>
                        {
                            Assert.Fail("Board is not invalid");
                        },
                        nullBoard =>
                        {
                            Assert.Fail("Board is not null");
                        },
                        unsolveable =>
                        {
                            Assert.Fail("This board can be solved.");
                        });
                    }
                }
            }
        }

        Assert.That(solutions, Is.EqualTo(16));
    }

    /// <summary>
    /// There are states^quantity permutations of a board. The board can only be on/off, so 2^6 is 64.
    /// Even boards (well, at least 4x4 and 6x6) do not have unsolvable states, whereas odd boards can.
    /// </summary>
    [Test]
    public void CanSolveAnySixBySix()
    {
        var rows = new BoardRow[6];
        rows[0] = new BoardRow(6, 0);
        rows[1] = new BoardRow(6, 0);
        rows[2] = new BoardRow(6, 0);
        rows[3] = new BoardRow(6, 0);
        rows[4] = new BoardRow(6, 0);

        int solutions = 0;

        for (int i = 0; i <= 1; i++)
        {
            for (int j = 0; j <= 1; j++)
            {
                for (int k = 0; k <= 1; k++)
                {
                    for (int  l = 0; l <= 1; l++)
                    {
                        for (int m = 0; m <= 1;  m++)
                        {
                            for (int  n = 0; n <= 1; n++)
                            {
                                rows[5] = new BoardRow(new int[] { i, j, k, l, m, n });
                                solutions++;

                                var board = new LightsOutBoard(rows);
                                var solver = new BoardSolver(board);

                                solver.Solve().Switch(solution =>
                                {

                                },
                                invalidBoard => Assert.Fail("Board is not invalid"),
                                nullBoard => Assert.Fail("Board is not null"),
                                unsolvableBoard => Assert.Fail("All 6x6 boards are solvable."));
                            }
                        }
                    }
                }
            }
        }

        Assert.That(solutions, Is.EqualTo(64));
    }
}