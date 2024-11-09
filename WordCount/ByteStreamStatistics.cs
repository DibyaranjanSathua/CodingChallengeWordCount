using System.Text;

namespace WordCount;

public static class ByteStreamStatistics
{
    public static FileStatsData GetStats(Stream stream)
    {
        long byteCount = 0;
        long charCount = 0;
        long wordCount = 0;
        long lineCount = 0;
        char previousChar = ' ';
        using BinaryReader reader = new(stream);
        while (reader.PeekChar() >= 0)
        {
            byte value = reader.ReadByte();
            byte[] bytes = GetCharBytes(reader, value);
            byteCount += bytes.Length;
            char ch = Encoding.UTF8.GetChars(bytes).First();
            charCount++;
            // 3 different flavours of end-of-line characters
            // Carriage Return (CR)  \r used in Mac
            // Line feed (LF) \n in Linux
            // \r\n is Windows
            if (ch is '\n' or '\r')
            {
                // Line count is already incremented at previous character step
                if (previousChar == '\r' && ch == '\n')
                {
                    continue;
                }
                lineCount++;
            }
            if (!char.IsWhiteSpace(previousChar) && char.IsWhiteSpace(ch))
            {
                wordCount++;
            }
        
            previousChar = ch;
        }
        
        // If the file ends without any whitespace
        if (!char.IsWhiteSpace(previousChar))
        {
            wordCount++;
        }
        return new FileStatsData(ByteCount: byteCount, CharCount: charCount, WordCount: wordCount, LineCount: lineCount);
    }

    /// <summary>
    /// The first 128 characters in the Unicode library are represented as one byte.
    /// Characters with a code larger than 127 are encoded with multiple bytes, which includes a leading byte and one
    /// or more continuation bytes.
    /// U+00 - U+7F (0XXX_XXXX): This is the standard 128-bit ASCII characters set.
    /// U+80 - U+07FF (110X_XXXX 10XX_XXXX): This is the second code space in UTF-8.
    /// U+0800 - U+FFFF (1110_XXXX 10XX_XXXX 10XX_XXXX): This is the third code space in UTF-8.
    /// U+010000 - U+10FFFF (1111_0XXX 10XX_XXXX 10XX_XXXX 10XX_XXXX): This is the fourth code
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private static byte[] GetCharBytes(BinaryReader reader, byte value)
    {
        // U+00 to U+7F
        if ((value & 0b10000000) == 0)
        {
            return [value];
        }

        int result = (value & 0b11110000) >> 4;
        int setBitCount = CountSetBits(result);
        byte[] bytes = reader.ReadBytes(setBitCount - 1);
        // Check all the bytes are continuation byte
        foreach (byte b in bytes)
        {
            if (!IsContinuationByte(b))
            {
                foreach (byte b1 in bytes)
                {
                    Console.WriteLine(b1);
                }
                throw new InvalidDataException("Number of continuation bytes are not correct.");
            }
        }

        return [value, ..bytes];
    }

    /// <summary>
    /// The continuation bytes have '10' in the high-order position.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static bool IsContinuationByte(byte value) => (value & 0b11000000) == 0b10000000;

    /// <summary>
    /// Count number of one on MSB.
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    private static int CountSetBits(int n)
    {
        int count = 0;
        while (n > 0)
        {
            count += n & 1;
            n >>= 1;
        }

        return count;
    }
}