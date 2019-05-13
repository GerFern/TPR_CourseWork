using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibrary
{
    public class Timer
    {
        TimeSpan deltaTime;
        DateTime resumeTime;
        DateTime suspendTime;
        bool resume = false;
        bool init = false;

        public TimeSpan TimeSpent => init ? resume ? DateTime.Now - resumeTime + deltaTime :
                                                     deltaTime :
                                            TimeSpan.Zero;
        public void Start()
        {
            init = true;
            resume = true;
            deltaTime = TimeSpan.Zero;
            suspendTime = resumeTime = DateTime.Now;
        }

        public void Suspend()
        {
            resume = false;
            deltaTime += DateTime.Now - resumeTime;
            suspendTime = DateTime.Now;
        }

        public void Resume()
        {
            if (init)
            {
                resume = true;
                resumeTime = DateTime.Now;
            }
            else
                Start();
        }

        public void Reset()
        {
            init = false;
        }
    }
}
