namespace ChurchWeb.Infrastructure.Services;

public interface IYouTubeService
{
    /// <summary>
    /// YouTube URL에서 비디오 ID 추출
    /// </summary>
    string? ExtractVideoId(string url);

    /// <summary>
    /// YouTube 비디오 메타데이터 가져오기
    /// </summary>
    Task<YouTubeVideoMetadata?> GetVideoMetadataAsync(string videoId);
}

public class YouTubeVideoMetadata
{
    public string VideoId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public DateTime? PublishedAt { get; set; }
    public TimeSpan? Duration { get; set; }
    public long ViewCount { get; set; }
}
