using System.Diagnostics;
using System.Text;

string dirName = "TestFiles\\";

await CalculationSpaceCountFromDir(dirName);
await CalculationSpaceCountFromThreeFiles();
async Task CalculationSpaceCountFromDir(string pathDir)
{
    {
        if (Directory.Exists(pathDir))
        {
            var stopwatch = Stopwatch.StartNew();
            var filePaths = Directory.GetFiles(pathDir);
            Console.WriteLine($"Количество файлов = {filePaths.Length}");

            if (!filePaths.Any())
            {
                throw new Exception($"В папке {pathDir} нет файлов");
            }
    
            var readTasks = new List<Task<(string, string)>>();
            foreach (string filePath in filePaths)
            {
                var readTask = Task.Run(() => GetFileTextAsync(filePath));
                readTasks.Add(readTask);
            }
    
            var readTasksResults = await Task.WhenAll(readTasks.ToArray());

            var calculationTasks = new List<Task>();
            foreach (var readTasksResult in readTasksResults)
            {
                var calculationTask = Task.Run(() => CalculationSpaceCountFromFile(readTasksResult.Item1, readTasksResult.Item2));
                calculationTasks.Add(calculationTask);
            }

            await Task.WhenAll(calculationTasks.ToArray());
    
            stopwatch.Stop();
            Console.WriteLine($"Операция заняла {stopwatch.ElapsedMilliseconds} миллисекунд");
        }
        else
        {
            throw new Exception($"Папки {pathDir} не существует");
        }
    }
}

async Task CalculationSpaceCountFromThreeFiles()
{
    var stopwatch = Stopwatch.StartNew();
    var filePaths = new []{"TestFiles\\1.txt", "TestFiles\\2.txt", "TestFiles\\3.txt"};
    var readTasks = new List<Task<(string, string)>>();
    foreach (string filePath in filePaths)
    {
        var readTask = Task.Run(() => GetFileTextAsync(filePath));
        readTasks.Add(readTask);
    }
    
    var readTasksResults = await Task.WhenAll(readTasks.ToArray());

    var calculationTasks = new List<Task>();
    foreach (var readTasksResult in readTasksResults)
    {
        var calculationTask = Task.Run(() => CalculationSpaceCountFromFile(readTasksResult.Item1, readTasksResult.Item2));
        calculationTasks.Add(calculationTask);
    }

    await Task.WhenAll(calculationTasks.ToArray());
    
    stopwatch.Stop();
    Console.WriteLine($"Операция заняла {stopwatch.ElapsedMilliseconds} миллисекунд");
}

async Task<(string, string)> GetFileTextAsync(string filePath)
{
    Console.WriteLine($"Начинаем получение текста из файла {filePath}");
    await using (FileStream fstream = File.OpenRead(filePath))
    {
        var buffer = new byte[fstream.Length];

        await fstream.ReadAsync(buffer, 0, buffer.Length);
            
        var textFromFile = Encoding.Default.GetString(buffer);
            
        return (filePath, textFromFile);
    }
}

void CalculationSpaceCountFromFile(string filePath, string textFromFile)
{
    var spaceCount = textFromFile.Count(c => c == ' ');
    
    Console.WriteLine($"Количество пробелов в файле {filePath} равно {spaceCount}.");
}