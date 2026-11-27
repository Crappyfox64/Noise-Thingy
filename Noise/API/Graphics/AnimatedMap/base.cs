using System;
using System.Collections.Generic;
using System.IO;

namespace CRPY.Graphics
{
    public class CABMP
    {
        public int Width { get; }
        public int Height { get; }

        // Total duration in seconds, e.g. 3.5
        public double Duration { get; set; } = 1.0;

        // Delay per frame in 1/100s - what GIF actually stores
        public int FrameDelay
        {
            get
            {
                if (Frames.Count == 0) return 10;
                return (int)((Duration / Frames.Count) * 100);
            }
        }

        public int FrameCount => Frames.Count;

        // Store each frame as raw RGB, not CBMP - so ToGIF is fast
        private List<byte[]> Frames = new List<byte[]>();

        public CABMP(int width, int height)
        {
            Width = width;
            Height = height;
        }

        // Anim += BMP
        public static CABMP operator +(CABMP anim, CBMP bmp)
        {
            anim.Add(bmp);
            return anim;
        }

        public void Add(CBMP bmp)
        {
            byte[] rgb = new byte[Width * Height * 3];
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    CColor c = bmp.From(x, y);
                    int idx = (y * Width + x) * 3;
                    rgb[idx + 0] = c.R;
                    rgb[idx + 1] = c.G;
                    rgb[idx + 2] = c.B;
                }
            }
            Frames.Add(rgb);
        }

        public void Clear() => Frames.Clear();

        // --- Export ---

        public byte[] ToGIF()
        {
            return GifWriter.EncodeAnimated(Width, Height, Frames, FrameDelay);
        }

        // Animated, full color, loads everywhere - USE THIS FOR ANIMATION
        public byte[] ToAPNG()
        {
            return ApngWriter.EncodeAnimated(Width, Height, Frames, FrameDelay);
        }

        // Single frame WebP - Android native, loads fine
        // NOTE: Android Bitmap.Compress cannot do animated WebP, so this is 1 frame only
        public byte[] ToWebP(int frameIndex = 0)
        {
#if ANDROID
            if (Frames.Count == 0) return new byte[0];
            if (frameIndex >= Frames.Count) frameIndex = 0;
            byte[] src = Frames[frameIndex];

            int[] pixels = new int[Width * Height];
            for (int i = 0; i < Width * Height; i++)
            {
                int r = src[i * 3 + 0];
                int g = src[i * 3 + 1];
                int b = src[i * 3 + 2];
                pixels[i] = (255 << 24) | (r << 16) | (g << 8) | b;
            }

            using var bitmap = Android.Graphics.Bitmap.CreateBitmap(pixels, Width, Height, Android.Graphics.Bitmap.Config.Argb8888);
            using var ms = new MemoryStream();
            bool ok = bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.WebpLossless, 100, ms);
            if (!ok)
                bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Webp, 100, ms);
            return ms.ToArray();
#else
            return ToBMP();
#endif
        }

        // Single frame BMP - was missing, your #else called it
        public byte[] ToBMP(int frameIndex = 0)
        {
            if (Frames.Count == 0) return new byte[0];
            if (frameIndex >= Frames.Count) frameIndex = 0;
            byte[] rgb = Frames[frameIndex];

            int rowSize = Width * 3;
            int padding = (4 - (rowSize % 4)) % 4;
            int pixelDataSize = (rowSize + padding) * Height;
            int fileSize = 54 + pixelDataSize;

            using (var ms = new MemoryStream())
            using (var w = new BinaryWriter(ms))
            {
                w.Write(new char[] { 'B', 'M' });
                w.Write(fileSize);
                w.Write(0);
                w.Write(54);
                w.Write(40);
                w.Write(Width);
                w.Write(Height);
                w.Write((short)1);
                w.Write((short)24);
                w.Write(0);
                w.Write(pixelDataSize);
                w.Write(0);
                w.Write(0);
                w.Write(0);
                w.Write(0);

                // BMP is bottom-up, BGR
                for (int y = Height - 1; y >= 0; y--)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        int idx = (y * Width + x) * 3;
                        w.Write(rgb[idx + 2]); // B
                        w.Write(rgb[idx + 1]); // G
                        w.Write(rgb[idx + 0]); // R
                    }
                    for (int p = 0; p < padding; p++) w.Write((byte)0);
                }
                return ms.ToArray();
            }
        }

        // Your own format - for later, so you can save/load .cabmp
        public byte[] ToCABMP()
        {
            using (var ms = new MemoryStream())
            using (var w = new BinaryWriter(ms))
            {
                w.Write(new char[] { 'C', 'A', 'B', 'M', 'P' });
                w.Write(Width);
                w.Write(Height);
                w.Write(Duration);
                w.Write(Frames.Count);
                foreach (var f in Frames) w.Write(f);
                return ms.ToArray();
            }
        }

        public static CABMP FromCABMP(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            using (var r = new BinaryReader(ms))
            {
                r.ReadChars(5); // CABMP
                int w = r.ReadInt32();
                int h = r.ReadInt32();
                double dur = r.ReadDouble();
                int count = r.ReadInt32();
                var anim = new CABMP(w, h) { Duration = dur };
                for (int i = 0; i < count; i++) anim.Frames.Add(r.ReadBytes(w * h * 3));
                return anim;
            }
        }
    }
}