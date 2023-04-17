using Infrastructure;
using OneOf;
using OneOf.Types;

namespace Solver;

public static class FiveByFiveStrategy
{
    /// <summary>
    /// Five by five boards (and other odd boards I assume) have a distinct set of solvable states.
    /// Ex: if just the bottom left light needs to change, there is no solution.
    /// Again, a board must only have lights on the bottom for this to function.
    /// This will not tell you if it's unsolvable or not, it just checks for the valid states and performs the moves
    /// </summary>
    /// <param name="board"></param>
    /// <param name="solution"></param>
    /// <returns></returns>
    public static OneOf<None> SetUpChaseDown(LightsOutBoard board, Solution solution)
    {
        bool firstLight = board[4, 0] == 1;
        bool secondLight = board[4, 1] == 1;
        bool thirdLight = board[4, 2] == 1;
        bool fourthLight = board[4, 3] == 1;
        bool fifthLight = board[4, 4] == 1;

        if (firstLight)
        {
            if (secondLight)
            {
                if (thirdLight)
                {
                    solution.AddInteraction(0, 1);
                    board.CreateNextState(0, 1);
                }
                else if (fourthLight && fifthLight)
                {
                    solution.AddInteraction(0, 2);
                    board.CreateNextState(0, 2);
                }

            }
            else if (thirdLight && fourthLight)
            {
                solution.AddInteraction(0, 4);
                board.CreateNextState(0, 4);
            }
            else if (fifthLight)
            {
                solution.AddInteraction(0, 0);
                board.CreateNextState(0, 0);
                solution.AddInteraction(0, 1);
                board.CreateNextState(0, 1);
            }
        }
        else if (secondLight)
        {
            if (thirdLight && fifthLight)
            {
                solution.AddInteraction(0, 0);
                board.CreateNextState(0, 0);
            }
            else if (fourthLight)
            {
                solution.AddInteraction(0, 1);
                board.CreateNextState(0, 1);
                solution.AddInteraction(0, 3);
                board.CreateNextState(0, 3);
            }
        }
        else if (thirdLight && fourthLight && fifthLight)
        {
            solution.AddInteraction(0, 3);
            board.CreateNextState(0, 3);
        }

        return new None();
    }
}

