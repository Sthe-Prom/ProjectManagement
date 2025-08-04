using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ProjectManagement.Models
{
    //DataContract for Serializing Data - required to serve in JSON format
    [DataContract]
    public class DataPointE
    {
        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "value")]
        public string Label = "";

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "name")]
        public string Y = null;

        public DataPointE(string y, string label)
        {
            this.Label = label;
            this.Y = y;
        }
    }
}