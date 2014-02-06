// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using System.Runtime.Serialization;

#endregion

namespace nsplit.Api.Dto
{
    [DataContract]
    public class NodeDto
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "text")]
        public string Name { get; set; }

        [IgnoreDataMember]
        public bool IsLeaf { get; set; }

        [DataMember(Name = "children")]
        public bool CanHaveChildren
        {
            get { return !IsLeaf; }
        }

        [DataMember(Name = "icon")]
        public string Icon
        {
            get { return IsLeaf ? "/img/c.png" : "/img/n.png"; }
        }
    }
}