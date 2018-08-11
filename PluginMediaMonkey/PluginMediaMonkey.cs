﻿using System;
using System.Runtime.InteropServices;
using Rainmeter;

namespace PluginMediaMonkey
{
    public class Plugin
    {
        //private static MediaMonkey mm;

        [DllExport]
        public static void Initialize(ref IntPtr data, IntPtr rm)
        {
            data = GCHandle.ToIntPtr(GCHandle.Alloc(new Measure()));
            API api = (Rainmeter.API)rm;
            api.LogF(API.LogType.Warning, "MediaMonkey:init");

            //mm = new MediaMonkey();
            //try
            //{
            //    mm.Session = new MediaMonkeySession();
            //    //mm.Session.Player.RefreshAsync().GetAwaiter;
            //    mm.TempConnect();
            //}
            //catch (Exception ex)
            //{
            //    API.LogF(rm, API.LogType.Error, "MediaMonkey:error: {0}", ex.InnerException.Message);
            //}
        }

        [DllExport]
        public static void Finalize(IntPtr data)
        {
            Measure measure = (Measure)data;
            if (measure.buffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(measure.buffer);
            }
            GCHandle.FromIntPtr(data).Free();
        }

        [DllExport]
        public static void Reload(IntPtr data, IntPtr rm, ref double maxValue)
        {
            var api = (API)rm;
            var measure = (Measure)data;
            api.LogF(API.LogType.Warning, "MediaMonkey.dll:reload");

            string inputType = api.ReadString("PlayerType", "", false);
            Measure.MeasureType parsedMeasure;
            bool measureParseSuccess = Enum.TryParse(inputType, true, out parsedMeasure);

            if (!measureParseSuccess)
            {
                api.LogF(API.LogType.Error, "MediaMonkey.dll: Measure type=" + inputType + " not valid");
                return;
            }

            measure.Reload(parsedMeasure);

            api.LogF(API.LogType.Warning, "MediaMonkey.dll:reload done");
        }

        [DllExport]
        public static double Update(IntPtr data)
        {
            Measure measure = (Measure)data;
            return measure.Update();
        }

        [DllExport]
        public static IntPtr GetString(IntPtr data)
        {
            Measure measure = (Measure)data;
            if (measure.buffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(measure.buffer);
                measure.buffer = IntPtr.Zero;
            }

            string measureString = measure.GetString();
            if(measureString != null)
            {
                measure.buffer = Marshal.StringToHGlobalUni(measureString);
            }

            return measure.buffer;
        }

        [DllExport]
        public static void ExecuteBang(IntPtr data, [MarshalAs(UnmanagedType.LPWStr)]String args)
        {
            Measure measure = (Measure)data;
        }

        //[DllExport]
        //public static IntPtr (IntPtr data, int argc,
        //    [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1)] string[] argv)
        //{
        //    Measure measure = (Measure)data;
        //    if (measure.buffer != IntPtr.Zero)
        //    {
        //        Marshal.FreeHGlobal(measure.buffer);
        //        measure.buffer = IntPtr.Zero;
        //    }
        //
        //    measure.buffer = Marshal.StringToHGlobalUni("");
        //
        //    return measure.buffer;
        //}
    }
}

