
using System;

namespace CRPY.Numerics.Noise;

public class CNoise3D
{

    public int Seed { get; private set; }

    public float CellSize { get; set; } = 20f;

    public float Radius { get; set; } = 15f;

    public CNoise3D(int? Seed = null)
    {
        this.Seed = Seed ?? Random.Shared.Next();
    }

    private uint Hash(int X, int Y, int Z)
    {
        unchecked
        {
            uint H = (uint)Seed;

            H ^= (uint)X * 374761393u;
            H ^= (uint)Y * 668265263u;
            H ^= (uint)Z * 2246822519u;

            H = (H ^ (H >> 13)) * 1274126177u;

            return H ^ (H >> 16);
        }
    }

    private float Random01(int X, int Y, int Z, uint Offset)
    {
        uint H = Hash(X, Y, Z) + Offset;

        H ^= H >> 16;
        H *= 0x7FEB352Du;
        H ^= H >> 15;
        H *= 0x846CA68Bu;
        H ^= H >> 16;

        return (H & 0x00FFFFFF) / 16777215f;
    }

    private (float X, float Y, float Z, float R) GetPoint(
        int X, int Y, int Z)
    {
        float S = CellSize;

        return (
            (X + Random01(X, Y, Z, 0xA341316Cu)) * S,
            (Y + Random01(X, Y, Z, 0xC8013EA4u)) * S,
            (Z + Random01(X, Y, Z, 0xAD90777Du)) * S,
            Radius * (0.5f + Random01(X, Y, Z, 0x7E95761Eu))
        );
    }

    public double From(float X = 0, float Y = 0, float Z = 0)
    {
        if (CellSize <= 0)
            throw new InvalidOperationException("CellSize must be greater than zero.");

        if (Radius <= 0)
            throw new InvalidOperationException("Radius must be greater than zero.");

        int CX = (int)MathF.Floor(X / CellSize);
        int CY = (int)MathF.Floor(Y / CellSize);
        int CZ = (int)MathF.Floor(Z / CellSize);

        double Amount = 0;

        int Range = (int)Math.Ceiling((Radius * 1.5f) / CellSize) + 1;

        for (int i = -Range; i <= Range; i++)
        {
            for (int j = -Range; j <= Range; j++)
            {
                for (int k = -Range; k <= Range; k++)
                {
                    var P = GetPoint(CX + i, CY + j, CZ + k);

                    double DX = P.X - X;
                    double DY = P.Y - Y;
                    double DZ = P.Z - Z;

                    double D = Math.Sqrt(
                        DX * DX +
                        DY * DY +
                        DZ * DZ
                    );

                    if (D >= P.R)
                        continue;

                    double S = 1.0 - (D / P.R);

                    S = S * S * (3.0 - 2.0 * S);

                    Amount += S;
                }
            }
        }

        return Math.Clamp(Amount, 0.0, 1.0);
    }
}
