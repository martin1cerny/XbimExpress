using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Xbim.CodeGeneration.Differences
{
    internal class ReadOnlyItemSets: IEnumerable<ItemSetAttribute>
    {
        private static readonly List<ItemSetAttribute> Attributes = new List<ItemSetAttribute>(); 

        static ReadOnlyItemSets()
        {
            var data = Properties.Resources.ReadonlyItemSets;
            using (var reader = new StringReader(data))
            {
                var line = reader.ReadLine();
                while (!string.IsNullOrWhiteSpace(line))
                {
                    var parts = line.Split(',').Select(p => p.Trim()).ToArray();
                    Attributes.Add(new ItemSetAttribute {Class = parts[0], Attribute = parts[1]});
                    line = reader.ReadLine();
                }
            }
        }

        public IEnumerator<ItemSetAttribute> GetEnumerator()
        {
            return Attributes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal class ItemSetAttribute
    {
        public string Class { get; set; }
        public string Attribute { get; set; }
    }
}
