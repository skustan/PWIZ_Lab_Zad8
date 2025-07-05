using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWIZ_Lab_8.Models
{
    public class WarGameResult
    {
        public string Winner { get; set; } = string.Empty;
        public int PlayerCardsLeft { get; set; }
        public int ComputerCardsLeft { get; set; }
        public DateTime GameEndTime { get; set; }
    }
}
