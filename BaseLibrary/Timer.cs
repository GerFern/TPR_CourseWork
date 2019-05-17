using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibrary
{
    /// <summary>
    /// Представляет объект таймер, показывающей пройденное время с возможностью приостановки времени
    /// </summary>
    public class Timer
    {
        TimeSpan deltaTime;
        DateTime resumeTime;
        DateTime suspendTime;

        /// <summary>
        /// Пройденное время (во время отладки таймер продолжает работать!)
        /// </summary>
        public TimeSpan TimeSpent => IsInit ? IsResume ? DateTime.Now - resumeTime + deltaTime :
                                                     deltaTime :
                                            TimeSpan.Zero;

        /// <summary>
        /// Инициализирован ли таймер
        /// </summary>
        public bool IsInit { get; private set; } = false;
        /// <summary>
        /// Действует ли таймер. Если <see langword="true"/>, то пройденное время увеличивается
        /// </summary>
        public bool IsResume { get; private set; } = false;

        /// <summary>
        /// Запустить таймер
        /// </summary>
        /// <param name="reset">Нужно ли сбрасывать таймер, если запущен</param>
        public void Start(bool reset = true)
        {
            if (!reset && IsInit) return;
            IsInit = true;
            IsResume = true;
            deltaTime = TimeSpan.Zero;
            suspendTime = resumeTime = DateTime.Now;
        }

        /// <summary>
        /// Приостановить таймер
        /// </summary>
        public void Suspend()
        {
            if (IsResume)
            {
                IsResume = false;
                deltaTime += DateTime.Now - resumeTime;
                suspendTime = DateTime.Now;
            }
        }

        /// <summary>
        /// Возобновить таймер
        /// </summary>
        public void Resume()
        {
            if (IsInit)
            {
                if (!IsResume)
                {
                    IsResume = true;
                    resumeTime = DateTime.Now;
                }
            }
            else
                Start();
        }

        /// <summary>
        /// Сбросить таймер
        /// </summary>
        public void Reset()
        {
            IsInit = false;
        }
    }
}
