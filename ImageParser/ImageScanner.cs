using Emgu.CV;
using Emgu.CV.Structure;
using Infrastructure;
using OneOf;
using Solver;

namespace ImageParser;


/// <summary>
/// Scans an image to interpret the board state on display
/// </summary>
/// <remarks>
/// Can't rely on fixed values due to DPI/resolution scaling, but they are usable for the minimum gap
/// Not all glyphs are centered, so this will just be an ugly process
/// </remarks>
public class ImageScanner
{
    private readonly Image<Bgr, byte> _image;
    private readonly int _startAtRow;
    private readonly int _startAtCol;
    private readonly int _endAtRow;
    private readonly int _endAtCol;
    private const int INCORRECT_COLOR_THRESHOLD = 2;
    private const int VALIDATION_REQUIREMENT = 10;

    private const int PIXEL_STEP = 2;
    private const int MAXIMUM_BOARD_SIZE = 6;
    // Part of the top/bottom of the screen is reserved for the player menu and controls such as chat/friends + zone name. This stuff can be skipped to reduce the potential amount of processing time. I don't care that Zipper plays in 320x240, I'm just gonna give him(?) an error
    private const double VERTICAL_RESERVED_SIZE = 0.1d;
    private const double HORIZONTAL_RESERVED_SIZE = 0.05d;

    public ImageScanner(Image<Bgr, byte> image)
    {
        ArgumentNullException.ThrowIfNull(image);

        _image = image;

        var width = _image.Width;
        var height = _image.Height;
        _startAtRow = 20;// (int)(height * VERTICAL_RESERVED_SIZE);
        _endAtRow = height - _startAtRow - 1;
        _startAtCol = 20; // (int)(width * HORIZONTAL_RESERVED_SIZE);
        _endAtCol = width - _startAtCol - 1;
    }

    /// <summary>
    /// Steps needed to achieve:
    /// 1 - Scan the image by column for the top 1/3rd of the screen - this should provide enough to vertically locate the top cell
    /// 2 - Record the state of this first item
    /// 3 - Scan horizontally until the next tile is encountered: We can use recurrences of non-tile colors to determine this. Scanning down instead of across means we will land on an edge of the tile that will always immediately lead inside the tile once we begin moving horizontall
    /// 4 - As new tiles are found horizontally, record their state
    /// 5 - Once the end of the image is reached (or if we find 6 tiles), record the number of tiles
    /// 6 - Also record the column index where the first piece of the tile (the outer edge) was encountered, so we do not need to continue scanning horizontally
    /// 7 - Scan vertically until the next tile is found from the X position of the first tile found - record this distance so we no longer need to scan vertically
    /// 8 - Iterate over all of the items using the deterministic locations/distances to populate the board state
    /// 9 - Generate the board state
    /// </summary>
    /// <returns></returns>
    public OneOf<LightsOutBoard, CouldNotParseBoard> ProcessImage()
    {
        // First, we purely need to worry about finding the first tile.
        int firstTileX = int.MaxValue, firstTileY = 0;
        // gotta go row by row unfortunately
        // Need to find several potential start points and then pick the small x/y

        bool firstTileFound = false;
        var yCoordinates = new List<int>();
        bool foundY = false;
        for (int row = 0; row < _endAtRow; row += PIXEL_STEP)
        {
            for (int column = 0; column < _endAtCol; column += PIXEL_STEP)
            {
                var pixel = _image[row, column];
                var pixelColor = new RGB((int)pixel.Red, (int)pixel.Green, (int)pixel.Blue);

                if (TileColors.IsTileInterior(pixelColor))
                {
                    firstTileFound = true;
                    const int _validation_count = 5;
                    for (int i = -_validation_count; i <= _validation_count; i++)
                    {
                        for (int j = -_validation_count; j <= _validation_count; j++)
                        {
                            var x = Math.Min(_image.Width - 1, Math.Max(0, column + (PIXEL_STEP * i)));
                            var y = Math.Min(_image.Height - 1, Math.Max(0, row + (PIXEL_STEP * j)));
                            var otherPixel = _image[y, x];
                            var otherPixelColor = new RGB(otherPixel.Red, otherPixel.Green, otherPixel.Blue);
                            firstTileFound &= TileColors.IsTileInterior(otherPixelColor);

                            if (!firstTileFound)
                                break;
                        }
                        if (!firstTileFound)
                            break;
                    }
                    if (firstTileFound)
                    {
                        firstTileX = Math.Min(firstTileX, column);
                        yCoordinates.Add(row);
                        firstTileY = Math.Max(firstTileY, row);
                    }
                }
            }
        }

        if (firstTileX == int.MaxValue || !yCoordinates.Any())
        {
            return new CouldNotParseBoard();
        }

        yCoordinates.Sort();
        int cutOff = 1;
        int samples = 0;

        for (; cutOff < yCoordinates.Count; cutOff++)
        {
            if (yCoordinates[cutOff] - yCoordinates[cutOff - 1] > PIXEL_STEP * 6)
            {
                if (samples > 10)
                {
                    break;
                }
            }
            else
            {
                samples++;
            }
        }

        var chosenYIdx = yCoordinates[(cutOff - 1) / 2];
        firstTileY = chosenYIdx;

        int gap = 0;
        bool insidePriorTile = true;
        int incorrectColorsEncountered = 0;
        // This will give an approximate distance between tiles horizontally. But based off the images I used, the vertical and horizontal gaps are equivalent. Until proven otherwise, the gap value will be shared but the methods will accept two in case this code needs amending
        for (int column = firstTileX; column < _endAtCol; column += PIXEL_STEP)
        {
            bool secondTileFound = false;
            var pixel = _image[firstTileY, column];
            var tileRgb = new RGB((int)pixel.Red, (int)pixel.Green, (int)pixel.Blue);
            if (insidePriorTile)
            {
                if (!TileColors.IsAnyGlyphColor(tileRgb))
                {
                    incorrectColorsEncountered++;
                }
                else
                {
                    incorrectColorsEncountered = 0; // We want them to occur directly in a row
                }

                if (incorrectColorsEncountered >= INCORRECT_COLOR_THRESHOLD)
                {
                    insidePriorTile = false;
                }
            }
            else
            {
                if (TileColors.IsTileInterior(tileRgb))
                {
                    secondTileFound = ValidateNextTilesAreInsideGlyph(column, firstTileY);

                    if (secondTileFound)
                    {
                        gap = column - firstTileX;
                        break;
                    }
                }
            }
        }

        if (gap == 0)
        {
            return new CouldNotParseBoard();
        }

        return PopulateBoard(firstTileX, firstTileY, gap, gap);
    }

    private bool ValidateNextTilesAreInsideGlyph(int column, int row, int steps = VALIDATION_REQUIREMENT)
    {
        var foundColors = new List<RGB>();
        for (int i = 1; i <= steps; i++)
        {
            var columnToCheck = Math.Min(column + (PIXEL_STEP * i), _image.Width - 1);
            // Since the color is found, sample a few pixels to validate
            var nextPixel = _image[row, columnToCheck];
            var nextRgb = new RGB((int)nextPixel.Red, (int)nextPixel.Green, (int)nextPixel.Blue);
            if (TileColors.IsAnyGlyphColor(nextRgb) && (!foundColors.Any() || foundColors.Last() != nextRgb))
            {
                foundColors.Add(nextRgb);
            }
            if (!TileColors.IsAnyGlyphColor(nextRgb))
            {
                return false;
            }
        }

        if (foundColors.Count == 1 && (TileColors.IsBorderExterior(foundColors.First()) || TileColors.IsBorderInterior(foundColors.First())))
        {
            return false;
        }
        else if (foundColors.Count == 2 && foundColors.Any(rgb => TileColors.IsBorderExterior(rgb)) && foundColors.Any(rgb => TileColors.IsBorderInterior(rgb)))
        {
            return false;
        }
        return foundColors.Any();
    }

    private LightsOutBoard PopulateBoard(int startX, int startY, int gapX, int gapY)
    {
        var rows = new List<BoardRow>(MAXIMUM_BOARD_SIZE);

        int glyphs = int.MaxValue;

        // Scan for the next row vertically, then populate it
        for (int row = startY; row < _endAtRow && rows.Count < glyphs; row += PIXEL_STEP)
        {
            // Allow a scan of up to the next 10% of the gap to see if we've coun
            var maxOffset = (int)(gapX * 0.1d);
            // The irregular shapes mean it is possible to miss a tile, so 
            for (int columnOffset = 0; columnOffset < maxOffset; columnOffset += PIXEL_STEP)
            {
                var x = startX + columnOffset;
                var gX = gapX - columnOffset;
                var valid = ValidateNextTilesAreInsideGlyph(x, row);

                if (valid)
                {
                    var rowToAdd = PopulateRow(x, row, gX);
                    if (rowToAdd.Size >= 4 && rowToAdd.Size <= 6)
                    {
                        rows.Add(rowToAdd);
                        row = startY + (gapY * rows.Count) - PIXEL_STEP;

                        if (glyphs == int.MaxValue)
                        {
                            glyphs = rowToAdd.Size;
                        }
                    }
                    break;
                }
            }
        }

        return new LightsOutBoard(rows.ToArray());
    }

    private BoardRow PopulateRow(int startX, int yPos, int gapX)
    {
        var tileStates = new List<int>(MAXIMUM_BOARD_SIZE);

        for (int column = startX; column < _endAtCol && (tileStates.Count < MAXIMUM_BOARD_SIZE); column += PIXEL_STEP)
        {
            // If we fell into a gap, the glyph shape didn't quite line up with the previous. This isn't an error.

            List<RGB> colorsFound = new();
            for (int columnOffset = 0; columnOffset < gapX; columnOffset += PIXEL_STEP)
            {
                if (column + columnOffset > _image.Width - 1)
                    break;
                
                var tilePixel = _image[yPos, column + columnOffset];
                var tileRgb = new RGB((int)tilePixel.Red, (int)tilePixel.Green, (int)tilePixel.Blue);

                if (!TileColors.IsAnyGlyphColor(tileRgb))
                {
                    break;
                }

                if (colorsFound.Count == 0 || !RGB.RoughlyEqual(tileRgb, colorsFound.Last()))
                {
                    colorsFound.Add(tileRgb);
                }
            }

            if (!colorsFound.Any())
            {
                continue;
            }

            int tileState = 1;

            var highlightColors = colorsFound.Any(rgb => TileColors.IsAnyHighlightColor(rgb));

            if (highlightColors)
            {
                int highlightsFound = 0;
                highlightsFound += colorsFound.Any(rgb => TileColors.IsBorderHighlight(rgb)) ? 1 : 0;
                highlightsFound += colorsFound.Any(rgb => TileColors.IsGlyphCornerHighlight(rgb)) ? 1 : 0;
                highlightsFound += colorsFound.Any(rgb => TileColors.IsGlyphExteriorHighlight(rgb)) ? 1 : 0;
                highlightsFound += colorsFound.Any(rgb => TileColors.IsGlyphInteriorHighlight(rgb)) ? 1 : 0;

                if (highlightsFound > 1)
                    tileState = 0;
            }

            tileStates.Add(tileState);
            column = startX + (gapX * tileStates.Count) - PIXEL_STEP;
        }

        return new BoardRow(tileStates.ToArray());
    }
}

public struct CouldNotParseBoard
{ }

public struct TileValidationResult
{
    public bool IsGlyph { get; init; }

    public bool IsOn { get; init; }

    public int X { get; init; }

    public int Y { get; init; }

    public TileValidationResult(bool isGlyph, bool isOn, int x, int y)
    {
        IsGlyph = isGlyph;
        IsOn = isOn;
        X = x; 
        Y = y;
    }
}