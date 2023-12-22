using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_GMI.Models
{
    public class DefaultSheetsModel
    {
        public string range { get; set; }
        public string majorDimension { get; set; }
        public List<List<string>> Values { get; set; }
    }
}
