using ProjectM;
using ProjectM.Network;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace VRisingNewMoon
{
    public static class ChatMessageSystemHook
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] unsafe delegate void OriginalLambdaBody(IntPtr _this, Entity* chatMessageEntity, ChatMessageEvent* chatMessage, FromCharacter* fromCharacter);
        unsafe static OriginalLambdaBody OriginalLambdaBody_Orig;
        unsafe static OriginalLambdaBody OriginalLambdaBody_Hook = new OriginalLambdaBody(OriginalLambdaBodyHook);
        static unsafe void OriginalLambdaBodyHook(IntPtr _this, Entity* chatMessageEntity, ChatMessageEvent* chatMessage, FromCharacter* fromCharacter)
        {
            var from = Shared.World.EntityManager.GetComponentData<User>(fromCharacter->User).CharacterName.ToString();
            Console.WriteLine("[" + chatMessage->MessageType + "] " + from + " : " + chatMessage->MessageText.ToString());
            OriginalLambdaBody_Orig(_this, chatMessageEntity, chatMessage, fromCharacter);
        }
        public unsafe static void Enable()
        {
            var OriginalLambdaBody = typeof(ChatMessageSystem).GetNestedTypes().First(t => t.Name.Contains("ChatMessageJob")).GetMethod("OriginalLambdaBody");
            var OriginalLambdaBodyPtr = UnhollowerUtils.ResolveFromMethodInfo(OriginalLambdaBody);
            OriginalLambdaBody_Orig = NativeNetSharp.Detour(OriginalLambdaBodyPtr, OriginalLambdaBody_Hook);
        }
    }
}