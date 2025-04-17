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
        public UInt64 fileWrite([FromQuery] long fileSizeMB)
        {
            string filePath = $"fileWrite-{Guid.NewGuid()}.bin";
            long fileSize = fileSizeMB * 1024 * 1024; // to bytes
            byte[] data = new byte[fileSize];
            new Random().NextBytes(data);

            // Measure write speed
            Stopwatch sw = Stopwatch.StartNew();
            System.IO.File.WriteAllBytes(filePath, data);
            sw.Stop();

            System.IO.File.Delete(filePath);
            return Convert.ToUInt64(sw.Elapsed.TotalMilliseconds); ;
        }
        
        [HttpGet]
        [Route("fileRead")]
        //https://localhost:7167/api/Operations/fileRead?fileSizeMB=100
        public UInt64 fileRead([FromQuery] long fileSizeMB)
        {
            string filePath = $"fileRead-{Guid.NewGuid()}.bin";
            long fileSize = fileSizeMB * 1024 * 1024; // to bytes
            byte[] data = new byte[fileSize];
            new Random().NextBytes(data);
            System.IO.File.WriteAllBytes(filePath, data);

            // Measure write speed
            Stopwatch sw = Stopwatch.StartNew();
            byte[] readData = System.IO.File.ReadAllBytes(filePath);
            sw.Stop();

            System.IO.File.Delete(filePath);
            return Convert.ToUInt64(sw.Elapsed.TotalMilliseconds);
        }





        [HttpGet]
        [Route("memoryWrite")]
        //https://localhost:7167/api/Operations/memoryWrite?dataSizeMB=100
        public UInt64 memoryWrite([FromQuery] long dataSizeMB)
        {
            long dataSize = dataSizeMB * 1024 * 1024; // to bytes
            byte[] memory = new byte[dataSize];

            Stopwatch sw = Stopwatch.StartNew();

            // Writing to memory
            for (long i = 0; i < memory.Length; i++)
                memory[i] = (byte)(i % 256);
            sw.Stop();
            return Convert.ToUInt64(sw.Elapsed.TotalMilliseconds);
        }
        
        
        [HttpGet]
        [Route("memoryRead")]
        //https://localhost:7167/api/Operations/memoryRead?dataSizeMB=100
        public UInt64 memoryRead([FromQuery] long dataSizeMB)
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
            return Convert.ToUInt64(sw.Elapsed.TotalMilliseconds);
        }
        
        
        [HttpGet]
        [Route("cpuStress")]
        //https://localhost:7167/api/Operations/cpuStress?operations=100&threads=10
        public UInt64 cpuStress([FromQuery] long operations, [FromQuery] long threads)
        {
            int numCores = Environment.ProcessorCount;
            Console.WriteLine($"Using {numCores} CPU cores");

            Stopwatch sw = Stopwatch.StartNew();

            Parallel.For(0, threads, _ =>
            {
                double result = 0;
                for (int i = 1; i <= operations; i++)
                    result += Math.Sqrt(i);
            });

            sw.Stop();
            return Convert.ToUInt64(sw.Elapsed.TotalMilliseconds);
        }



        [HttpPost]
        [Route("dataFile")]
        //post https://localhost:7167/api/Operations/dataFile
        public DataFilePayload dataFile(DataFilePayload request)
        {
            return request;
        }
        
        [HttpPost]
        [Route("runLoad")]
        //post https://localhost:7167/api/Operations/runLoad
        public string runLoad(RunLoadRequest request)
        {
            var reader = new StreamReader(request.PythonFile.OpenReadStream());
            string pythonCode =reader.ReadToEnd();

            PythonRunner pythonRunner = new PythonRunner();
            var result = pythonRunner.RunFromString<string>(pythonCode, request.ResultVariableName);
            return result;
        }
    }

    public class DataFilePayload
    {
        public IFormFile DataFile { set; get; }
    }
    
    public class RunLoadRequest
    {
        public IFormFile PythonFile { set; get; }
        public string ResultVariableName { set; get; }
    }

}
