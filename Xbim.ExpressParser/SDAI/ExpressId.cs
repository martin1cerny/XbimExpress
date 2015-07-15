using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.ExpressParser.SDAI
{
    public struct ExpressId
    {
        public ExpressId(string value)
        {
            Value = value;
        }

        public bool Equals(ExpressId other)
        {
            return string.Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }

        public string Value;

        public override bool Equals(object obj)
        {
            return Value != null && Value.Equals(obj);
        }

        public override string ToString()
        {
            return Value ?? base.ToString();
        }

        public static implicit operator ExpressId(string value)
        {
            return new ExpressId{Value = value};
        }

        public static implicit operator string(ExpressId value)
        {
            return value.Value;
        }

       

        public static bool operator ==(ExpressId x, ExpressId y)
        {
            return x.Value == y.Value;
        }

        public static bool operator !=(ExpressId x, ExpressId y)
        {
            return x.Value != y.Value;
        }
    }
}
