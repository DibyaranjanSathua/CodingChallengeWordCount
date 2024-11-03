namespace WordCount;

public static class StreamStatistics
{
    public static FileStatsData GetStats(Stream stream)
    {
        long byteCount = 0;
        long charCount = 0;
        long wordCount = 0;
        long lineCount = 0;
        char previousChar = ' ';
        using StreamReader reader = new(stream);
        while (reader.Peek() >= 0)
        {
            int charValue = reader.Read();
            char ch = (char)charValue;
            charCount++;
            byteCount += GetByteCount(charValue);
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
    /// U+00 - U+7F (decimal 0 - 127)(0XXX_XXXX): This is the standard 128-bit ASCII characters set.
    /// U+80 - U+07FF (decimal 128 - 2047) (110X_XXXX 10XX_XXXX): This is the second code space in UTF-8.
    /// U+0800 - U+FFFF (decimal 2048 - 65535)(1110_XXXX 10XX_XXXX 10XX_XXXX): This is the third code space in UTF-8.
    /// U+010000 - U+10FFFF (decimal 65536 - 1114111) (1111_0XXX 10XX_XXXX 10XX_XXXX 10XX_XXXX): This is the fourth code space in UTF-8.
    /// </summary>
    /// <param name="charValue"></param>
    /// <returns></returns>
    private static int GetByteCount(int charValue) => charValue switch
    {
        >= 65536 => 4,
        >= 2048 => 3,
        >= 128 => 2,
        _ => 1
    };
}