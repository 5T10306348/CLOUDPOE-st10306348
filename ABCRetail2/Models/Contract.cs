using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABCRetail2.Models
{
    public class Contract
    {
        [Key]
        public int Id { get; set; }
        public string FileName { get; set; }
        public DateTime UploadDate { get; set; }

        // Binary data for the file
        public byte[] FileData { get; set; }
        public string ContentType { get; set; } // Store the file type, e.g., "application/pdf"
    }

}
