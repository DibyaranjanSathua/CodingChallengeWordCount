// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using System.Text;
using WordCount;

Argument<FileInfo> fileArgument = new(name: "file", description: "Input file path");
Option<bool> lineCountOption = new(name: "--line", description: "Count number of lines in the input file");
lineCountOption.AddAlias("-l");
Option<bool> wordCountOption = new(name: "--word", description: "Count number of words in the input file");
wordCountOption.AddAlias("-w");
Option<bool> charCountOption = new(name: "--char", description: "Count number of characters in the input file");
charCountOption.AddAlias("-m");
Option<bool> byteCountOption = new(name: "--byte", description: "Count number of bytes in the input file");
byteCountOption.AddAlias("-c");

RootCommand rootCommand = new("Word counter CLI");
rootCommand.AddArgument(fileArgument);
rootCommand.AddOption(lineCountOption);
rootCommand.AddOption(wordCountOption);
rootCommand.AddOption(charCountOption);
rootCommand.AddOption(byteCountOption);
rootCommand.SetHandler(WordCountHandler, fileArgument, lineCountOption, wordCountOption, charCountOption, byteCountOption);

if (Console.IsInputRedirected)
{
    string inputRedirectedTempFile = ProcessStandardInput();
    args = [..args, inputRedirectedTempFile];

}
await rootCommand.InvokeAsync(args);
return;

static void WordCountHandler(FileInfo? file, bool lineCount, bool wordCount, bool charCount, bool byteCount)
{
    if (file is null)
    {
        Console.WriteLine("Input file is not provided");
        return;
    }

    StringBuilder stringBuilder = new();
    FileStatsData fileStatsData = StreamStatistics.GetStats(file.OpenRead());
    if (lineCount)
    {
        stringBuilder.Append($"{fileStatsData.LineCount} ");
    }

    if (wordCount)
    {
        stringBuilder.Append($"{fileStatsData.WordCount} ");
    }

    if (charCount)
    {
        stringBuilder.Append($"{fileStatsData.CharCount} ");
    }

    if (byteCount)
    {
        stringBuilder.Append($"{fileStatsData.ByteCount} ");
    }

    if (!(lineCount || wordCount || charCount || byteCount))
    {
        stringBuilder.Append(
            $"Lines: {fileStatsData.LineCount} Words: {fileStatsData.WordCount} Characters: {fileStatsData.CharCount} " +
            $"Bytes: {fileStatsData.ByteCount}");
    }
    
    Console.WriteLine(stringBuilder.ToString());
}


static string ProcessStandardInput()
{
    string tempFile = Path.GetTempFileName();
    using StreamReader streamReader = new(Console.OpenStandardInput());
    using StreamWriter streamWriter = new(tempFile);
    streamWriter.Write(streamReader.ReadToEnd());
    return tempFile;
}