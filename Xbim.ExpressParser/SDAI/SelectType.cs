﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.ExpressParser.SDAI
{
    public class SelectType:NamedType, ConstructedType
    {
        public List<NamedType> Selections { get; set; }
    }
}
