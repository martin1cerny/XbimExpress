using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.ExpressParser.SDAI
{
    public struct ExpressId:IEquatable<string>
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
            if (Value == null)
                return false;

            if (obj is ExpressId id)
                return Equals(id);
            if (obj is string str)
                return Equals(str);

            return false;
        }

        public bool Equals(string other)
        {
            return Value.Equals(other);
        }

        public override string ToString()
        {
            return Value ?? base.ToString();
        }

        public string ToUpper() {  return Value.ToUpperInvariant();  }

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

        public static bool operator ==(ExpressId x, string y)
        {
            return x.Value == y;
        }

        public static bool operator !=(ExpressId x, string y)
        {
            return x.Value != y;
        }
    }
}
