﻿using System.Collections.Generic;
using System.Net.Mail;

namespace Skeleton.DTOs;

public class EmailOptionsDto
{
    public IList<string> ToEmails { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public IList<KeyValuePair<string, string>> PlaceHolders { get; set; }
    public IList<Attachment> Attachments { get; set; }
}