using System;
using System.Collections.Generic;
using System.IO;

namespace CRPY.Graphics
{
    public static class GifWriter
    {
        public static byte[] EncodeAnimated(int width, int height, List<byte[]> frames, int frameDelay)
        {
            using var ms = new MemoryStream();
            using var w = new BinaryWriter(ms);

            // Header
            w.Write(new char[] { 'G', 'I', 'F', '8', '9', 'a' });
            w.Write((short)width);
            w.Write((short)height);
            w.Write((byte)0xF7); // Global Color Table, 256 colors, 8-bit
            w.Write((byte)0); // bg
            w.Write((byte)0); // aspect

            // Palette: 3-3-2 RGB = 256 colors
            // This is the same palette we use to quantize below
            for (int i = 0; i < 256; i++)
            {
                w.Write((byte)((i >> 5 & 7) * 255 / 7));
                w.Write((byte)((i >> 2 & 7) * 255 / 7));
                w.Write((byte)((i & 3) * 255 / 3));
            }

            // Looping
            if (frames.Count > 1)
            {
                w.Write((byte)0x21);
                w.Write((byte)0xFF);
                w.Write((byte)11);
                w.Write(new char[] { 'N', 'E', 'T', 'S', 'C', 'A', 'P', 'E', '2', '.', '0' });
                w.Write((byte)3);
                w.Write((byte)1);
                w.Write((short)0); // loop forever
                w.Write((byte)0);
            }

            foreach (var rgb in frames)
            {
                // RGB -> indices using 3-3-2 palette
                byte[] indices = new byte[width * height];
                for (int p = 0; p < width * height; p++)
                {
                    int r = rgb[p * 3 + 0] >> 5; // 0-7
                    int g = rgb[p * 3 + 1] >> 5; // 0-7
                    int b = rgb[p * 3 + 2] >> 6; // 0-3
                    indices[p] = (byte)((r << 5) | (g << 2) | b);
                }

                // Graphic Control - delay
                w.Write((byte)0x21);
                w.Write((byte)0xF9);
                w.Write((byte)4);
                w.Write((byte)0);
                w.Write((short)frameDelay);
                w.Write((byte)0);
                w.Write((byte)0);

                // Image Descriptor
                w.Write((byte)0x2C);
                w.Write((short)0);
                w.Write((short)0);
                w.Write((short)width);
                w.Write((short)height);
                w.Write((byte)0); // no local palette
                w.Write((byte)8); // LZW min code size = 8

                // LZW data - chunked into 255-byte blocks
                byte[] compressed = LzwEncodeUncompressed(indices);
                int offset = 0;
                while (offset < compressed.Length)
                {
                    int chunk = Math.Min(255, compressed.Length - offset);
                    w.Write((byte)chunk);
                    w.Write(compressed, offset, chunk);
                    offset += chunk;
                }
                w.Write((byte)0); // terminator
            }

            w.Write((byte)0x3B); // trailer
            return ms.ToArray();
        }

        // BULLETPROOF LZW: No dictionary, just literals.
        // We send Clear every 100 pixels so decoder's code size NEVER grows past 9.
        // File is bigger (~60KB per 240x240 frame) but it CANNOT desync.
        private static byte[] LzwEncodeUncompressed(byte[] indices)
        {
            const int CLEAR = 256;
            const int EOI = 257;

            var output = new List<byte>();
            int bitBuffer = 0;
            int bitCount = 0;

            void Write9(int code)
            {
                bitBuffer |= (code << bitCount);
                bitCount += 9;
                while (bitCount >= 8)
                {
                    output.Add((byte)(bitBuffer & 0xFF));
                    bitBuffer >>= 8;
                    bitCount -= 8;
                }
            }

            Write9(CLEAR);
            int sinceClear = 0;

            foreach (byte b in indices)
            {
                Write9(b);
                sinceClear++;

                if (sinceClear >= 100)
                {
                    Write9(CLEAR);
                    sinceClear = 0;
                }
            }

            Write9(EOI);
            if (bitCount > 0)
                output.Add((byte)(bitBuffer & 0xFF));

            return output.ToArray();
        }
    }
}