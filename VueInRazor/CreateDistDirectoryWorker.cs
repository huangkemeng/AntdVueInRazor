
using System.IO;

namespace VueInRazor
{
    public class CreateDistDirectoryWorker : IHostedService
    {
        private readonly IWebHostEnvironment hostEnvironment;

        public CreateDistDirectoryWorker(IWebHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var wwwrootPath = hostEnvironment.WebRootPath;
            string distPath = Path.Combine(wwwrootPath, "js/dist");
            if (!Directory.Exists(distPath))
            {
                Directory.CreateDirectory(distPath);
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    }
}
