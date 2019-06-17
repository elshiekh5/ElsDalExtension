using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElsDalExtension
{
   public  class RecordSetDefinition
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public bool IsGenericType  { get; set; }
        public Type GenericObjectType { get; set; }

    }

}
