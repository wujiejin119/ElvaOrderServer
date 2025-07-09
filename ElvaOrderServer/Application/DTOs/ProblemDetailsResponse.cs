using System.Text.Json.Serialization;

namespace ElvaOrderServer.Application.DTOs
{
    public class ProblemDetailsResponse
    {      
        [JsonPropertyName("type")]
        public string Type { get; set; } = "about:blank";

        [JsonPropertyName("title")]
        public string Title { get; set; } = "An error occurred";

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("errors")]
        public Dictionary<string, string[]>? Errors { get; set; }


        [JsonPropertyName("traceId")]
        public string? TraceId { get; set; }
    }
}