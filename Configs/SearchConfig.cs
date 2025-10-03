using System.Collections.Generic;
using System.IO;
using System.Text.Json;
 
namespace TheConnectedShop.Configs
{
    public class SearchConfig
    {
        public List<string> SearchQueries { get; set; } = new List<string>();
 
        public static SearchConfig LoadFromFile(string path)
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<SearchConfig>(json)
                   ?? new SearchConfig();
        }
    }
}
