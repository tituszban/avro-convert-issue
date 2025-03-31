using Avro.File;
using Avro.Specific;
using SolTechnology.Avro;

namespace TypeUnionExample.Benchmarks;

public class AvroBenchmarks
{
    private Stream GetAvroStream()
    {
        List<TypeUnionExampleAvro.TypeWithUnionAvro> records =
        [
            new()
            {
                TopLevelField = "TopLevelField1",
                UnionField = new TypeUnionExampleAvro.ObjA()
                {
                    FieldA = "FieldA1"
                }
            },
            new()
            {
                TopLevelField = "TopLevelField2",
                UnionField = new TypeUnionExampleAvro.ObjB()
                {
                    FieldB = "FieldB2"
                }
            }
        ];

        var stream = new MemoryStream();
        var datumWriter =
            new SpecificWriter<TypeUnionExampleAvro.TypeWithUnionAvro>(TypeUnionExampleAvro.TypeWithUnionAvro._SCHEMA);
        using var writer = DataFileWriter<TypeUnionExampleAvro.TypeWithUnionAvro>.OpenWriter(
            datumWriter,
            stream,
            leaveOpen: true,
            codec: Codec.CreateCodec(Codec.Type.Null));

        for (var i = 0; i < 1000; i++)
        {
            foreach (var record in records)
            {
                writer.Append(record);
            }
        }

        writer.Flush();

        stream.Position = 0;
        return stream;
    }

    private Stream _stream = null!;
    
    public void SetUp()
    {
        _stream = GetAvroStream();
    }
    
    public void Cleanup()
    {
        _stream.Dispose();
    }
    
    public List<TypeUnionExampleAvro.TypeWithUnionAvro> ApacheAvro()
    {
        using var reader =
            DataFileReader<TypeUnionExampleAvro.TypeWithUnionAvro>.OpenReader(_stream,
                TypeUnionExampleAvro.TypeWithUnionAvro._SCHEMA);
        
        var readRecords = new List<TypeUnionExampleAvro.TypeWithUnionAvro>();
        foreach (var orderEvent in reader.NextEntries)
        {
            readRecords.Add(orderEvent);
        }

        return readRecords;
    }
    
    public List<AvroConvertModel.TypeWithUnionAvro> AvroConvert_()
    {
        using var reader = AvroConvert.OpenDeserializer<AvroConvertModel.TypeWithUnionAvro>(_stream);

        var readRecords = new List<AvroConvertModel.TypeWithUnionAvro>();
        while (reader.HasNext())
        {
            readRecords.Add(reader.ReadNext());
        }

        return readRecords;
    }
}