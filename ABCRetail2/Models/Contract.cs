using System;
using System.ComponentModel.DataAnnotations;

namespace ABCRetail2.Models
{
    public class Contract
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; }  // Original file name

        [Required]
        public string FilePath { get; set; }  // Path where the file is stored

        [Required]
        public DateTime UploadDate { get; set; } = DateTime.Now;  // Date of upload
    }
}
