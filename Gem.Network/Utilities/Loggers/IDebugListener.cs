﻿using System;

namespace Gem.Network.Utilities.Loggers
{
    public interface IDebugListener
    {
        void RegisterAppender(IAppender appender);
        void DeregisterAppender(IAppender appender);
    }
}
