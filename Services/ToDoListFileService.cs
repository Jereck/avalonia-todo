using AvaloniaTodo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace AvaloniaTodo.Services;

public static class ToDoListFileService
{
    private static string _jsonFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TodoList", "MyToDoList.txt");

    public static async Task SaveFileToFileAsync(IEnumerable<ToDoItem> itemsToSave)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_jsonFileName)!);

        using (var fs = File.Create(_jsonFileName))
        {
            await JsonSerializer.SerializeAsync(fs, itemsToSave);
        }

    }

    public static async Task<IEnumerable<ToDoItem>?> LoadFromFileAsync()
    {
        try
        {
            using (var fs = File.OpenRead(_jsonFileName))
            {
                return await JsonSerializer.DeserializeAsync<IEnumerable<ToDoItem>>(fs);
            }
        }
        catch (Exception e) when (e is FileNotFoundException || e is DirectoryNotFoundException)
        {
            return null;
        }
    }
}
