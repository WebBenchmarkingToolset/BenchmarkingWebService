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
        public uint fileWrite([FromQuery] long fileSizeMB)
        {
            string filePath = $"fileWrite-{DateTime.UtcNow.Ticks}.bin";
            long fileSize = fileSizeMB * 1024 * 1024; // to bytes
            byte[] data = new byte[fileSize];
            new Random().NextBytes(data);

            // Measure write speed
            Stopwatch sw = Stopwatch.StartNew();
            System.IO.File.WriteAllBytes(filePath, data);
            sw.Stop();

            System.IO.File.Delete(filePath);
            return Convert.ToUInt32(sw.Elapsed.TotalMilliseconds); ;
        }
        
        [HttpGet]
        [Route("fileRead")]
        //https://localhost:7167/api/Operations/fileRead?fileSizeMB=100
        public uint fileRead([FromQuery] long fileSizeMB)
        {
            string filePath = $"fileRead-{DateTime.UtcNow.Ticks}.bin";
            long fileSize = fileSizeMB * 1024 * 1024; // to bytes
            byte[] data = new byte[fileSize];
            new Random().NextBytes(data);
            System.IO.File.WriteAllBytes(filePath, data);

            // Measure write speed
            Stopwatch sw = Stopwatch.StartNew();
            byte[] readData = System.IO.File.ReadAllBytes(filePath);
            sw.Stop();

            System.IO.File.Delete(filePath);
            return Convert.ToUInt32(sw.Elapsed.TotalMilliseconds);
        }





        [HttpGet]
        [Route("memoryWrite")]
        //https://localhost:7167/api/Operations/memoryWrite?dataSizeMB=100
        public uint memoryWrite([FromQuery] long dataSizeMB)
        {
            long dataSize = dataSizeMB * 1024 * 1024; // to bytes
            byte[] memory = new byte[dataSize];

            Stopwatch sw = Stopwatch.StartNew();

            // Writing to memory
            for (long i = 0; i < memory.Length; i++)
                memory[i] = (byte)(i % 256);
            sw.Stop();
            return Convert.ToUInt32(sw.Elapsed.TotalMilliseconds);
        }
        
        
        [HttpGet]
        [Route("memoryRead")]
        //https://localhost:7167/api/Operations/memoryRead?dataSizeMB=100
        public uint memoryRead([FromQuery] long dataSizeMB)
        {
            long dataSize = dataSizeMB * 1024 * 1024; // to bytes
            byte[] memory = new byte[dataSize];


            // Writing to memory
            for (int i = 0; i < memory.Length; i++)
                memory[i] = (byte)(i % 256);

            Stopwatch sw = Stopwatch.StartNew();
            // Reading from memory
            long sum = 0;
            for (int i = 0; i < memory.Length; i++)
                sum += memory[i];
            sw.Stop();
            return Convert.ToUInt32(sw.Elapsed.TotalMilliseconds);
        }
    }
}
