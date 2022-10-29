//https://github.com/aaberdeen/cobsEnDeCodeForm/blob/master/cobsEnDeCodeForm/COBSCodec.cs

using System;
using System.Collections.Generic;
using System.Linq;

namespace OBSKeyBoard_SB00.Utils;

public class COBSCodec
{
    protected COBSCodec()
    {
    } // not instantiable

    /**
         * Returns the encoded representation of the given bytes.
         * Inefficient method, but easy to use and understand.
         *
         * @param src the bytes to encode
         * @return the encoded bytes.
         */
    public static byte[] encode(byte[] src)
    {
        var dest = new List<byte[]>(maxEncodedSize(src.Length));
        encode(src, 0, src.Length, dest);
        dest.TrimExcess();
        return dest.SelectMany(a => a).ToArray();
    }

    /**
     * Adds (appends) the encoded representation of the range
     * <code>src[from..to)</code>
     * to the given destination list.
     * 
     * @param src the bytes to encode
     * @param from the first byte to encode (inclusive)
     * @param to the last byte to encode (exclusive)
     * @param dest the destination list to append to
     */
    public static void encode(byte[] src, int from, int to, List<byte[]> dest)
    {
        checkRange(from, to, src);
        //dest.EnsureCapacity(dest.size() + maxEncodedSize(to - from)); // for performance ensure add() will never need to expand list
        var code = 1; // can't use unsigned byte arithmetic...
        var blockStart = -1;

        // find zero bytes
        while (from < to)
        {
            if (src[from] == 0)
            {
                finishBlock(code, src, blockStart, dest, from - blockStart);
                code = 1;
                blockStart = -1;
            }
            else
            {
                if (blockStart < 0) blockStart = from;
                code++;
                if (code == 0xFF)
                {
                    finishBlock(code, src, blockStart, dest, from - blockStart + 1);
                    code = 1;
                    blockStart = -1;
                }
            }

            from++;
        }

        finishBlock(code, src, blockStart, dest, from - blockStart);
    }

    private static void finishBlock(int code, byte[] src, int blockStart, List<byte[]> dest, int length)
    {
        var codeByteArray = new byte[1];
        codeByteArray[0] = (byte)code;

        dest.Add(codeByteArray); //code is the count to next 00 so add one byte array to list with code in

        if (blockStart >= 0)
        {
            var myByteArray = new byte[length];

            for (var i = 0; i < length; i++) myByteArray[i] = src[blockStart + i];
            dest.Add(myByteArray);
        }
    }

    /**
     * Returns the maximum amount of bytes an ecoding of
     * <code>size</code>
     * bytes takes in the worst case.
     */
    public static int maxEncodedSize(int size)
    {
        return size + 1 + size / 254;
    }

    /**
         * Returns the decoded representation of the given bytes.
         * Inefficient method, but easy to use and understand.
         *
         * @param src the bytes to decode
         * @return the decoded bytes.
         */
    public static byte[] decode(byte[] src) //throws IOException 
    {
        var dest = new List<byte[]>(src.Length);
        decode(src, 0, src.Length, dest);
        dest.TrimExcess();
        // return dest.asArray();
        return dest.SelectMany(a => a).ToArray();
    }

    /**
     * Adds (appends) the decoded representation of the range
     * <code>src[from..to)</code>
     * to the given destination list.
     * 
     * @param src the bytes to decode
     * @param from the first byte to decode (inclusive)
     * @param to the last byte to decode (exclusive)
     * @param dest the destination list to append to
     * @throws IOException if src data is corrupt (encoded erroneously)
     */
    public static void decode(byte[] src, int from, int to, List<byte[]> dest) //throws IOException 
    {
        checkRange(from, to, src);
        // dest.ensureCapacity(dest.size() + (to-from)); // for performance ensure add() will never need to expand list

        while (from < to)
        {
            var code = src[from++] & 0xFF;
            var len = code - 1;

            if (code == 0 || from + len > to) throw new Exception("Corrupt COBS encoded data - bug in remote encoder?");

            var myByteArray = new byte[len];

            for (var i = 0; i < len; i++) myByteArray[i] = src[from + i];
            dest.Add(myByteArray);


            from += len;
            if (code < 0xFF && from < to)
            {
                // unnecessary to write last zero (is implicit anyway)
                var zero = new byte[1];
                zero[0] = 0x00;
                dest.Add(zero);
            }
        }
    }

    /**
         * Checks if the given range is within the contained array's bounds.
         */
    private static void checkRange(int from, int to, byte[] arr)
    {
        if (from < 0 || from > to || to > arr.Length)
            throw new Exception("from: " + from + ", to: "
                                + to + ", size: " + arr.Length);
    }
}