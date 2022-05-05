using BoldReports.Web.ReportViewer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ReportDemo.Api;

[Route("api/{controler}/{action}/{id?}")]
[ApiController]
public class ReportController : ControllerBase, IReportController
{
    private readonly IMemoryCache _cache;
    private readonly IWebHostEnvironment _hostingEnvironment;

    public ReportController(IMemoryCache memoryCache, IWebHostEnvironment hostingEnvironment)
    {
        _cache = memoryCache;
        _hostingEnvironment = hostingEnvironment;
    }
    
    [ActionName("GetResource")]
    [AcceptVerbs("GET")]
    public object GetResource(ReportResource resource)
    {
        return ReportHelper.GetResource(resource, this, _cache);
    }

    [NonAction]
    public void OnInitReportOptions(ReportViewerOptions reportOption)
    {
        string basePath = _hostingEnvironment.WebRootPath;
        FileStream inputStream = new(basePath + @"\reports\" + reportOption.ReportModel.ReportPath + ".rdl", System.IO.FileMode.Open, System.IO.FileAccess.Read);
        MemoryStream reportStream = new();
        inputStream.CopyTo(reportStream);
        reportStream.Position = 0;
        inputStream.Close();
        reportOption.ReportModel.Stream = reportStream;
    }

    [NonAction]
    public void OnReportLoaded(ReportViewerOptions reportOption)
    {
    }

    [HttpPost]
    public object PostFormReportAction()
    {
        return ReportHelper.ProcessReport(null, this, _cache);
    }

    [HttpPost]
    public object PostReportAction([FromBody] Dictionary<string, object> jsonArray)
    {
        return ReportHelper.ProcessReport(jsonArray, this, this._cache);
    }
}
