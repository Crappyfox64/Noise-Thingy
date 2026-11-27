
using System;
using System.IO;
using System.Threading.Tasks;

namespace CRPY.Graphics;

public class CBMP : CBitmap
{
    public CBMP(int Width, int Height) : base(Width, Height)
    {
    }
    
    public CBMP Clone()
    {
		CBMP Output = new CBMP(Width,Height);
        byte[,,] T = this.Buffer;
        Parallel.For(0, Width, i=>{
			for ( int k = 0; k < Height; k++ )
            {
				CColor Col = new CColor(
					T[i,k,0],
                    T[i,k,1],
                    T[i,k,2]
				);
				Output.Set(i,k,Col);
            }
        });
		return Output;
	}

    public byte[] ToBMP()
    {
        const int FILE_HEADER_SIZE = 14;
        const int DIB_HEADER_SIZE = 108;

        const uint BI_BITFIELDS = 3;
        const uint SRGB = 0x73524742;

        int PixelSize = checked(Width * Height * 4);

        int Offset = FILE_HEADER_SIZE + DIB_HEADER_SIZE;
        int FileSize = checked(Offset + PixelSize);

        using MemoryStream MS = new MemoryStream(FileSize);
        using BinaryWriter BW = new BinaryWriter(MS);

        BW.Write((ushort)0x4D42);
        BW.Write((uint)FileSize);
        BW.Write((ushort)0);
        BW.Write((ushort)0);
        BW.Write((uint)Offset);

        BW.Write((uint)DIB_HEADER_SIZE);
        BW.Write(Width);
        BW.Write(Height);
        BW.Write((ushort)1);
        BW.Write((ushort)32);
        BW.Write(BI_BITFIELDS);
        BW.Write((uint)PixelSize);
        BW.Write(2835);
        BW.Write(2835);
        BW.Write((uint)0);
        BW.Write((uint)0);

        BW.Write((uint)0x00FF0000);
        BW.Write((uint)0x0000FF00);
        BW.Write((uint)0x000000FF);
        BW.Write((uint)0xFF000000);

        BW.Write(SRGB);
        
        for (int i = 0; i < 9; i++) BW.Write((uint)0);
        BW.Write((uint)0);
        BW.Write((uint)0);
        BW.Write((uint)0);

        for (int Y = Height - 1; Y >= 0; Y--)
        {
            for (int X = 0; X < Width; X++)
            {

                BW.Write(Buffer[X, Y, 2]);
                BW.Write(Buffer[X, Y, 1]);
                BW.Write(Buffer[X, Y, 0]);
                BW.Write(Buffer[X, Y, 3]);
            }
        }

        return MS.ToArray();
    }
}
