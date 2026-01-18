using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");
var salesSummaryDir = Path.Combine(currentDirectory, "salesSummaryDir");
Directory.CreateDirectory(salesSummaryDir);
var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);
var salesFiles = FindFiles(storesDirectory);
var salesTotal = CalculateSalesTotal(salesFiles);

File.WriteAllLines(Path.Combine(salesSummaryDir, "SalesSummary.txt"), CalculateSalesSummaryText(salesFiles));
File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");

foreach (var file in salesFiles)
{
    Console.WriteLine(file);
}

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);
    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);
        if(extension == ".json")
        {
            salesFiles.Add(file);
        }
    }
    
    return salesFiles;
}
double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;

    foreach(var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
        salesTotal += data?.Total ?? 0;
    }

    return salesTotal;
}
List<string> CalculateSalesSummaryText(IEnumerable<string> salesFiles)
{
    List<string> fileText = new List<string>();
    fileText.Add("Sales Summary\n-----------------------------");
    fileText.Add($"  Total Sales: ${salesTotal.ToString("0.00")}\n\n  Details:");
    foreach(var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);
        SalesData? data = JsonConvert.DeserializeObject<SalesData>(salesJson);
        if (file.EndsWith("sales.json") && Directory.GetParent(file).Name != "stores")
        {
            var total = data?.Total ?? 0;
            fileText.Add($"    Store {Directory.GetParent(file).Name}: ${total.ToString("0.00")}");
        }
    }

    return fileText;
}
record SalesData (double Total);
