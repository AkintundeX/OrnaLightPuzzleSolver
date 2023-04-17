using System.Drawing;
using System.Runtime.CompilerServices;

namespace Solver;

/// <summary>
/// Represents a color and yes I am ignoring <see cref="Color"/>
/// </summary>
public struct RGB
{
    private const int MINIMUM_COLOR_VALUE = 0;
    private const int MAXIMUM_COLOR_VALUE = 255;
    private const int ALLOWED_VARIANCE = 1;
    public int Red { get; init; }
    public int Green { get; init; }
    public int Blue { get; init; }

    public RGB(int red, int green, int blue)
    {
        if (red < MINIMUM_COLOR_VALUE || red > MAXIMUM_COLOR_VALUE)
            throw new ArgumentOutOfRangeException(nameof(red));
        if (green < MINIMUM_COLOR_VALUE || green > MAXIMUM_COLOR_VALUE)
            throw new ArgumentOutOfRangeException(nameof(green));
        if (blue < MINIMUM_COLOR_VALUE || blue > MAXIMUM_COLOR_VALUE)
            throw new ArgumentOutOfRangeException(nameof(blue));

        Red = red;
        Green = green;
        Blue = blue;
    }

    public RGB(double red, double green, double blue)
        : this((int)red, (int)green, (int)blue) { }

    public override bool Equals(object? obj)
    {
        var rhs = obj as RGB?;

        if (!rhs.HasValue)
            return false;

        return GetHashCode() == rhs.Value.GetHashCode();
    }

    public override int GetHashCode()
    {
        return Red * 1_000_000 + Green * 1_000 + Blue;
    }

    public static bool operator ==(RGB left, RGB right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(RGB left, RGB right)
    {
        return !(left == right);
    }

    public static RGB FromColor(Color color)
    {
        return new RGB(color.R, color.G, color.B);
    }

    /// <summary>
    /// Why roughly, you may ask?
    /// Cuz. (Also, Color uses doubles and oh big scary standard maybe will rounding error me)
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    public static bool RoughlyEqual(RGB lhs, RGB rhs)
    {
        var rDiff = lhs.Red - rhs.Red;
        if (Math.Abs(rDiff) > ALLOWED_VARIANCE)
            return false;

        var gDiff = lhs.Green - rhs.Green;
        if (Math.Abs(gDiff) > ALLOWED_VARIANCE)
            return false;

        var bDiff = lhs.Blue - rhs.Blue;
        if (Math.Abs(bDiff) > ALLOWED_VARIANCE)
            return false;

        return true;
    }
    public override string ToString()
    {
        return $"Red: {Red} | Green: {Green} | Blue: {Blue}";
    }
}