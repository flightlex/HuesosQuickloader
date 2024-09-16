using HuesosQuickloader.Models;

namespace HuesosQuickloader.Services.Config;

public interface IConfigService
{
    string GetPath();
    void Initialize();
    AppConfig Read();
    void Write(AppConfig config);
}
