
namespace CRPY.Graphics;

public partial class CColor
{
    // [ P R I M A R Y ]

    public static CColor Red => new CColor(255, 0, 0);
    public static CColor Green => new CColor(0, 255, 0);
    public static CColor Blue => new CColor(0, 0, 255);

    // [ S E C O N D A R Y ]

    public static CColor Yellow => new CColor(255, 255, 0);
    public static CColor Cyan => new CColor(0, 255, 255);
    public static CColor Magenta => new CColor(255, 0, 255);

    // [ T E R T I A R Y ]

    public static CColor Orange => new CColor(255, 165, 0);
    public static CColor Chartreuse => new CColor(127, 255, 0);
    public static CColor SpringGreen => new CColor(0, 255, 127);
    public static CColor Azure => new CColor(0, 127, 255);
    public static CColor Violet => new CColor(127, 0, 255);
    public static CColor Rose => new CColor(255, 0, 127);

    // [ M O N O C H R O M E ]

    public static CColor White => new CColor(255, 255, 255);
    public static CColor Black => new CColor(0, 0, 0);
    public static CColor Gray => new CColor(128, 128, 128);
    public static CColor Silver => new CColor(192, 192, 192);
    public static CColor DimGray => new CColor(105, 105, 105);
    public static CColor LightGray => new CColor(211, 211, 211);
    public static CColor DarkGray => new CColor(64, 64, 64);

    // [ R E D S ]

    public static CColor Maroon => new CColor(128, 0, 0);
    public static CColor DarkRed => new CColor(139, 0, 0);
    public static CColor Crimson => new CColor(220, 20, 60);
    public static CColor IndianRed => new CColor(205, 92, 92);
    public static CColor Salmon => new CColor(250, 128, 114);
    public static CColor LightSalmon => new CColor(255, 160, 122);
    public static CColor Tomato => new CColor(255, 99, 71);
    public static CColor Coral => new CColor(255, 127, 80);
    public static CColor Firebrick => new CColor(178, 34, 34);

    // [ O R A N G E S ]

    public static CColor DarkOrange => new CColor(255, 140, 0);
    public static CColor OrangeRed => new CColor(255, 69, 0);
    public static CColor Gold => new CColor(255, 215, 0);
    public static CColor Goldenrod => new CColor(218, 165, 32);
    public static CColor Khaki => new CColor(240, 230, 140);
    public static CColor PeachPuff => new CColor(255, 218, 185);

    // [ Y E L L O W S ]

    public static CColor LightYellow => new CColor(255, 255, 224);
    public static CColor LemonChiffon => new CColor(255, 250, 205);
    public static CColor Moccasin => new CColor(255, 228, 181);
    public static CColor PapayaWhip => new CColor(255, 239, 213);

    // [ G R E E N S ]

    public static CColor Lime => new CColor(0, 255, 0);
    public static CColor LimeGreen => new CColor(50, 205, 50);
    public static CColor ForestGreen => new CColor(34, 139, 34);
    public static CColor DarkGreen => new CColor(0, 100, 0);
    public static CColor Olive => new CColor(128, 128, 0);
    public static CColor OliveDrab => new CColor(107, 142, 35);
    public static CColor SeaGreen => new CColor(46, 139, 87);
    public static CColor MediumSeaGreen => new CColor(60, 179, 113);
    public static CColor Mint => new CColor(152, 255, 152);
    public static CColor PaleGreen => new CColor(152, 251, 152);
    public static CColor DarkOliveGreen => new CColor(85, 107, 47);

    // [ B L U E S ]

    public static CColor Navy => new CColor(0, 0, 128);
    public static CColor MidnightBlue => new CColor(25, 25, 112);
    public static CColor RoyalBlue => new CColor(65, 105, 225);
    public static CColor DodgerBlue => new CColor(30, 144, 255);
    public static CColor CornflowerBlue => new CColor(100, 149, 237);
    public static CColor SkyBlue => new CColor(135, 206, 235);
    public static CColor DeepSkyBlue => new CColor(0, 191, 255);
    public static CColor LightBlue => new CColor(173, 216, 230);
    public static CColor SteelBlue => new CColor(70, 130, 180);
    public static CColor Teal => new CColor(0, 128, 128);
    public static CColor Turquoise => new CColor(64, 224, 208);

    // [ P U R P L E S ]

    public static CColor Purple => new CColor(128, 0, 128);
    public static CColor Indigo => new CColor(75, 0, 130);
    public static CColor DarkViolet => new CColor(148, 0, 211);
    public static CColor BlueViolet => new CColor(138, 43, 226);
    public static CColor Orchid => new CColor(218, 112, 214);
    public static CColor Plum => new CColor(221, 160, 221);
    public static CColor Lavender => new CColor(230, 230, 250);

    // [ P I N K S ]

    public static CColor Pink => new CColor(255, 192, 203);
    public static CColor HotPink => new CColor(255, 105, 180);
    public static CColor DeepPink => new CColor(255, 20, 147);
    public static CColor LightPink => new CColor(255, 182, 193);

    // [ B R O W N S ]

    public static CColor Brown => new CColor(165, 42, 42);
    public static CColor SaddleBrown => new CColor(139, 69, 19);
    public static CColor Sienna => new CColor(160, 82, 45);
    public static CColor Chocolate => new CColor(210, 105, 30);
    public static CColor Tan => new CColor(210, 180, 140);
    public static CColor Beige => new CColor(245, 245, 220);
    public static CColor Wheat => new CColor(245, 222, 179);
    public static CColor BurlyWood => new CColor(222, 184, 135);
    
    public static CColor Mono(byte Value) => new CColor(Value,Value,Value);
}
