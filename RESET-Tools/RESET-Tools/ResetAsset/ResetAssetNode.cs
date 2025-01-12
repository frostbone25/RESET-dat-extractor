using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESET_Tools
{
    public class ResetAssetNode
    {
        public string Name { get; set; }
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
        public List<ResetAssetNode> Children { get; set; } = new List<ResetAssetNode>();
    }
}
