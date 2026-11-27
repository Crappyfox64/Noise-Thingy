using System;

namespace CRPY.Graphics;

public partial class CColor
{
    byte[] Data = new byte[4];
    public byte r
    {
        get => Data[0];
        set => Data[0] = value;
    }
    public byte g
    {
        get => Data[1];
        set => Data[1] = value;
    }
    public byte b
    {
        get => Data[2];
        set => Data[2] = value;
    }
    public byte a
    {
        get => Data[3];
        set => Data[3] = value;
    }

    public byte R
    {
        get => Data[0];
        set => Data[0] = value;
    }
    public byte G
    {
        get => Data[1];
        set => Data[1] = value;
    }
    public byte B
    {
        get => Data[2];
        set => Data[2] = value;
    }
    public byte A
    {
        get => Data[3];
        set => Data[3] = value;
    }

    public CColor(byte r=255, byte g=255, byte b=255, byte a=255)
    {
        Data[0] = r;
        Data[1] = g;
        Data[2] = b;
        Data[3] = a;
    }

    public CColor Lerp(CColor B, double t)
    {
        return new CColor(
            (byte)(r + (B.r - r) * t),
            (byte)(g + (B.g - g) * t),
            (byte)(b + (B.b - b) * t),
            (byte)(a + (B.a - a) * t)
        );
    }

    public CColor Inverse()
    {
        return new CColor((byte)(255 - r), (byte)(255 - g), (byte)(255 - b), a);
    }

    public byte[] ToByteArray()
    {
        return (byte[]) Data.Clone();
    }

    public (byte r, byte g, byte b, byte a) ToTuple()
    {
        return (r, g, b, a);
    }

    public static implicit operator CColor((byte r, byte g, byte b, byte a) tuple)
    {
        return new CColor(tuple.r, tuple.g, tuple.b, tuple.a);
    }

    public static implicit operator CColor(byte[] array)
    {
        if (array.Length != 4)
            throw new ArgumentException("Array must have exactly 4 elements.");
        return new CColor(array[0], array[1], array[2], array[3]);
    }

    public static CColor operator +(CColor A, CColor B)
    {
        int r = Math.Clamp(A.r + B.r,0,255);
        int g = Math.Clamp(A.g + B.g,0,255);
        int b = Math.Clamp(A.b + B.b,0,255);

        return new CColor((byte)r,(byte)g,(byte)b);
    }

    public static CColor operator -(CColor A, CColor B)
    {
        int r = Math.Clamp(A.r - B.r,0,255);
        int g = Math.Clamp(A.g - B.g,0,255);
        int b = Math.Clamp(A.b - B.b,0,255);

        return new CColor((byte)r,(byte)g,(byte)b);
    }

    public override string ToString()
    {
        return $"CColor({r}, {g}, {b}, {a})";
    }
}