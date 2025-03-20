using Avro.File;
using Avro.Specific;
using SolTechnology.Avro;

namespace TypeUnionExample;

public class Tests
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

        foreach (var record in records)
        {
            writer.Append(record);
        }

        writer.Flush();

        stream.Position = 0;
        return stream;
    }

    [Test]
    public void AvroWrite_AvroRead()
    {
        // Encode data with Apache.Avro
        using var stream = GetAvroStream();

        // Decode data with Apache.Avro
        using var reader =
            DataFileReader<TypeUnionExampleAvro.TypeWithUnionAvro>.OpenReader(stream,
                TypeUnionExampleAvro.TypeWithUnionAvro._SCHEMA);

        var readRecords = new List<TypeUnionExampleAvro.TypeWithUnionAvro>();
        foreach (var orderEvent in reader.NextEntries)
        {
            readRecords.Add(orderEvent);
        }

        // Assert matches, this passes
        Assert.Multiple(() =>
        {
            Assert.That(readRecords.Count, Is.EqualTo(2));
            Assert.That(readRecords[0].TopLevelField, Is.EqualTo("TopLevelField1"));
            Assert.That(readRecords[0].UnionField is TypeUnionExampleAvro.ObjA { FieldA: "FieldA1" });
            Assert.That(readRecords[1].TopLevelField, Is.EqualTo("TopLevelField2"));
            Assert.That(readRecords[1].UnionField is TypeUnionExampleAvro.ObjB { FieldB: "FieldB2" });
        });
    }

    [Test]
    public void GenerateModel()
    {
        // Crude but simple way of getting the model
        var content = File.ReadAllText("schema.json");
        var model = AvroConvert.GenerateModel(content);
        Assert.Fail(model);
    }

    [Test]
    public void AvroWrite_AvroConvertRead()
    {
        // Encode data with Apache.Avro
        using var stream = GetAvroStream();

        // Decode data with AvroConvert
        using var reader = AvroConvert.OpenDeserializer<AvroConvertModel.TypeWithUnionAvro>(stream);

        var readRecords = new List<AvroConvertModel.TypeWithUnionAvro>();
        while (reader.HasNext())
        {
            readRecords.Add(reader.ReadNext());
        }

        // Assert matches
        Assert.Multiple(() =>
        {
            Assert.That(readRecords.Count, Is.EqualTo(2));
            Assert.That(readRecords[0].TopLevelField, Is.EqualTo("TopLevelField1"));
            // This fails, because the UnionField doesn't have any of data
            Assert.That(readRecords[0].UnionField is AvroConvertModel.TargetObjA { FieldA: "FieldA1" });
            Assert.That(readRecords[1].TopLevelField, Is.EqualTo("TopLevelField2"));
            // This too fails for the same reason
            Assert.That(readRecords[1].UnionField is AvroConvertModel.TargetObjB { FieldB: "FieldB2" });
        });
    }
}