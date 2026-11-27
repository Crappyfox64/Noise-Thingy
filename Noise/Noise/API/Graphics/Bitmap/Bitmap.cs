
namespace CRPY.Graphics;

public abstract class CBitmap
{

    public int Width { get; }
    public int Height { get; }

    protected byte[,,] Buffer { get; }

    public byte[] this[int x, int y]
    {
        get
        {
            return new byte[]
            {
                Buffer[x, y, 0],
                Buffer[x, y, 1],
                Buffer[x, y, 2],
                Buffer[x, y, 3]
            };
        }
    }
    
    public CColor From(int x, int y)
    {
		return new CColor(
			Buffer[x,y,0],
            Buffer[x,y,1],
            Buffer[x,y,2]
		);
	}

    public void Set(int X, int Y, byte[] Color)
    {
        if (Color.Length < 3) return;

        Buffer[X, Y, 0] = Color[0];
        Buffer[X, Y, 1] = Color[1];
        Buffer[X, Y, 2] = Color[2];
        Buffer[X, Y, 3] = Color.Length < 4 ? (byte)255 : Color[3];
    }

    public void Set(int X, int Y, (byte r, byte g, byte b, byte a) Color)
    {
        Buffer[X, Y, 0] = Color.r;
        Buffer[X, Y, 1] = Color.g;
        Buffer[X, Y, 2] = Color.b;
        Buffer[X, Y, 3] = Color.a;
    }

    public void Set(int X, int Y, (byte r, byte g, byte b) Color)
    {
        Buffer[X, Y, 0] = Color.r;
        Buffer[X, Y, 1] = Color.g;
        Buffer[X, Y, 2] = Color.b;
        Buffer[X, Y, 3] = 255;
    }

    public void Set(int X, int Y, CColor Color)
    {
        Buffer[X, Y, 0] = Color.r;
        Buffer[X, Y, 1] = Color.g;
        Buffer[X, Y, 2] = Color.b;
        Buffer[X, Y, 3] = Color.a;
    }

    public CBitmap(int width, int height)
    {
        if (width <= 0 || height <= 0) return;

        Width = width;
        Height = height;

        Buffer = new byte[width, height, 4];
    }
    
}
