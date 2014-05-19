using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LolTroll
{
    [DataContract]
    class Troll
    {
        public TrollData Context
        {
            get;
            set;
        }

        private String name;
        [DataMember]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                if (name.Length == 0)
                {
                    if (Context != null)
                        Context.TrollList.Remove(this);
                }
            }
        }
        [DataMember]
        public string Reason { get; set; }
        [DataMember]
        public int FucktardLevel { get; set; }
    }
}
