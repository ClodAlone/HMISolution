using Microsoft.SemanticKernel.Text;
using System.Text;
using System.Text.RegularExpressions;

namespace ChatApp1.Services.Ingestion;

public class HTMLDirectorySource(string sourceDirectory) : IIngestionSource
{
    public static string SourceFileId(string path) => Path.GetFileName(path);
    public static string SourceFileVersion(string path) => File.GetLastWriteTimeUtc(path).ToString("o");

    public string SourceId => $"{nameof(HTMLDirectorySource)}:{sourceDirectory}";

    public Task<IEnumerable<IngestedDocument>> GetNewOrModifiedDocumentsAsync(IReadOnlyList<IngestedDocument> existingDocuments)
    {
        var results = new List<IngestedDocument>();
        var sourceFiles = Directory.GetFiles(sourceDirectory, "*.html");
        var existingDocumentsById = existingDocuments.ToDictionary(d => d.DocumentId);

        foreach (var sourceFile in sourceFiles)
        {
            var sourceFileId = SourceFileId(sourceFile);
            var sourceFileVersion = SourceFileVersion(sourceFile);
            var existingDocumentVersion = existingDocumentsById.TryGetValue(sourceFileId, out var existingDocument) ? existingDocument.DocumentVersion : null;
            if (existingDocumentVersion != sourceFileVersion)
            {
                results.Add(new() { Key = Guid.CreateVersion7().ToString(), SourceId = SourceId, DocumentId = sourceFileId, DocumentVersion = sourceFileVersion });
            }
        }

        return Task.FromResult((IEnumerable<IngestedDocument>)results);
    }

    public Task<IEnumerable<IngestedDocument>> GetDeletedDocumentsAsync(IReadOnlyList<IngestedDocument> existingDocuments)
    {
        var currentFiles = Directory.GetFiles(sourceDirectory, "*.html");
        var currentFileIds = currentFiles.ToLookup(SourceFileId);
        var deletedDocuments = existingDocuments.Where(d => !currentFileIds.Contains(d.DocumentId));
        return Task.FromResult(deletedDocuments);
    }

    public Task<IEnumerable<IngestedChunk>> CreateChunksForDocumentAsync(IngestedDocument document)
    {
        var filePath = Path.Combine(sourceDirectory, document.DocumentId);
        var html = File.ReadAllText(filePath, Encoding.UTF8);
        var text = StripHtmlTags(html);
#pragma warning disable SKEXP0050 // Type is for evaluation purposes only
        var paragraphs = TextChunker.SplitPlainTextParagraphs([text], 200)
            .Select((chunk, index) => new IngestedChunk
            {
                Key = Guid.CreateVersion7().ToString(),
                DocumentId = document.DocumentId,
                PageNumber = 1,
                Text = chunk,
            });
#pragma warning restore SKEXP0050 // Type is for evaluation purposes only
        return Task.FromResult(paragraphs);
    }

    private static string StripHtmlTags(string html)
    {
        return Regex.Replace(html, "<.*?>", string.Empty);
    }
}
