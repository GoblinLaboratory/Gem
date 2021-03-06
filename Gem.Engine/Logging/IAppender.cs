﻿using System;

namespace Gem.Engine.Logging
{
    /// <summary>
    /// Logging facade
    /// </summary>
    public interface IAppender
    {
        void Message(string message);
        void Message(string message, params object[] args);

        void Info(string message);
        void Info(string message, params object[] args);

        void Debug(string message);
        void Debug(string message, params object[] args);

        void Warn(string message);
        void Warn(string message, params object[] args);

        void Error(string message);
        void Error(string message, params object[] args);

        void Fatal(string message);
        void Fatal(string message, params object[] args);        
    }
}
