using System.Net;
using System.Net.Http;

namespace DekapuSkillLauncher.Services;

public static class HttpRetryHelper
{
    public static async Task<T> ExecuteAsync<T>(
        Func<Task<T>> action,
        int maxAttempts = 5,
        CancellationToken cancellationToken = default)
    {
        for (int attempt = 0; ; attempt++)
        {
            try
            {
                return await action();
            }
            catch (Exception ex) when (attempt < maxAttempts - 1 && IsTransient(ex))
            {
                var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt)); // 1s, 2s, 4s, 8s
                await Task.Delay(delay, cancellationToken);
            }
        }
    }

    private static bool IsTransient(Exception ex) => ex switch
    {
        HttpRequestException { StatusCode: null } => true,                          // ネットワーク到達不可
        HttpRequestException { StatusCode: HttpStatusCode.TooManyRequests } => true, // 429
        HttpRequestException { StatusCode: >= HttpStatusCode.InternalServerError } => true, // 5xx
        HttpRequestException => false,                                               // その他4xx
        TaskCanceledException => true,                                               // タイムアウト
        OperationCanceledException => false,                                         // ユーザーキャンセル
        _ => false,
    };
}
