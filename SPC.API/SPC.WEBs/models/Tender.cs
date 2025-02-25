﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPC.Web.Models
{
    [Table("Tenders")]
    public class Tender
    {
        public int Id { get; set; }


        public int SupplierId { get; set; }

        public string Description { get; set; }

        public string Status { get; set; } = "Pending"; // Default to Pending
        public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;
    }
}
