using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Reflection;
using System.Text;

namespace Luminus.API.Controllers
{
    [ApiController]
    [Route("api/file")]
    public class FileController : ControllerBase
    {
        private readonly string assemblyPath = new FileInfo(fileName: Assembly.GetExecutingAssembly().Location).DirectoryName;
        [HttpPost("send")]
        public async Task<IActionResult> FilePost(IFormFile file)
        {
            if (!Directory.Exists($"{assemblyPath}/files/"))
                Directory.CreateDirectory($"{assemblyPath}/files/");
            using var streamWriter = new FileStream($"{assemblyPath}/files/{file.FileName}", FileMode.OpenOrCreate);
            await file.CopyToAsync(streamWriter);
            return Ok();
        }

        [HttpGet("{fileName}")]
        public IActionResult FileGet(string fileName)
        {
            try
            {
                var data = System.IO.File.ReadAllBytes($"{assemblyPath}/files/{fileName}");
                return File(data, "application/octet-stream", fileDownloadName: fileName);
            }
            catch
            {

            }
            return NotFound();
        }
    }
}
