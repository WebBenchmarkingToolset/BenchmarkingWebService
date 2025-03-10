using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
namespace CloudBenchmark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Operations : ControllerBase
    {
        [HttpGet]
        [Route("fileWrite")]
        //https://localhost:7167/api/Operations/fileWrite?fileSizeMB=100
        public double fileWrite([FromQuery] int fileSizeMB)
        {
            string filePath = $"fileWrite-{DateTime.UtcNow.Ticks}.bin";
            int fileSize = fileSizeMB * 1024 * 1024; // to MB
            byte[] data = new byte[fileSize];
            new Random().NextBytes(data);

            // Measure write speed
            Stopwatch sw = Stopwatch.StartNew();
            System.IO.File.WriteAllBytes(filePath, data);
            sw.Stop();

            System.IO.File.Delete(filePath);
            return Math.Round(sw.Elapsed.TotalMilliseconds);
        }
        
        [HttpGet]
        [Route("fileRead")]
        //https://localhost:7167/api/Operations/fileRead?fileSizeMB=100
        public double fileRead([FromQuery] int fileSizeMB)
        {
            string filePath = $"fileRead-{DateTime.UtcNow.Ticks}.bin";
            int fileSize = fileSizeMB * 1024 * 1024; // to MB
            byte[] data = new byte[fileSize];
            new Random().NextBytes(data);
            System.IO.File.WriteAllBytes(filePath, data);

            // Measure write speed
            Stopwatch sw = Stopwatch.StartNew();
            byte[] readData = System.IO.File.ReadAllBytes(filePath);
            sw.Stop();

            System.IO.File.Delete(filePath);
            return Math.Round(sw.Elapsed.TotalMilliseconds);
        }
    }
}
