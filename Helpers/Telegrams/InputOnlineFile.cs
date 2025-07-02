using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Telegrams
{
    public class InputOnlineFile
    {
        private MemoryStream stream;
        private string v;

        public InputOnlineFile(MemoryStream stream, string v)
        {
            this.stream = stream;
            this.v = v;
        }
    }
}
