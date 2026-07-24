using Google.Apis.Services;
using GoogleYouTube = Google.Apis.YouTube.v3;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using System.Xml;

namespace ChurchWeb.Infrastructure.Services;

public class ChurchYouTubeService : IYouTubeService
{
    private readonly string? _apiKey;
    private readonly IConfiguration _configuration;

    public ChurchYouTubeService(IConfiguration configuration)
    {
        _configuration = configuration;
        _apiKey = configuration["YouTube:ApiKey"];
    }

    public string? ExtractVideoId(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        // YouTube URL 패턴들
        // https://www.youtube.com/watch?v=VIDEO_ID
        // https://youtu.be/VIDEO_ID
        // https://www.youtube.com/embed/VIDEO_ID
        // https://www.youtube.com/v/VIDEO_ID

        var patterns = new[]
        {
            @"(?:youtube\.com\/watch\?v=|youtu\.be\/|youtube\.com\/embed\/|youtube\.com\/v\/)([a-zA-Z0-9_-]{11})",
            @"youtube\.com\/watch\?.*v=([a-zA-Z0-9_-]{11})"
        };

        foreach (var pattern in patterns)
        {
            var match = Regex.Match(url, pattern);
            if (match.Success && match.Groups.Count > 1)
            {
                return match.Groups[1].Value;
            }
        }

        // URL이 이미 비디오 ID인 경우 (11자 영숫자)
        if (Regex.IsMatch(url, @"^[a-zA-Z0-9_-]{11}$"))
        {
            return url;
        }

        return null;
    }

    public async Task<YouTubeVideoMetadata?> GetVideoMetadataAsync(string videoId)
    {
        if (string.IsNullOrWhiteSpace(videoId))
            return null;

        // API 키가 있으면 YouTube Data API 사용
        if (!string.IsNullOrWhiteSpace(_apiKey))
        {
            var apiResult = await GetVideoMetadataFromApiAsync(videoId);
            if (apiResult != null)
                return apiResult;
        }

        // API 키가 없거나 실패하면 oEmbed 사용 (키 불필요)
        return await GetVideoMetadataFromOEmbedAsync(videoId);
    }

    private async Task<YouTubeVideoMetadata?> GetVideoMetadataFromApiAsync(string videoId)
    {
        try
        {
            using var youtubeService = new GoogleYouTube.YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = _apiKey,
                ApplicationName = "ChurchWeb"
            });

            var request = youtubeService.Videos.List("snippet,contentDetails,statistics");
            request.Id = videoId;

            var response = await request.ExecuteAsync();

            if (response.Items == null || response.Items.Count == 0)
                return null;

            var video = response.Items[0];
            var snippet = video.Snippet;
            var contentDetails = video.ContentDetails;
            var statistics = video.Statistics;

            // ISO 8601 duration 파싱 (예: PT1H2M10S)
            TimeSpan? duration = null;
            if (!string.IsNullOrEmpty(contentDetails?.Duration))
            {
                try
                {
                    duration = XmlConvert.ToTimeSpan(contentDetails.Duration);
                }
                catch
                {
                    // Duration 파싱 실패 시 무시
                }
            }

            // 가장 높은 해상도의 썸네일 선택
            var thumbnailUrl = snippet.Thumbnails?.Maxres?.Url
                ?? snippet.Thumbnails?.High?.Url
                ?? snippet.Thumbnails?.Medium?.Url
                ?? snippet.Thumbnails?.Default__?.Url
                ?? string.Empty;

            return new YouTubeVideoMetadata
            {
                VideoId = videoId,
                Title = snippet.Title ?? string.Empty,
                Description = snippet.Description ?? string.Empty,
                ThumbnailUrl = thumbnailUrl,
                PublishedAt = snippet.PublishedAtDateTimeOffset?.DateTime,
                Duration = duration,
                ViewCount = (long)(statistics?.ViewCount ?? 0)
            };
        }
        catch (Exception)
        {
            // API 오류 발생 시 null 반환
            return null;
        }
    }

    private async Task<YouTubeVideoMetadata?> GetVideoMetadataFromOEmbedAsync(string videoId)
    {
        try
        {
            using var httpClient = new HttpClient();
            var url = $"https://www.youtube.com/oembed?url=https://www.youtube.com/watch?v={videoId}&format=json";

            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var data = System.Text.Json.JsonSerializer.Deserialize<OEmbedResponse>(json);

            if (data == null)
                return null;

            // oEmbed에서는 썸네일 URL을 직접 제공
            return new YouTubeVideoMetadata
            {
                VideoId = videoId,
                Title = data.Title ?? string.Empty,
                Description = data.AuthorName ?? string.Empty, // oEmbed는 description이 없어서 채널명 사용
                ThumbnailUrl = data.ThumbnailUrl ?? string.Empty,
                PublishedAt = null, // oEmbed는 발행일 제공 안함
                Duration = null, // oEmbed는 duration 제공 안함
                ViewCount = 0 // oEmbed는 조회수 제공 안함
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    private class OEmbedResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("title")]
        public string? Title { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("author_name")]
        public string? AuthorName { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("thumbnail_url")]
        public string? ThumbnailUrl { get; set; }
    }
}
