namespace Solver;

/// <summary>
/// Provides the colors that a tile can be, along with collections to determine if a glyph is highlighted or not
/// </summary>
public static class TileColors
{
    /// <summary>
    /// Color of an exterior border when highlighted
    /// </summary>
    public static readonly RGB BorderHighlight = new(252, 152, 56);
    /// <summary>
    /// Color of a non-highlighted borders exterior
    /// </summary>
    public static readonly RGB BorderExterior = new(38, 38, 38);
    /// <summary>
    /// Color of interior of a border (the gradient)
    /// </summary>
    public static readonly RGB BorderInterior = new(62, 56, 38);
    /// <summary>
    /// Color inside the tile
    /// </summary>
    public static readonly RGB TileInterior = new(86, 80, 58);
    /// <summary>
    /// Color of a non-highlighted glyphs exterior
    /// </summary>
    public static readonly RGB GlyphExterior = new(62, 56, 38);
    /// <summary>
    /// Color of a non-highlighted glyphs interior
    /// </summary>
    public static readonly RGB GlyphInterior = new(38, 38, 38);
    /// <summary>
    /// Color of a glyphs corner highlights
    /// </summary>
    public static readonly RGB GlyphCornerHighlight = new(184, 110, 40);
    /// <summary>
    /// Color of a glyphs outer edge when highlighted
    /// </summary>
    public static readonly RGB GlyphExteriorHighlight = new(252, 152, 56);
    /// <summary>
    /// Color of a glyph when highlighted
    /// </summary>
    public static readonly RGB GlyphInteriorHighlight = new(234, 255, 0);
    /// <summary>
    /// Collection of all the unique colors on a highlighted glyph
    /// </summary>
    public static readonly RGB[] Highlights = new RGB[] 
    { 
        BorderHighlight, 
        GlyphCornerHighlight, 
        GlyphExteriorHighlight, 
        GlyphInteriorHighlight 
    };
    /// <summary>
    /// Collection of the colors on a non-highlighted glyph
    /// </summary>
    public static readonly RGB[] Darks = new RGB[] 
    { 
        BorderExterior, 
        BorderInterior, 
        GlyphInterior, 
        GlyphExterior 
    };
    /// <summary>
    /// Collection of all of the colors a glyph can have
    /// This will power the ghetto solution of just scanning an image top to bottom to find all the glyphs
    /// </summary>
    public static readonly RGB[] AllGlyphColors = new RGB[] 
    { 
        BorderHighlight, 
        BorderExterior, 
        BorderInterior, 
        TileInterior, 
        GlyphExterior, 
        GlyphInterior, 
        GlyphCornerHighlight, 
        GlyphExteriorHighlight, 
        GlyphInteriorHighlight 
    };
    /// <summary>
    /// Every highlighted glyph will have at least one row with all of these colors in this order
    /// When examining a row, it is important to ensure that <see cref="GlyphCornerHighlight"/> does not interfere with the determination of if this is a valid, highlighted row.
    /// </summary>
    public static readonly RGB[] HighlightedRow = new RGB[]
    {
        BorderHighlight,
        BorderExterior,
        BorderInterior,
        TileInterior,
        GlyphExteriorHighlight,
        GlyphInteriorHighlight,
        GlyphCornerHighlight,
    };
    /// <summary>
    /// Every non-highlighted glyph will have at least one row with all of these colors in this order. Unlike highlighted rows, there will never be a surprise color here.
    /// </summary>
    public static readonly RGB[] Row = new RGB[]
    {
        BorderExterior,
        BorderInterior,
        TileInterior,
        GlyphExterior,
        GlyphInterior,
    };
}
