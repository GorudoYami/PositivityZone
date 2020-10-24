using System;
using System.Collections.Generic;
using System.Text;

namespace PositivityZone.API.Models {
    public class Entry {
        public int ID { get; set; }
        public string Text { get; set; }
        public DateTime Posted { get; set; }
        public bool Approved { get; set; }
        public bool Disapproved { get; set; }
        public bool Answered { get; set; }
        public string Language { get; set; }
        public string UID { get; set; }
    }
}
