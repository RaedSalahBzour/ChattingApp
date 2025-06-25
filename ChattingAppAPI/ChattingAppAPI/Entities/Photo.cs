using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ChattingAppAPI.Entities;
[Table("Photos")]
public class Photo
{
    public int Id { get; set; }
    public required string Url { get; set; }
    public bool IsMain { get; set; }
    public string? PublicId { get; set; }
    public int UserId { get; set; }
    [JsonIgnore]
    public AppUser User { get; set; } = null!;

}