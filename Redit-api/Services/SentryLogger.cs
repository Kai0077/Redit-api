using Redit_api.Services.Interfaces;

namespace Redit_api.Services
{
    public class SentryLogger : ISentryLogger
    {
        private readonly IHub _hub;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SentryLogger(IHub hub, IHttpContextAccessor httpContextAccessor)
        {
            _hub = hub;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetRequestContext()
        {
            var http = _httpContextAccessor.HttpContext;
            if (http == null)
            {
                return "No HTTP context";
            }

            var user = http.User?.Identity?.Name ?? "Anonymous";
            var path = http.Request.Path.HasValue ? http.Request.Path.Value : "(unknown path)";

            return $"User: {user}, Path: {path}";
        }

        private void Log(SentryLevel level, string tag, string action, string details = "", Exception? exception = null)
        {
            var context = GetRequestContext();
            var message = $"[{tag}] {action} | {details} | {context}";

            if (exception != null)
            {
                _hub.CaptureException(exception);
            }

            _hub.CaptureMessage(message, level);
        }

        public void Info(string action, string details = "") =>
            Log(SentryLevel.Info, "INFO", action, details);

        public void Success(string action, string details = "") =>
            Log(SentryLevel.Info, "SUCCESS", action, details);

        public void Warn(string action, string details = "") =>
            Log(SentryLevel.Warning, action, details);

        public void Error(string action, Exception exception) =>
            Log(SentryLevel.Error, "ERROR", action, exception: exception);
    }
}