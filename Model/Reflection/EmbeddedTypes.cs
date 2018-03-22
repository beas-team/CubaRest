using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubaRest.Model.Reflection
{
    public static class EmbeddedTypes
    {
        public static Dictionary<string, Type> Types
        {
            get => new Dictionary<string, Type>()
            {
                { "dateTime", typeof(DateTime) },
                { "date", typeof(DateTime) },
                { "boolean", typeof(bool) },
                { "string", typeof(string) },
                { "double", typeof(double) },
                // { "byteArray", typeof(???) }, // TODO: biteArray unsupported yet!
                // { "time", typeof(???) }, // TODO: time unsupported yet!
                { "decimal", typeof(decimal) },
                { "uuid", typeof(string) },
                { "int", typeof(int) },
                { "long", typeof(long) },
            };
        }        
    }
}
