using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;
using System.Web;

public static class HtmlHelpers
{
    public static IDisposable DefineScripts(this IHtmlHelper helper, string blockName = "", bool isScope = true)
    {
        var fileName = helper.ViewContext.ExecutingFilePath.Replace(".cshtml", "").Replace("/", "_");
        if (string.IsNullOrWhiteSpace(blockName))
        {
            blockName = fileName;
        }
        else if (isScope)
        {
            blockName += fileName;
        }
        return new ScriptBlock(helper.ViewContext, blockName);
    }

    public static HtmlString RenderScripts(this IHtmlHelper helper, string blockName = "", bool isScope = true)
    {
        var fileName = helper.ViewContext.View.Path.Replace(".cshtml", "").Replace("/", "_");
        if (string.IsNullOrWhiteSpace(blockName))
        {
            blockName = fileName;
        }
        else if (isScope)
        {
            blockName += fileName;
        }
        return new HtmlString(string.Join(Environment.NewLine, GetPageScriptsList(helper.ViewContext.HttpContext, blockName)));
    }

    private static List<string> GetPageScriptsList(HttpContext httpContext, string blockName)
    {
        var pageScripts = (List<string>)httpContext.Items[blockName];
        if (pageScripts == null)
        {
            pageScripts = new List<string>();
            httpContext.Items[blockName] = pageScripts;
        }
        return pageScripts;
    }

    private class ScriptBlock : IDisposable
    {
        private readonly TextWriter _originalWriter;
        private readonly StringWriter _scriptsWriter;

        private readonly ViewContext _viewContext;
        private readonly string _blockName;
        public ScriptBlock(ViewContext viewContext, string blockName)
        {
            _viewContext = viewContext;
            _originalWriter = _viewContext.Writer;
            _viewContext.Writer = _scriptsWriter = new StringWriter();
            _blockName = blockName;
        }

        public void Dispose()
        {
            _viewContext.Writer = _originalWriter;
            var pageScripts = GetPageScriptsList(_viewContext.HttpContext, _blockName);
            pageScripts.Add(_scriptsWriter.ToString());
        }
    }

    public static string GetString(this IHtmlContent content)
    {
        using (var writer = new System.IO.StringWriter())
        {
            content.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }
    }

    public static string? GetString(this IHtmlString? content)
    {
        return content?.ToHtmlString();
    }
}