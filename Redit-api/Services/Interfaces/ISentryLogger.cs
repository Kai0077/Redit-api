namespace Redit_api.Services.Interfaces
{
    public interface ISentryLogger
    {
        void Info(string action, string details = "");
        void Success(string action, string details = "");
        void Warn(string action, string details = "");
        void Error(string action, Exception exception);
    }
}