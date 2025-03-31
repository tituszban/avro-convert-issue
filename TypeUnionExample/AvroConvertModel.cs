using System.Runtime.Serialization;
using SolTechnology.Avro.Infrastructure.Attributes;

namespace AvroConvertModel;

public class TypeWithUnionAvro
{
    public string TopLevelField { get; set; }
    
    [AvroUnion(typeof(ObjA), typeof(ObjB))]
    public object UnionField { get; set; }
}

public class ObjA
{
    public string FieldA { get; set; }
}

public class ObjB
{
    public string FieldB { get; set; }
}
