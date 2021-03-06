﻿using System;
using System.Diagnostics;
using System.Threading;

namespace Dm.Auto.Testing.Core.Utils
{
    public static class Wait
    {
        public static void For(int timeout)
        {
            try
            {
                For(() => false, timeout);
            }
            catch (Exception)
            {
                //ignored
            }
        }

        public static void For(Func<bool> waitFunc, int timeout = 5000, string timeoutMessage = "Timed out")
        {
            For(waitFunc, () => timeoutMessage, timeout);
        }

        public static void For(Func<bool> waitFunc, Func<string> errorMessageFunc, int timeout = 5000)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                while (stopwatch.ElapsedMilliseconds < timeout)
                {
                    if (waitFunc())
                    {
                        return;
                    }
                    Thread.Sleep(50);
                }
                throw new TimeoutException(errorMessageFunc());
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        public static void UntilEquals<T>(T expected, Func<T> actual, int timeout = 5000)
        {
            For(() => expected.Equals(actual()), timeout, $"Value never changed to \"{expected}\"");
        }

        public static void UntilChanged<T>(T oldValue, Func<T> getNewValue, int timeout = 5000)
        {
            For(() => !oldValue.Equals(getNewValue()), timeout, $"Value \"{oldValue}\" never changed");
        }

        public static void UntilChanged<T>(Func<T> getValue, int timeout = 5000)
        {
            UntilChanged(getValue(), getValue, timeout);
        }

        public static void BecomeFalse(Func<bool> waitFunc, string errorMessage, int timeout = 5000)
        {
            For(() => !waitFunc(), timeout, errorMessage);
        }
    }
}