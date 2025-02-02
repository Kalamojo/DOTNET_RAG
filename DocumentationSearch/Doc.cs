using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Data;

namespace DocumentationSearch;

public class Doc
{
    [VectorStoreRecordKey]
    public required string DocId {  get; init; }

    [VectorStoreRecordData]
    [TextSearchResultName]
    public required string SectionName { get; init; }

    [VectorStoreRecordData]
    [TextSearchResultValue]
    public required string Text { get; init; }

    [VectorStoreRecordData]
    [TextSearchResultLink]
    public required string Link { get; init; }

    [VectorStoreRecordVector(Constants.VectorDimensions)]
    public ReadOnlyMemory<float> Embedding { get; init; }
}
