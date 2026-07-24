using System;

namespace HGC.Models
{
    public class CollectionPhoto
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string OriginalPath { get; set; } = "";

        public string ThumbnailPath { get; set; } = "";

        public bool IsMain { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}