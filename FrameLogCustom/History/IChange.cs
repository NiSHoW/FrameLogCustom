﻿using FrameLog.Models;
using System;
using System.Collections.Generic;

namespace FrameLog.History
{
    public interface IChange<TValue, TPrincipal>
    {
        DateTimeOffset TimestampDate { get; }
        TPrincipal Author { get; }
        TValue Value { get; }

        IObjectChange<TPrincipal> ObjectChange { get; }
        
        IEnumerable<Exception> Errors { get; }
        bool ProblemsRetrievingData { get; }
    }
}
