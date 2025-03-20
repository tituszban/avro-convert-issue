using System.Runtime.Serialization;
using SolTechnology.Avro.Infrastructure.Attributes;

namespace AvroConvertModel;

public class TypeWithUnionAvro
{
    public string TopLevelField { get; set; }
    
    [AvroUnion(typeof(TargetObjA), typeof(TargetObjB))]
    public object UnionField { get; set; }
}

[DataContract(Name = "ObjA")]
public class TargetObjA
{
    public string FieldA { get; set; }
}

[DataContract(Name = "ObjB")]
public class TargetObjB
{
    public string FieldB { get; set; }
}
