using System.Text.Json;

namespace FilterEC2InstancesByTag
{
    public static class FilterEC2InstancesByTag
    {
        private static readonly string FilePath = @"C:\Users\Yse\Downloads\response.json";

        private static List<string> FilterEC2InstanceIDsByTag(Dictionary<string, string> tags)
        {
            List<string> result = new();

            string response = File.ReadAllText(FilePath);
            JsonDocument jsonDoc = JsonDocument.Parse(response)!;
            JsonElement reservations = jsonDoc.RootElement.GetProperty("Reservations")[0];
            JsonElement instances = reservations.GetProperty("Instances");

            foreach (JsonElement instance in instances.EnumerateArray())
            {
                int matchingTagsCount = 0;

                if (instance.TryGetProperty("Tags", out JsonElement instanceTags))
                {
                    foreach (JsonElement kvp in instanceTags.EnumerateArray())
                    {
                        if (tags.TryGetValue(kvp.GetProperty("Key").ToString(), out string? value))
                        {
                            if (kvp.GetProperty("Value").ToString() != value)
                            {
                                break;
                            }
                            else
                            {
                                matchingTagsCount++;
                            }
                        }
                    }
                }
                if (matchingTagsCount == tags.Count)
                {
                    result.Add(instance.GetProperty("InstanceId").ToString());
                }
            }

            return result;
        }

        public static void Main()
        {
            Dictionary<string, string> tags = new()
            {
                { "Department", "security" },
                { "Environment", "staging" }
            };

            List<string> instanceIDs = FilterEC2InstanceIDsByTag(tags);

            foreach (string instanceID in instanceIDs)
            {
                Console.WriteLine(instanceID);
            }
        }
    }
}
