using System;
using Microsoft.AspNetCore.Http;

namespace QrCo3ds.Models
{
    public class GameRequest
    {
        public IFormFile BoxArtFile { get; set; }
        public IFormFile CiaFile { get; set; }
        public GameInfo Data { get; set; }
    }
}
