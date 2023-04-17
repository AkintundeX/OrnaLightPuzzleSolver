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
    private const int VALIDATION_REQUIREMENT = 40;

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
        _startAtRow = (int)(height * VERTICAL_RESERVED_SIZE);
        _endAtRow = height - _startAtRow;
        _startAtCol = (int)(width * HORIZONTAL_RESERVED_SIZE);
        _endAtCol = width - _startAtCol;
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
        // Don't know the dimensions yet so no array for you
        var boardRows = new List<List<int>>(MAXIMUM_BOARD_SIZE);

        // First, we purely need to worry about finding the first tile.
        int firstTileX = -1, firstTileY = -1;
        var extColor = TileColors.BorderExterior;
        var highlightcolor = TileColors.BorderHighlight;
        bool firstTileFound = false;

        for (int column = _startAtCol; column < _endAtCol && !firstTileFound; column += PIXEL_STEP)
        {
            for (int row = _startAtRow; row < _endAtCol && !firstTileFound; row += PIXEL_STEP)
            {
                var pixel = _image[row, column];
                var pixelColor = new RGB((int)pixel.Red, (int)pixel.Green, (int)pixel.Blue);

                if (RGB.RoughlyEqual(pixelColor, extColor) ||
                    RGB.RoughlyEqual(pixelColor, highlightcolor))
                {
                    firstTileFound = ValidateNextTilesAreInsideGlyph(column, row);
                    if (firstTileFound)
                    {
                        firstTileX = column;
                        firstTileY = row;
                    }
                }
            }
        }

        if (!firstTileFound)
        {
            return new CouldNotParseBoard();
        }

        int gap = 0;
        bool insidePriorTile = true;
        int incorrectColorsEncountered = 0;
        bool secondTileFound = false;
        // This will give an approximate distance between tiles horizontally. But based off the images I used, the vertical and horizontal gaps are equivalent. Until proven otherwise, the gap value will be shared but the methods will accept two in case this code needs amending
        for (int column = firstTileX; column < _endAtCol && !secondTileFound; column += PIXEL_STEP)
        {
            var pixel = _image[firstTileY, column];
            var tileRgb = new RGB((int)pixel.Red, (int)pixel.Green, (int)pixel.Blue);
            if (insidePriorTile)
            {
                if (!TileColors.AllGlyphColors.Any(rgb => RGB.RoughlyEqual(rgb, tileRgb)))
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
                if (RGB.RoughlyEqual(tileRgb, extColor) || RGB.RoughlyEqual(tileRgb, highlightcolor))
                {
                    secondTileFound = ValidateNextTilesAreInsideGlyph(column, firstTileY);

                    if (secondTileFound)
                    {
                        gap = column - firstTileX;
                    }
                }
            }
        }

        if (!secondTileFound)
        {
            return new CouldNotParseBoard();
        }

        return PopulateBoard(firstTileX, firstTileY, gap, gap);
    }

    private bool ValidateTile(int column, int row)
    {
        // At this point, we should be at the left side of the tile. We may not be at the corner, however.
        // First, lets scan across to find a row that includes the Exterior color, Interior Color, Tile Color, Glyph Exterior and Glyph Interior in a row.
        // We will have to scan down AND up, as we are not guaranteed to be at any specific corner.

        List<RGB> colorsFound = new();
        var firstPixelColor = _image[row, column];
        var firstPixelRgb = new RGB(firstPixelColor.Red, firstPixelColor.Green, firstPixelColor.Blue);
        bool firstPixelOn;

        if (RGB.RoughlyEqual(firstPixelRgb, TileColors.BorderExterior))
        {
            firstPixelOn = false;
        }
        else if (RGB.RoughlyEqual(firstPixelRgb, TileColors.GlyphExteriorHighlight))
        {
            firstPixelOn = true;
        }
        else { return false; }

        // 1000 is just a sanity check, this loop should always break well before then.
        for (int offset = 0; offset < 1000; offset += PIXEL_STEP)
        {
            var pixel = _image[row, column + offset];
            var tileRgb = new RGB(pixel.Red, pixel.Green, pixel.Blue);

            if (colorsFound.Count == 0 || colorsFound.Last() != tileRgb)
                colorsFound.Add(tileRgb);

            if (colorsFound.Count > 4 && RGB.RoughlyEqual(tileRgb, TileColors.GlyphInterior))
            {
                // To validate that this is a tile, we will check the previous set of colors.
                if (RGB.RoughlyEqual(colorsFound[colorsFound.Count - 1], TileColors.GlyphExterior) &&
                    RGB.RoughlyEqual(colorsFound[colorsFound.Count - 2], TileColors.TileInterior) &&
                    RGB.RoughlyEqual(colorsFound[colorsFound.Count - 3], TileColors.BorderInterior) &&
                    RGB.RoughlyEqual(colorsFound[colorsFound.Count - 4], TileColors.BorderExterior))
                {
                    // This is a valid horizontal row with all the colors in the order we want. If we're looking for a tile that is off.
                    if (firstPixelOn)
                        break;
                }
            }
        }

        return true;
    }

    private bool ValidateNextTilesAreInsideGlyph(int column, int row, int steps = VALIDATION_REQUIREMENT)
    {
        for (int i = 1; i <= steps; i++)
        {
            var columnToCheck = Math.Min(column + (PIXEL_STEP * i), _image.Width);
            // Since the color is found, sample a few pixels to validate
            var nextPixel = _image[row, columnToCheck];
            var nextRgb = new RGB((int)nextPixel.Red, (int)nextPixel.Green, (int)nextPixel.Blue);
            if (!TileColors.AllGlyphColors.Any(tileColor => RGB.RoughlyEqual(tileColor, nextRgb)))
            {
                return false;
            }
        }

        return true;
    }

    private bool ValidateNextTilesAreNotInsideGlyph(int column, int row, int steps = 4)
    {
        for (int i = 1; i <= steps; i++)
        {
            var columnToCheck = Math.Min(column + (PIXEL_STEP * i), _image.Width);
            // Since the color is found, sample a few pixels to validate
            var nextPixel = _image[row, columnToCheck];
            var nextRgb = new RGB((int)nextPixel.Red, (int)nextPixel.Green, (int)nextPixel.Blue);
            if (TileColors.AllGlyphColors.Any(tileColor => RGB.RoughlyEqual(tileColor, nextRgb)))
            {
                return false;
            }
        }

        return true;
    }

    private LightsOutBoard PopulateBoard(int startX, int startY, int gapX, int gapY)
    {
        var rows = new List<BoardRow>(MAXIMUM_BOARD_SIZE);

        // Scan for the next row vertically, then populate it
        for (int row = startY; row < _endAtRow; row += PIXEL_STEP)
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
                    rows.Add(PopulateRow(x, row, gX));
                    row = startY + (gapY * rows.Count) - PIXEL_STEP;
                    break;
                }
            }
        }

        return new LightsOutBoard(rows.ToArray());
    }

    private BoardRow PopulateRow(int startX, int yPos, int gapX)
    {
        var tileStates = new List<int>(MAXIMUM_BOARD_SIZE);
        bool insidePriorGlyph = false;
        // The base size of a tile is about 128x128, including their transparent space. The gap (at base size) appears to be 24 pixels, but gapX is the distance from the leftmost side of the first tile to the first occurance of the exterior/highlight of the next one, and their irregular shape means we can not rely on that value. 24 is about 19% of 128, so by cutting off ~10%, we guarantee that regardless of scaling, we are cleanly outside the first tile without having entered the next one. This threshold could likely be increased to 15%, or I could be a good programmer and just check the prior pixels once a void space is entered. 10% should guarantee that regardless of the scaling we stay outside the next glyph
        gapX = (int)(gapX * 0.9);

        for (int column = startX; column < _endAtCol && (tileStates.Count < MAXIMUM_BOARD_SIZE); column += PIXEL_STEP)
        {
            // If we fell into a gap, the glyph shape didn't quite line up with the previous. This isn't an error.
            var foundNextGlyph = ValidateNextTilesAreInsideGlyph(column, yPos);

            if (foundNextGlyph && !insidePriorGlyph)
            {
                List<RGB> colorsFound = new();
                for (int columnOffset = 0; columnOffset < gapX; columnOffset += PIXEL_STEP)
                {
                    var tilePixel = _image[yPos, column + columnOffset];
                    var tileRgb = new RGB((int)tilePixel.Red, (int)tilePixel.Green, (int)tilePixel.Blue);

                    if (colorsFound.Count == 0 || !RGB.RoughlyEqual(tileRgb, colorsFound.Last()))
                    {
                        colorsFound.Add(tileRgb);
                    }
                }

                int tileState = 1;

                // To determine the the tile state, we just need to see if some colors occurred consecutively
                // It could be ExtHighlight -> BorderExt || BorderInt
                // BorderInt -> ExtHighlight
                // BorderExt -> ExtHighlight
                for (int i = 0; i < colorsFound.Count - 1; i++)
                {
                    var nextRgb = colorsFound[i + 1];
                    if (RGB.RoughlyEqual(colorsFound[i], TileColors.GlyphExteriorHighlight))
                    {
                        if (RGB.RoughlyEqual(nextRgb, TileColors.BorderInterior) ||
                            RGB.RoughlyEqual(nextRgb, TileColors.BorderExterior))
                        {
                            tileState = 0;
                            break;
                        }
                    }
                    if (RGB.RoughlyEqual(colorsFound[i], TileColors.BorderInterior))
                    {
                        if (RGB.RoughlyEqual(nextRgb, TileColors.BorderHighlight))
                        {
                            tileState = 0;
                            break;
                        }
                    }
                    if (RGB.RoughlyEqual(colorsFound[i], TileColors.BorderExterior))
                    {
                        if (RGB.RoughlyEqual(nextRgb, TileColors.BorderHighlight))
                        {
                            tileState = 0;
                            break;
                        }
                    }
                }

                tileStates.Add(tileState);
                insidePriorGlyph = true;
            }
            else if (insidePriorGlyph)
            {
                var tilePixel = _image[yPos, column];
                var tileRgb = new RGB((int)tilePixel.Red, (int)tilePixel.Green, (int)tilePixel.Blue);
                insidePriorGlyph = TileColors.AllGlyphColors.Any(tileColor => 
                                        RGB.RoughlyEqual(tileColor, tileRgb)) &&
                                        ValidateNextTilesAreInsideGlyph(column, yPos, 4);
            }
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