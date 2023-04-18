namespace Solver;

/// <summary>
/// Provides the colors that a tile can be, along with collections to determine if a glyph is highlighted or not
/// If multiple colors are listed, the first one is always Orna (from my phone), the second is Aetheric
/// </summary>
public static class TileColors
{
    /// <summary>
    /// Color of a glyph when highlighted
    /// </summary>
    public static readonly RGB[] GlyphInteriorHighlights = new RGB[] { new(234, 255, 0), new(238, 254, 83) };
    /// <summary>
    /// Color of an exterior border when highlighted
    /// </summary>
    public static readonly RGB[] BorderHighlights = new RGB[] { new(252, 152, 56), new(238, 157, 78) };
    /// <summary>
    /// Color of a non-highlighted borders exterior
    /// </summary>
    public static readonly RGB[] BorderExteriors = new RGB[] { new(38, 38, 38) };
    /// <summary>
    /// Color of interior of a border (the gradient)
    /// </summary>
    /// 
    public static readonly RGB[] BorderInteriors = new RGB[] { new(62, 56, 38), new(61, 56, 40) };
    /// <summary>
    /// Color inside the tile
    /// </summary>
    public static readonly RGB[] TileInteriors = new RGB[] { new(86, 80, 58), new(85, 80, 60) };
    /// <summary>
    /// Color of a non-highlighted glyphs exterior
    /// </summary>
    public static readonly RGB[] GlyphExteriors = new RGB[] { new(62, 56, 38), new(61, 56, 40) };
    /// <summary>
    /// Color of a non-highlighted glyphs interior
    /// </summary>
    public static readonly RGB[] GlyphInteriors = new RGB[] { new(38, 38, 38) };
    /// <summary>
    /// Color of a glyphs corner highlights
    /// </summary>
    public static readonly RGB[] GlyphCornerHighlights = new RGB[] { new(184, 110, 40), new(174, 113, 55) };
    /// <summary>
    /// Color of a glyphs outer edge when highlighted
    /// </summary>
    public static readonly RGB[] GlyphExteriorHighlights = new RGB[] { new(252, 152, 56), new(238, 157, 78) };

    /// <summary>
    /// Compares a color to an array of possible colors
    /// </summary>
    /// <param name="colors"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    private static bool RoughlyEquals(RGB[] colors, RGB color)
    {
        return colors.Any(col => RGB.RoughlyEqual(col, color));
    }

    public static bool IsBorderHighlight(RGB color)
    {
        return RoughlyEquals(BorderHighlights, color);
    }

    public static bool IsBorderExterior(RGB color)
    {
        return RoughlyEquals(BorderExteriors, color);
    }

    public static bool IsBorderInterior(RGB color)
    {
        return RoughlyEquals(BorderInteriors, color);
    }

    public static bool IsTileInterior(RGB color)
    {
        return RoughlyEquals(TileInteriors, color);
    }

    public static bool IsGlyphExterior(RGB color)
    {
        return RoughlyEquals(GlyphExteriors, color);
    }

    public static bool IsGlyphInterior(RGB color)
    {
        return RoughlyEquals(GlyphInteriors, color);
    }

    public static bool IsGlyphCornerHighlight(RGB color)
    {
        return RoughlyEquals(GlyphCornerHighlights, color);
    }

    public static bool IsGlyphExteriorHighlight(RGB color)
    {
        return RoughlyEquals(GlyphExteriorHighlights, color);
    }

    public static bool IsGlyphInteriorHighlight(RGB color)
    {
        return RoughlyEquals(GlyphInteriorHighlights, color);
    }

    public static bool IsAnyGlyphColor(RGB color)
    {
        return IsBorderHighlight(color) || IsBorderExterior(color) || IsBorderInterior(color) ||
               IsTileInterior(color) || IsGlyphExterior(color) || IsGlyphInterior(color) ||
               IsGlyphCornerHighlight(color) || IsGlyphExteriorHighlight(color) || IsGlyphInteriorHighlight(color);
    }

    public static bool IsAnyHighlightColor(RGB rGB)
    {
        return IsGlyphCornerHighlight(rGB) || IsGlyphExteriorHighlight(rGB) || IsGlyphInteriorHighlight(rGB) || IsBorderHighlight(rGB);
    }
}
