﻿using System;
using System.Diagnostics;

namespace BlazarTech.QueryableValues
{
    internal static class Util
    {
        public static void TraceError(string? message)
        {
            Trace.TraceError($"QueryableValues: {message}");
        }

        public static void TraceError(string? message, Exception? exception)
        {
            Trace.TraceError($"QueryableValues: {message}: {exception?.GetType().Name}: {exception?.Message}");
        }

        public static InvalidOperationException NewOnlyWorksWithSqlServerException()
        {
            return new InvalidOperationException("QueryableValues only works with a SQL Server provider.");
        }
    }
}
