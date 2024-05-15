using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Xakpc.TelegramFileServer.Services;

public interface IStorageService
{
    Task<Document> UploadImageAsync(TelegramCredentials credentials, IFormFile image, CancellationToken cancellationToken);
    Task<string?> GetImageUrlAsync(TelegramCredentials credentials, string fileId);
}

public record TelegramCredentials(string BotToken, string? ChatId);

public class TelegramStorageService : IStorageService
{
    private IMemoryCache _memoryCache;

    public TelegramStorageService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task<Document> UploadImageAsync(TelegramCredentials credentials, IFormFile image, CancellationToken cancellationToken)
    {
        var botClient = new TelegramBotClient(credentials.BotToken);

        await using var imageStream = image.OpenReadStream();

        // Send the photo using the bot client
        var message = await botClient.SendDocumentAsync(
            chatId: credentials.ChatId,
            document: InputFile.FromStream(imageStream, image.FileName),
            disableNotification: true,           
            cancellationToken: cancellationToken);

        return message.Document;
    }

    public async Task<string?> GetImageUrlAsync(TelegramCredentials credentials, string fileId)
    {
        var metadata = await GetMetadataAsync(credentials, fileId);

        if (metadata == null)
        {
            return null;
        }

        return $"https://api.telegram.org/file/bot{credentials.BotToken}/{metadata.FilePath}";
    }

    private async Task<Telegram.Bot.Types.File?> GetMetadataAsync(TelegramCredentials credentials, string fileId)
    {
        var botClient = new TelegramBotClient(credentials.BotToken);

        // add memory cache to avoid multiple requests to the Telegram API
        if (_memoryCache.TryGetValue(fileId, out Telegram.Bot.Types.File file))
        {
            return file;
        }

        var fileInfo = await botClient.GetFileAsync(fileId);
        
        if (fileInfo != null)
        {
            _memoryCache.Set(fileId, fileInfo, TimeSpan.FromHours(1));
        }
        
        return fileInfo;
    }
}
