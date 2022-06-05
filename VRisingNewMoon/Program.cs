using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace VRisingNewMoon
{
    static class Program
    {
        static unsafe void Main()
        {
            if (Process.GetCurrentProcess().ProcessName == typeof(Program).Namespace)
            {
                var dllBytes = File.ReadAllBytes(Assembly.GetEntryAssembly().Location);
                NativeNetSharp.Inject("VRisingServer", dllBytes);
            }
            else
            {
                try
                {
                    NativeNetSharp.AllocConsole();
                    var standardOutput = new StreamWriter(new FileStream(new SafeFileHandle(NativeNetSharp.GetStdHandle(-11), true), FileAccess.Write), Encoding.GetEncoding(437)) { AutoFlush = true };
                    Console.SetOut(standardOutput);
                    LogSupport_TraceHandler("C# DLL loaded");
                    Setup();
                }
                catch (Exception e)
                {
                    LogSupport_TraceHandler(e.Message.ToString());
                    LogSupport_TraceHandler(e.StackTrace.ToString());
                }
            }
        }
        public unsafe static void Setup()
        {
            IL2CPP.il2cpp_thread_attach(IL2CPP.il2cpp_domain_get());
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Console.WriteLine(Environment.Version);
            Console.WriteLine(Application.unityVersion);
            Console.WriteLine(Directory.GetCurrentDirectory());
            UnityVersionHandler.Initialize(2020, 3, 31);
            LogSupport.RemoveAllHandlers();
            LogSupport.TraceHandler += LogSupport_TraceHandler;
            LogSupport.ErrorHandler += LogSupport_TraceHandler;
            LogSupport.InfoHandler += LogSupport_TraceHandler;
            LogSupport.WarningHandler += LogSupport_TraceHandler;
            //ClassInjector.DoHook?.GetInvocationList().ToList().ForEach(d => ClassInjector.DoHook -= (Action<IntPtr, IntPtr>)d);
            //ClassInjector.DoHook += NativeNetSharp.JmpPatch;
            ChatMessageSystemHook.Enable();
            Task.Run(WebManager.Start);
        }
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogSupport.Info(e.ToString());
            LogSupport.Info(e.ExceptionObject.ToString());
        }
        private static void LogSupport_TraceHandler(string obj)
        {
            //File.AppendAllText(@"log.txt", obj + "\n");
            Console.WriteLine(obj);
        }
    }
}