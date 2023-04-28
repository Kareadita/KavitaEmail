using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skeleton.DTOs;
using Skeleton.Services;

namespace Skeleton.Controllers;

/// <summary>
/// Responsible for all Send to Device Email Services
/// </summary>
public class SendToController : BaseApiController
{
    private readonly ILogger<SendToController> _logger;
    private readonly SmtpConfig _smtpConfig;
    private readonly IEmailService _emailService;
    private readonly string[] _permittedExtensions = { ".epub", ".pdf", ".cbz", ".zip", ".cbr", ".rar", ".cbt", ".tar.gz", ".cb7", ".7zip", ".7z" };
    private readonly string _tempPath;

    public SendToController(ILogger<SendToController> logger, IOptions<SmtpConfig> smtpConfig, IEmailService emailService)
    {
        _logger = logger;
        _smtpConfig = smtpConfig.Value;
        _emailService = emailService;
        
        _tempPath = Path.Join(Directory.GetCurrentDirectory(), "config", "temp");

        if (!Directory.Exists(_tempPath)) Directory.CreateDirectory(_tempPath);
    }


    [DisableRequestSizeLimit]
    [HttpPost]
    public async Task<ActionResult<bool>> UploadAndSend(IFormCollection formCollection)
    {
        Request.Headers.TryGetValue("x-kavita-installId", out var installId);
        Request.Headers.TryGetValue("x-kavita-version", out var version);
        _logger.LogInformation("[send-to] Request came in from {InstallId} on version {Version}", installId, version);
        if (!_smtpConfig.AllowSendTo) return BadRequest("This API is not enabled");
        
        _logger.LogInformation("Received a Send to request for {FileCount} files", formCollection.Files.Count);

        if (!formCollection.ContainsKey("email") || string.IsNullOrEmpty(formCollection["email"]))
            return BadRequest("Destination email must be set");

        if (formCollection.Files.Count == 0) return BadRequest("Nothing to send to device");
        
        // Validate size limit
        var size = formCollection.Files.Sum(f => f.Length);
        if (size > _smtpConfig.SizeLimit) return BadRequest("Files are too large");
        
        // Validate correct extension
        if (!formCollection.Files.All(f =>
            {
                var ext = Path.GetExtension(f.FileName).ToLowerInvariant();
                return !string.IsNullOrEmpty(ext) && _permittedExtensions.Contains(ext);
            }))
        {
            return BadRequest("Unsupported filetype");
        }

        var tempFiles = new List<string>();
        foreach (var formFile in formCollection.Files)
        {
            if (formFile.Length <= 0) continue;
            var lastIndex = formFile.FileName.LastIndexOf(".", StringComparison.Ordinal);
            var filename = formFile.FileName.Substring(0, lastIndex).Replace('.', '_') + Path.GetExtension(formFile.FileName);

            if (!Directory.Exists(_tempPath)) Directory.CreateDirectory(_tempPath);
            var tempFile = Path.Join(_tempPath, filename);
            
            await using var stream = System.IO.File.Create(tempFile);
            await formFile.CopyToAsync(stream);
            stream.Close();
            tempFiles.Add(tempFile);
        }

        await _emailService.SendToDevice(formCollection["email"], tempFiles);

        foreach (var file in tempFiles)
        {
            System.IO.File.Delete(file);
        }

        return Ok(true);
    }
}