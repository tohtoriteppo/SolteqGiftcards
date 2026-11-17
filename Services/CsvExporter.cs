using CsvHelper;
using System.Text.Json.Nodes;

public class CsvExporter
{
    public async Task ExportAsync(string jsonData, string filePath)
    {
        // Check inputs
        if (string.IsNullOrWhiteSpace(jsonData)) throw new ArgumentException("JSON data cannot be empty.", nameof(jsonData));
        if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("File path cannot be empty.", nameof(filePath));

        JsonNode rootNode = JsonNode.Parse(jsonData);

        if (rootNode is not JsonArray rootArray) throw new InvalidOperationException("Expected root JSON element to be an array.");
        if (rootArray.Count == 0) return; // nothing to export
        if (rootArray[0] is not JsonObject first) throw new InvalidOperationException("Expected JSON array elements to be objects.");

        // Write csv
        try
        {
            using var csv = CsvWriter.ToFile(filePath);

            var headers = first.Select(field => field.Key).ToArray();
            csv.WriteHeader(headers); 

            foreach (JsonObject item in rootArray)
            {
                foreach (KeyValuePair<string, JsonNode?> field in item)
                {
                    csv.WriteField(field.Value); // write each value
                }
                csv.EndRecord();
            }
        }
        catch(Exception ex)
        {
            throw new InvalidOperationException("Failed to write CSV", ex);
        }
    }
}