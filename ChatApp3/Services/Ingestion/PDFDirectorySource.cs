using Microsoft.SemanticKernel.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
using System.Text.RegularExpressions;

namespace ChatApp3.Services.Ingestion;

public class PDFDirectorySource(string sourceDirectory) : IIngestionSource
{
    private static readonly string[] SupportedExtensions = [".pdf", ".txt", ".html"];

    public static string SourceFileId(string path) => Path.GetFileName(path);
    public static string SourceFileVersion(string path) => File.GetLastWriteTimeUtc(path).ToString("o");

    public string SourceId => $"{nameof(PDFDirectorySource)}:{sourceDirectory}";

    public Task<IEnumerable<IngestedDocument>> GetNewOrModifiedDocumentsAsync(IReadOnlyList<IngestedDocument> existingDocuments)
    {
        var results = new List<IngestedDocument>();
        var sourceFiles = SupportedExtensions.SelectMany(ext => Directory.GetFiles(sourceDirectory, "*" + ext)).ToList();
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
        var currentFiles = SupportedExtensions.SelectMany(ext => Directory.GetFiles(sourceDirectory, "*" + ext)).ToList();
        var currentFileIds = currentFiles.ToLookup(SourceFileId);
        var deletedDocuments = existingDocuments.Where(d => !currentFileIds.Contains(d.DocumentId));
        return Task.FromResult(deletedDocuments);
    }

    public Task<IEnumerable<IngestedChunk>> CreateChunksForDocumentAsync(IngestedDocument document)
    {
        var filePath = Path.Combine(sourceDirectory, document.DocumentId);
        var ext = Path.GetExtension(filePath).ToLowerInvariant();
        List<(int PageNumber, int IndexOnPage, string Text)> paragraphs;

        if (ext == ".pdf")
        {
            using var pdf = PdfDocument.Open(filePath);
            paragraphs = pdf.GetPages().SelectMany(GetPageParagraphs).ToList();
        }
        else if (ext == ".txt")
        {
            var text = File.ReadAllText(filePath);
#pragma warning disable SKEXP0050 // Type is for evaluation purposes only
            paragraphs = TextChunker.SplitPlainTextParagraphs([text], 200)
                .Select((t, i) => (1, i, t)).ToList();
#pragma warning restore SKEXP0050 // Type is for evaluation purposes only
        }
        else if (ext == ".html")
        {
            var html = File.ReadAllText(filePath);
            var text = StripHtmlTags(html);
#pragma warning disable SKEXP0050 // Type is for evaluation purposes only
            paragraphs = TextChunker.SplitPlainTextParagraphs([text], 200)
                .Select((t, i) => (1, i, t)).ToList();
#pragma warning restore SKEXP0050 // Type is for evaluation purposes only
        }
        else
        {
            paragraphs = [];
        }

        return Task.FromResult(paragraphs.Select(p => new IngestedChunk
        {
            Key = Guid.CreateVersion7().ToString(),
            DocumentId = document.DocumentId,
            PageNumber = p.PageNumber,
            Text = p.Text,
        }));
    }

    private static IEnumerable<(int PageNumber, int IndexOnPage, string Text)> GetPageParagraphs(Page pdfPage)
    {
        var letters = pdfPage.Letters;
        var words = NearestNeighbourWordExtractor.Instance.GetWords(letters);
        var textBlocks = DocstrumBoundingBoxes.Instance.GetBlocks(words);
        var pageText = string.Join(Environment.NewLine + Environment.NewLine,
            textBlocks.Select(t => t.Text.ReplaceLineEndings(" ")));
#pragma warning disable SKEXP0050 // Type is for evaluation purposes only
        return TextChunker.SplitPlainTextParagraphs([pageText], 200)
            .Select((text, index) => (pdfPage.Number, index, text));
#pragma warning restore SKEXP0050 // Type is for evaluation purposes only
    }

    private static string StripHtmlTags(string html)
    {
        // Remove script and style blocks
        html = Regex.Replace(html, "<script.*?</script>", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        html = Regex.Replace(html, "<style.*?</style>", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        // Remove all HTML tags
        html = Regex.Replace(html, "<.*?>", string.Empty);
        // Decode HTML entities
        return System.Net.WebUtility.HtmlDecode(html);
    }
}
