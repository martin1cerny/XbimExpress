﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.ExpressParser.SDAI
{
    public struct ObjectInfoId
    {
        public bool Equals(ObjectInfoId other)
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

        public static implicit operator ObjectInfoId(string value)
        {
            return new ObjectInfoId{Value = value};
        }

        public static explicit operator string(ObjectInfoId value)
        {
            return value.Value;
        }

        public static bool operator ==(ObjectInfoId x, ObjectInfoId y)
        {
            return x.Value == y.Value;
        }

        public static bool operator !=(ObjectInfoId x, ObjectInfoId y)
        {
            return x.Value != y.Value;
        }
    }
}
