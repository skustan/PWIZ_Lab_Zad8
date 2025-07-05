using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWIZ_Lab_8.Models
{
    class OczkoGameResult
    {
        public string Result { get; set; }
        public int PlayerPoints { get; set; }
        public int DealerPoints { get; set; }
        public DateTime Time { get; set; }
    }
}
