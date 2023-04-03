using System.Diagnostics;
using System.Text;

internal class Program
{
    private static string FilePath = @"C:\Program Files (x86)\Steam\steamapps\common\Mad Tracks\Data\data2.bin";
    private static string OutDir = @"C:\Program Files (x86)\Steam\steamapps\common\Mad Tracks\Data\data2";

    private static void Main(string[] args)
    {
        Span<byte> data = File.ReadAllBytes(FilePath).AsSpan();

        if (data[0] == 0xAA && data[1] == 0xCC && data[2] == 0xBB && data[3] == 0xAA) {

            int i = BitConverter.ToInt32(data[^4..]);
            int numOfFiles = BitConverter.ToInt32(data[i..(i + 4)]);
            i += 4;

            Console.WriteLine($"Number of files: {numOfFiles}");
            Console.WriteLine();

            int n = 0;
            while (i < data.Length - 4)
            {
                long loc = BitConverter.ToInt64(data[i..(i + 8)]);
                i += 8;

                long len = BitConverter.ToInt64(data[i..(i + 8)]);
                i += 8;

                byte filePathLen = data[i++];
                string filePath = Encoding.UTF8.GetString(data[i..(i + filePathLen)]);
                i += filePathLen + 1;

                Console.WriteLine($"File {++n}:");
                Console.WriteLine($"File path: {filePath}");
                Console.WriteLine($"File path length: {filePathLen}");
                Console.WriteLine($"Location: {loc}");
                Console.WriteLine($"Data Length: {len}");
                Console.WriteLine($"Last byte: {loc + len}");

                int loc32 = (int)loc;
                int len32 = (int)len;
                string fileName = Path.GetFileName(filePath);

                Directory.CreateDirectory(Path.Combine(OutDir, string.Concat(filePath[..^fileName.Length])));
                File.WriteAllBytes($@"{OutDir}\{filePath}", data[loc32..(loc32 + len32)].ToArray());

                Console.WriteLine();
            }

            Process.Start($"explorer", OutDir);
        }
    }
}