using Microsoft.Extensions.VectorData;

namespace VectorStoreCreator;

public class Doc
{
    [VectorStoreRecordKey]
    public required string DocId {  get; init; }

    [VectorStoreRecordData]
    public required string SectionName { get; init; }

    [VectorStoreRecordData]
    public required string Text { get; init; }

    [VectorStoreRecordData]
    public required string Link { get; init; }

    [VectorStoreRecordVector(Constants.VectorDimensions)]
    public ReadOnlyMemory<float> Embedding { get; init; }
}
