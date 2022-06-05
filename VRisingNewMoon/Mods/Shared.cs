using System.Linq;
using Unity.Entities;

namespace VRisingNewMoon
{
    public static class Shared
    {
        public static World World = World.s_AllWorlds.ToArray().ToList().First(w => w.Name == "Server");
    }
}
