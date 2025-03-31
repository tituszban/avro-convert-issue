using System.Diagnostics;
using TypeUnionExample.Benchmarks;

var n = 100;
var measurements = new List<TimeSpan>();
for (var i = 0; i < n; i++)
{
    var stopwatch = new Stopwatch();
    var bench = new AvroBenchmarks();
    bench.SetUp();
    
    stopwatch.Start();
    bench.ApacheAvro();
    stopwatch.Stop();
    measurements.Add(stopwatch.Elapsed);
    bench.Cleanup();
}
var apacheAvroAvg = measurements.Average(x => x.TotalMilliseconds);
Console.WriteLine($"Apache Avro: {apacheAvroAvg:F2}ms");

measurements.Clear();

for (var i = 0; i < n; i++)
{
    var stopwatch = new Stopwatch();
    var bench = new AvroBenchmarks();
    bench.SetUp();
    
    stopwatch.Start();
    bench.AvroConvert_();
    stopwatch.Stop();
    measurements.Add(stopwatch.Elapsed);
    bench.Cleanup();
}


var avroConvertAvg = measurements.Average(x => x.TotalMilliseconds);

Console.WriteLine($"AvroConvert: {avroConvertAvg:F2}ms");