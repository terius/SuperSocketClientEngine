using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class SendMessage<T> where T : class, new()
    {
        // public int Length { get; set; }
        //  public string TimeStamp { get; set; }

        public T Data { get; set; }

        public int Action { get; set; }
    }

 
}
