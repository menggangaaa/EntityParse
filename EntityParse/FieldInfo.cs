using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityParse
{
    class FieldInfo
    {
        public string name { get; set; }
        public string alias { get; set; }
        public string tableName { get; set; }
        public string relTable { get; set; }
        public string dataType { get; set; }
        public string fullPath { get; set; }
    }
}
