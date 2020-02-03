using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace QrCo3ds.Models
{
    public class DlcRequest
    {
        public IFormFile CiaFile { get; set; }
        public DlcInfo Data { get; set; }
    }

    public class GameRequest
    {
        public IFormFile BoxArtFile { get; set; }
        public IFormFile CiaFile { get; set; }
        public GameInfo Data { get; set; }
        public List<IFormFile> ScreenshotFiles { get; set; } = new List<IFormFile>();
    }
}
