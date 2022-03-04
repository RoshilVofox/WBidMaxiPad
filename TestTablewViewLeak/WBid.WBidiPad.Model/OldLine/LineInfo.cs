#region NameSpace
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
#endregion

namespace WBid.WBidiPad.Model.OldLine
{
    [ProtoContract]
    public class LineInfo
    {
        [ProtoMember(1)]
        public string LineVersion { get; set; }
        [ProtoMember(2)]
        public Dictionary<string, Model.OldLine.Line> Lines { get; set; }
    }
}
