using System;
using System.Collections.Generic;
using System.Text;

namespace TravelMonkey.Models
{
    public class Language : ICloneable
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string Voice { get; set; }

        public string Flag { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public override string ToString()
        {
            return $"Name: {Name} Code: {Code} Voice: {Voice}";
        }
    }
}
