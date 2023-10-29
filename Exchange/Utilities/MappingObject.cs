using Exchange.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exchange.Utilities
{
    public class MappingObject
    {
        public static IEnumerable<T> MapTo<T>(APIResponseforExchangerates source)
    where T : new()
        {
            var properties = typeof(T).GetProperties();

            foreach (var datum in source.Data)
            {
                var t = new T();

                for (var colIndex = 0; colIndex < source.quotes.ToList().Count; colIndex++)
                {
                    var property = properties.SingleOrDefault(p => p.Name.Equals(source.quotes.ToList()[colIndex], StringComparison.InvariantCultureIgnoreCase));
                    if (property != null)
                    {
                        property.SetValue(t, Convert.ChangeType(datum[colIndex], property.PropertyType));
                    }
                }
                yield return t;
            }
        }
    }
}
