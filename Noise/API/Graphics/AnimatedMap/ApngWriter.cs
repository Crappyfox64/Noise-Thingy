using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace CRPY.Graphics
{
    public static class ApngWriter
    {
        public static byte[] EncodeAnimated(int width, int height, List<byte[]> frames, int frameDelay100th)
        {
            using var ms = new MemoryStream();
            using var w = new BinaryWriter(ms);

            // PNG signature
            w.Write(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });

            // IHDR
            WriteChunk(w, "IHDR", chunk =>
            {
                chunk.WriteInt32BE(width);
                chunk.WriteInt32BE(height);
                chunk.Write((byte)8); // bit depth
                chunk.Write((byte)2); // color type 2 = RGB
                chunk.Write((byte)0);
                chunk.Write((byte)0);
                chunk.Write((byte)0);
            });

            // acTL - animation control
            WriteChunk(w, "acTL", chunk =>
            {
                chunk.WriteInt32BE(frames.Count);
                chunk.WriteInt32BE(0); // 0 = loop forever
            });

            int seq = 0;
            // Delay: frameDelay is 1/100s, APNG wants num/den
            short delayNum = (short)frameDelay100th;
            short delayDen = 100;

            for (int f = 0; f < frames.Count; f++)
            {
                // fcTL
                WriteChunk(w, "fcTL", chunk =>
                {
                    chunk.WriteInt32BE(seq++);
                    chunk.WriteInt32BE(width);
                    chunk.WriteInt32BE(height);
                    chunk.WriteInt32BE(0); // x
                    chunk.WriteInt32BE(0); // y
                    chunk.WriteInt16BE(delayNum);
                    chunk.WriteInt16BE(delayDen);
                    chunk.Write((byte)0); // dispose = none
                    chunk.Write((byte)0); // blend = source
                });

                // Image data - raw RGB with filter 0 per row
                byte[] raw = new byte[height * (1 + width * 3)];
                for (int y = 0; y < height; y++)
                {
                    raw[y * (1 + width * 3)] = 0; // filter None
                    Buffer.BlockCopy(frames[f], y * width * 3, raw, y * (1 + width * 3) + 1, width * 3);
                }

                using var compMs = new MemoryStream();
                using (var zs = new ZLibStream(compMs, CompressionLevel.Optimal, true))
                    zs.Write(raw, 0, raw.Length);
                byte[] compressed = compMs.ToArray();

                if (f == 0)
                {
                    // First frame = IDAT
                    WriteChunk(w, "IDAT", chunk => chunk.Write(compressed));
                }
                else
                {
                    // Rest = fdAT
                    WriteChunk(w, "fdAT", chunk =>
                    {
                        chunk.WriteInt32BE(seq++);
                        chunk.Write(compressed);
                    });
                }
            }

            WriteChunk(w, "IEND", chunk => { });
            return ms.ToArray();
        }

        private static void WriteChunk(BinaryWriter w, string type, Action<ChunkWriter> writeData)
        {
            using var tmp = new MemoryStream();
            var cw = new ChunkWriter(tmp);
            writeData(cw);
            byte[] data = tmp.ToArray();
            w.WriteInt32BE(data.Length);
            byte[] typeBytes = Encoding.ASCII.GetBytes(type);
            w.Write(typeBytes);
            w.Write(data);
            // CRC = type + data
            uint crc = Crc32(typeBytes, data);
            w.WriteInt32BE((int)crc);
        }

        // CRC32 for PNG
        private static uint[] _table;
        private static uint Crc32(byte[] type, byte[] data)
        {
            if (_table == null)
            {
                _table = new uint[256];
                for (uint n = 0; n < 256; n++)
                {
                    uint c = n;
                    for (int k = 0; k < 8; k++)
                        c = (c & 1) != 0 ? 0xEDB88320 ^ (c >> 1) : c >> 1;
                    _table[n] = c;
                }
            }
            uint crc = 0xFFFFFFFF;
            foreach (byte b in type) crc = _table[(crc ^ b) & 0xFF] ^ (crc >> 8);
            foreach (byte b in data) crc = _table[(crc ^ b) & 0xFF] ^ (crc >> 8);
            return crc ^ 0xFFFFFFFF;
        }

        private class ChunkWriter : BinaryWriter
        {
            public ChunkWriter(Stream s) : base(s) { }
            public void WriteInt32BE(int v)
            {
                Write((byte)((v >> 24) & 0xFF));
                Write((byte)((v >> 16) & 0xFF));
                Write((byte)((v >> 8) & 0xFF));
                Write((byte)(v & 0xFF));
            }
            public void WriteInt16BE(short v)
            {
                Write((byte)((v >> 8) & 0xFF));
                Write((byte)(v & 0xFF));
            }
        }
    }

    internal static class BinaryWriterExt
    {
        public static void WriteInt32BE(this BinaryWriter w, int v)
        {
            w.Write((byte)((v >> 24) & 0xFF));
            w.Write((byte)((v >> 16) & 0xFF));
            w.Write((byte)((v >> 8) & 0xFF));
            w.Write((byte)(v & 0xFF));
        }
    }
}