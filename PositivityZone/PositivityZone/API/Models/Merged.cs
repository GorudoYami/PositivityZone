using System;
using System.Collections.Generic;
using System.Text;

namespace PositivityZone.API.Models {
    public class Merged {
        public Entry Entry { get; set; }
        public Answer Answer { get; set; }

        public Merged(Entry entry) {
            Entry = entry;
            Answer = null;
        }

        public Merged(Answer answer) {
            Answer = answer;
            Entry = null;
        }
    }
}
