using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADVoice
{
    public class VoiceAddress
    {
        public VoiceAddress()
        {
            Call = 0;
            Error = 0;
        }

        public string Address { get; set; }
        public int Call { get; set; }
        public int Error { get; set; }

    }
}
