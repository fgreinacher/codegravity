// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace nsplit.Api.Dto
{
    [DataContract(Name = "edge")]
    public class EdgeDto
    {
        [DataMember(Name = "sources")]
        public IEnumerable<int> Sources { get; set; }

        [DataMember(Name = "targets")]
        public IEnumerable<int> Targets { get; set; }

        [DataMember(Name = "kind")]
        public string Kinds { get; set; }
    }
}