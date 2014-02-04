// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using System.Runtime.Serialization;

#endregion

namespace nsplit.Api.Dto
{
    [DataContract]
    public class LinkDto
    {
        [DataMember(Name = "source")]
        public int Source { get; set; }

        [DataMember(Name = "target")]
        public int Target { get; set; }

        [DataMember(Name = "value")]
        public int Value
        {
            get { return 1; }
        }
    }
}