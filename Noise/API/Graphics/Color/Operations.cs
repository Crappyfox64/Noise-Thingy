using System;

namespace CRPY.Graphics;

public partial class CColor
{
    public static CColor operator /(CColor A, CColor B)
    {
		int r = (int)Math.Clamp((double)A.R / (double)B.R,0,255);
        int g = (int)Math.Clamp((double)A.G / (double)B.G,0,255);
        int b = (int)Math.Clamp((double)A.B / (double)B.B,0,255);
        
        return new CColor((byte)r,(byte)g,(byte)b){a = A.a};
	}
    
    public static CColor operator /(CColor A, double B)
    {
		int r = (int)Math.Clamp((double)A.R / B,0,255);
        int g = (int)Math.Clamp((double)A.G / B,0,255);
        int b = (int)Math.Clamp((double)A.B / B,0,255);
        
        return new CColor((byte)r,(byte)g,(byte)b){a = A.a};
	}
    
    public static CColor operator +(CColor A, double B)
    {
		int r = (int)Math.Clamp((double)A.R + B,0,255);
        int g = (int)Math.Clamp((double)A.G + B,0,255);
        int b = (int)Math.Clamp((double)A.B + B,0,255);
        
        return new CColor((byte)r,(byte)g,(byte)b){a = A.a};
	}
    
    public static CColor operator -(CColor A, double B)
    {
		int r = (int)Math.Clamp((double)A.R - B,0,255);
        int g = (int)Math.Clamp((double)A.G - B,0,255);
        int b = (int)Math.Clamp((double)A.B - B,0,255);
        
        return new CColor((byte)r,(byte)g,(byte)b){a = A.a};
	}
    
    public static CColor operator *(CColor A, CColor B)
    {
		double r = (double) A.r/255.0;
        double g = (double) A.g/255.0;
        double b = (double) A.b/255.0;
        
        double R = (double) B.r/255.0;
        double G = (double) B.g/255.0;
        double N = (double) B.b/255.0;
        
        byte a = (byte)(Math.Clamp(R*r,0,255) * 255);
        byte n = (byte)(Math.Clamp(G*g,0,255) * 255);
        byte c = (byte)(Math.Clamp(N*b,0,255) * 255);
        
        
        return new CColor(a,n,c){a = A.a};
	}
    
    public static CColor operator *(CColor A, double B)
    {
		double r = (double) A.r/255.0;
        double g = (double) A.g/255.0;
        double b = (double) A.b/255.0;
        
        byte a = (byte)(Math.Clamp(B*r,0,255) * 255);
        byte n = (byte)(Math.Clamp(B*g,0,255) * 255);
        byte c = (byte)(Math.Clamp(B*b,0,255) * 255);
        
        
        return new CColor(a,n,c){A = A.a};
	}
}