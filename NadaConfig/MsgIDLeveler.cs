using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace NadaConfigServer
{
    class MsgIDLeveler
    {
        Queue<string> buffer;
        uint depth;

        public MsgIDLeveler():this(10)
        {
        }

        public MsgIDLeveler(uint depth)
        {
            buffer = new Queue<string>();
            this.depth = depth;
        }

        public bool IsMsgFirst(string msgID)
        {
            if (buffer.Contains(msgID))
                return false;
            else
            {
                if (buffer.Count > depth-1)
                    buffer.Dequeue();
                buffer.Enqueue(msgID);
                return true;
            }
        }

    }
}