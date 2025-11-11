using System.Numerics;
using Windows.Foundation;

namespace DataGraphControl.WinUI.Acceleration;

public readonly struct Quad
{
    public readonly Point Center;
    public readonly double HalfWidth;
    public readonly double HalfHeight;

    public readonly Point TopLeft; 
    public readonly Point TopRight;
    public readonly Point BottomLeft;
    public readonly Point BottomRight; 

    public Quad(Point center, double halfSize)
    {
        Center = center;
        HalfWidth = halfSize;
        HalfHeight = halfSize;
        TopLeft = new(center.X - halfSize, center.Y + halfSize);
        TopRight = new(center.X + halfSize, center.Y + halfSize);
        BottomLeft = new(center.X - halfSize, center.Y - halfSize);
        BottomRight = new(center.X + halfSize, center.Y - halfSize);

    }
    public Quad(double x, double y, double width, double height)
    {
        TopLeft = new Point(x, y);
        TopRight = new Point(x + width, y);
        BottomLeft = new Point(x, y + height);
        BottomRight = new Point(x + width, y + height);

        HalfWidth = width * 0.5;
        HalfHeight = height * 0.5;
        Center = new Point(x + HalfWidth, y + HalfHeight);
    }
    public Quad(Point pt, double width, double height) : this(pt.X, pt.Y, width, height) { }
    public Quad(Point tl, Point tr, Point bl, Point br)
    {
        TopLeft = tl;
        TopRight = tr;
        BottomLeft = bl;
        BottomRight = br;

        var width = tr.X - tl.X;
        var height = tr.Y - br.Y;
        HalfWidth = width * 0.5;
        HalfHeight = height * 0.5;
        Center = new Point(tl.X + HalfWidth, tl.Y + HalfHeight);
    }

    public bool ContainsPoint(Point pt)
    {
        return TopLeft.X <= pt.X && pt.X <= TopRight.X &&
               TopLeft.Y <= pt.Y && pt.Y <= BottomRight.Y;
    }

    public bool IntersectsQuad(Quad other)
    {
        var centers = new Vector4((float)Center.X, (float)Center.Y, (float)other.Center.X, (float)other.Center.Y);
        var hs = new Vector4((float)HalfWidth, (float)HalfHeight, (float)other.HalfWidth, (float)other.HalfHeight);

        var plus = centers + hs;
        var minus = centers - hs;

        return minus.X <= plus.Z && plus.X >= minus.Z &&
               minus.Y <= plus.W && plus.Y >= minus.W;
    }

    public bool ContainsQuad(Quad other)
    {
        return ContainsPoint(other.TopLeft) &&
               ContainsPoint(other.TopRight) &&
               ContainsPoint(other.BottomLeft) &&
               ContainsPoint(other.BottomRight);
    }

    public (Quad TopLeft, Quad TopRight, Quad BottomLeft, Quad BottomRight) GetSubdivisions()
    {
        var rx = TopLeft.X;
        var ry = TopLeft.Y;

        var tl = new Quad(rx, ry, HalfWidth, HalfHeight);
        var tr = new Quad(rx + HalfWidth, ry, HalfWidth, HalfHeight);
        var bl = new Quad(rx, ry + HalfHeight, HalfWidth, HalfHeight);
        var br = new Quad(rx + HalfWidth, ry + HalfHeight, HalfWidth, HalfHeight);

        return (tl, tr, bl, br);
    }

    public bool CanFitInSubdivisions(Quad target)
    {
        (var nw, var ne, var sw, var se) = target.GetSubdivisions();

        if (nw.ContainsQuad(this))
            return true;

        if (ne.ContainsQuad(this))
            return true;

        if (sw.ContainsQuad(this))
            return true;

        if (se.ContainsQuad(this))
            return true;

        return false;
    }
}
