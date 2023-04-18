using Emgu.CV;
using Emgu.CV.Structure;
using ImageParser;
using Infrastructure;

namespace Tests.Parser;

public class ParserTests
{
    [Test]
    public void CanParseFourByFourImage()
    {
        var file = new Image<Bgr, byte>("./TestImages/FourByFourHighDpi.png");

        var parser = new ImageScanner(file);

        var processResult = parser.ProcessImage();

        processResult.Switch(board =>
        {
            var (rows, columns) = board.Size();

            Assert.That(rows, Is.EqualTo(4));
            Assert.That(columns, Is.EqualTo(4));

            var expectedRows = new BoardRow[4];
            expectedRows[0] = new BoardRow(new int[] { 0, 1, 1, 1 });
            expectedRows[1] = new BoardRow(new int[] { 1, 1, 1, 1 });
            expectedRows[2] = new BoardRow(new int[] { 1, 0, 1, 0 });
            expectedRows[3] = new BoardRow(new int[] { 0, 1, 0, 1 });

            var expectedBoard = new LightsOutBoard(expectedRows);

            for (int row = 0; row < 4; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    Assert.That(expectedBoard[row, column], Is.EqualTo(board[row, column]));
                }
            }
        },
        couldNotParse =>
        {
            Assert.Fail("Failed to parse image");
        });
    }
    [Test]
    public void CanParseFourByFourCroppedImage()
    {
        var file = new Image<Bgr, byte>("./TestImages/FourByFourCropped.png");

        var parser = new ImageScanner(file);

        var processResult = parser.ProcessImage();

        processResult.Switch(board =>
        {
            var (rows, columns) = board.Size();

            Assert.That(rows, Is.EqualTo(4));
            Assert.That(columns, Is.EqualTo(4));

            var expectedRows = new BoardRow[4];
            expectedRows[0] = new BoardRow(new int[] { 0, 1, 1, 1 });
            expectedRows[1] = new BoardRow(new int[] { 1, 1, 1, 1 });
            expectedRows[2] = new BoardRow(new int[] { 1, 0, 1, 0 });
            expectedRows[3] = new BoardRow(new int[] { 0, 1, 0, 1 });

            var expectedBoard = new LightsOutBoard(expectedRows);

            for (int row = 0; row < 4; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    Assert.That(expectedBoard[row, column], Is.EqualTo(board[row, column]));
                }
            }
        },
        couldNotParse =>
        {
            Assert.Fail("Failed to parse image");
        });
    }

    [Test]
    public void CanParseFiveByFiveImage()
    {
        var file = new Image<Bgr, byte>("./TestImages/FiveByFiveHighDpi.png");

        var parser = new ImageScanner(file);

        var processResult = parser.ProcessImage();

        processResult.Switch(board =>
        {
            var (rows, columns) = board.Size();

            Assert.That(rows, Is.EqualTo(5));
            Assert.That(columns, Is.EqualTo(5));

            var expectedRows = new BoardRow[5];
            expectedRows[0] = new BoardRow(new int[] { 0,1,1,0,1 });
            expectedRows[1] = new BoardRow(new int[] { 1,0,0,0,1 });
            expectedRows[2] = new BoardRow(new int[] { 1,0,1,1,1 });
            expectedRows[3] = new BoardRow(new int[] { 1,0,1,1,0 });
            expectedRows[4] = new BoardRow(new int[] { 0,0,0,1,0 });

            var expectedBoard = new LightsOutBoard(expectedRows);

            for (int row = 0; row < 5; row++)
            {
                for (int column = 0; column < 5; column++)
                {
                    Console.WriteLine($"{row} {column}");
                    Assert.That(expectedBoard[row, column], Is.EqualTo(board[row, column]), $"{row} {column} expected {expectedBoard[row,column]} actual {board[row,column]}");
                }
            }
        },
        couldNotParse =>
        {
            Assert.Fail("Failed to parse image");
        });
    }

    [Test]
    public void CanParseFiveByFiveCroppedImage()
    {
        var file = new Image<Bgr, byte>("./TestImages/FiveByFivecROPPED.png");

        var parser = new ImageScanner(file);

        var processResult = parser.ProcessImage();

        processResult.Switch(board =>
        {
            var (rows, columns) = board.Size();

            Assert.That(rows, Is.EqualTo(5));
            Assert.That(columns, Is.EqualTo(5));

            var expectedRows = new BoardRow[5];
            expectedRows[0] = new BoardRow(new int[] { 0, 1, 1, 0, 1 });
            expectedRows[1] = new BoardRow(new int[] { 1, 0, 0, 0, 1 });
            expectedRows[2] = new BoardRow(new int[] { 1, 0, 1, 1, 1 });
            expectedRows[3] = new BoardRow(new int[] { 1, 0, 1, 1, 0 });
            expectedRows[4] = new BoardRow(new int[] { 0, 0, 0, 1, 0 });

            var expectedBoard = new LightsOutBoard(expectedRows);

            for (int row = 0; row < 5; row++)
            {
                for (int column = 0; column < 5; column++)
                {
                    Console.WriteLine($"{row} {column}");
                    Assert.That(expectedBoard[row, column], Is.EqualTo(board[row, column]), $"{row} {column} expected {expectedBoard[row, column]} actual {board[row, column]}");
                }
            }
        },
        couldNotParse =>
        {
            Assert.Fail("Failed to parse image");
        });
    }

    [Test]
    public void CanParseFiveByFiveImageFromAetheric()
    {
        var file = new Image<Bgr, byte>("./TestImages/AethericFiveByFive.png");

        var parser = new ImageScanner(file);

        var processResult = parser.ProcessImage();

        processResult.Switch(board =>
        {
            var (rows, columns) = board.Size();

            Assert.That(rows, Is.EqualTo(5));
            Assert.That(columns, Is.EqualTo(5));

            var expectedRows = new BoardRow[5];
            expectedRows[0] = new BoardRow(new int[] { 1, 0, 1, 1, 0 });
            expectedRows[1] = new BoardRow(new int[] { 1, 1, 1, 0, 1 });
            expectedRows[2] = new BoardRow(new int[] { 0, 0, 0, 0, 0 });
            expectedRows[3] = new BoardRow(new int[] { 1, 1, 0, 0, 0 });
            expectedRows[4] = new BoardRow(new int[] { 0, 0, 0, 0, 0 });

            var expectedBoard = new LightsOutBoard(expectedRows);

            for (int row = 0; row < 5; row++)
            {
                for (int column = 0; column < 5; column++)
                {
                    Console.WriteLine($"{row} {column}");
                    Assert.That(expectedBoard[row, column], Is.EqualTo(board[row, column]));
                }
            }
        },
        couldNotParse =>
        {
            Assert.Fail("Failed to parse image");
        });
    }

    [Test]
    public void CanParseFiveByFiveCroppedImageFromAetheric()
    {
        var file = new Image<Bgr, byte>("./TestImages/AethericFiveByFiveCropped.png");

        var parser = new ImageScanner(file);

        var processResult = parser.ProcessImage();

        processResult.Switch(board =>
        {
            var (rows, columns) = board.Size();

            Assert.That(rows, Is.EqualTo(5));
            Assert.That(columns, Is.EqualTo(5));

            var expectedRows = new BoardRow[5];
            expectedRows[0] = new BoardRow(new int[] { 1, 0, 1, 1, 0 });
            expectedRows[1] = new BoardRow(new int[] { 1, 1, 1, 0, 1 });
            expectedRows[2] = new BoardRow(new int[] { 0, 0, 0, 0, 0 });
            expectedRows[3] = new BoardRow(new int[] { 1, 1, 0, 0, 0 });
            expectedRows[4] = new BoardRow(new int[] { 0, 0, 0, 0, 0 });

            var expectedBoard = new LightsOutBoard(expectedRows);

            for (int row = 0; row < 5; row++)
            {
                for (int column = 0; column < 5; column++)
                {
                    Console.WriteLine($"{row} {column}");
                    Assert.That(expectedBoard[row, column], Is.EqualTo(board[row, column]));
                }
            }
        },
        couldNotParse =>
        {
            Assert.Fail("Failed to parse image");
        });
    }

    [Test]
    public void CanParseSixBySixImage()
    {
        var file = new Image<Bgr, byte>("./TestImages/SixBySixHighDpi.png");

        var parser = new ImageScanner(file);

        var processResult = parser.ProcessImage();

        processResult.Switch(board =>
        {
            var (rows, columns) = board.Size();

            Assert.That(rows, Is.EqualTo(6));
            Assert.That(columns, Is.EqualTo(6));

            var expectedRows = new BoardRow[6];
            expectedRows[0] = new BoardRow(new int[] { 1,1,0,0,1,0 });
            expectedRows[1] = new BoardRow(new int[] { 0,0,1,0,1,1 });
            expectedRows[2] = new BoardRow(new int[] { 0,0,0,0,0,0 });
            expectedRows[3] = new BoardRow(new int[] { 0,1,1,0,0,1 });
            expectedRows[4] = new BoardRow(new int[] { 1,0,1,1,0,1 });
            expectedRows[5] = new BoardRow(new int[] { 0,0,0,1,0,1 });

            var expectedBoard = new LightsOutBoard(expectedRows);

            for (int row = 0; row < 6; row++)
            {
                for (int column = 0; column < 6; column++)
                {
                    Console.WriteLine($"{row} {column}");
                    Assert.That(expectedBoard[row, column], Is.EqualTo(board[row, column]));
                }
            }
        },
        couldNotParse =>
        {
            Assert.Fail("Failed to parse image");
        });
    }

    [Test]
    public void CanParseSixBySixCroppedImage()
    {
        var file = new Image<Bgr, byte>("./TestImages/SixBySixCropped.png");

        var parser = new ImageScanner(file);

        var processResult = parser.ProcessImage();

        processResult.Switch(board =>
        {
            var (rows, columns) = board.Size();

            Assert.That(rows, Is.EqualTo(6));
            Assert.That(columns, Is.EqualTo(6));

            var expectedRows = new BoardRow[6];
            expectedRows[0] = new BoardRow(new int[] { 1, 1, 0, 0, 1, 0 });
            expectedRows[1] = new BoardRow(new int[] { 0, 0, 1, 0, 1, 1 });
            expectedRows[2] = new BoardRow(new int[] { 0, 0, 0, 0, 0, 0 });
            expectedRows[3] = new BoardRow(new int[] { 0, 1, 1, 0, 0, 1 });
            expectedRows[4] = new BoardRow(new int[] { 1, 0, 1, 1, 0, 1 });
            expectedRows[5] = new BoardRow(new int[] { 0, 0, 0, 1, 0, 1 });

            var expectedBoard = new LightsOutBoard(expectedRows);

            for (int row = 0; row < 6; row++)
            {
                for (int column = 0; column < 6; column++)
                {
                    Console.WriteLine($"{row} {column}");
                    Assert.That(expectedBoard[row, column], Is.EqualTo(board[row, column]));
                }
            }
        },
        couldNotParse =>
        {
            Assert.Fail("Failed to parse image");
        });
    }
}