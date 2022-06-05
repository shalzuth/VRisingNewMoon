using ProjectM;
using ProjectM.Network;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using UnhollowerBaseLib;
using Unity.Collections;
using Unity.Transforms;
using Unity.Entities;
using UnhollowerRuntimeLib;

namespace VRisingNewMoon
{
    public class WebManager
    {
        static Semaphore sem = new Semaphore(16, 16);
        public static void Start()
        {
            var listener = new HttpListener();
            //listener.Prefixes.Add("https://*/");
            listener.Prefixes.Add("http://*/");
            listener.Start();
            Console.WriteLine("web manager started");
            while (true)
            {
                sem.WaitOne();
                try
                {
                    listener.GetContextAsync().ContinueWith(async (t) =>
                    {
                        sem.Release();
                        IL2CPP.il2cpp_thread_attach(IL2CPP.il2cpp_domain_get());
                        var context = await t;
                        var request = context.Request;
                        var response = context.Response;
                        try
                        {
                            var url = context.Request.RawUrl;
                            if (context.Request.HttpMethod == "POST")
                            {
                                if (url == "/dosomething")
                                {
                                    using (var body = request.InputStream)
                                    using (var reader = new StreamReader(body, request.ContentEncoding))
                                    {

                                    }
                                }
                            }
                            else
                            {
                                var urlComponents = url.Split(new String[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                                if (urlComponents[0] == "status")
                                {
                                    var userList = "";
                                    //var userQuery = Shared.World.EntityManager.CreateEntityQuery(new[] { ComponentType.ReadOnly<User>() });
                                    //var users = userQuery.ToEntityArray(Allocator.Persistent);
                                    var users = Shared.World.EntityManager.GetAllEntities(Allocator.Persistent);
                                    foreach (var e in users)
                                    {
                                        //Console.WriteLine(e.Index);
                                        if (Shared.World.EntityManager.HasComponent<User>(e))
                                        {
                                            var user = Shared.World.EntityManager.GetComponentData<User>(e);
                                            var location = Shared.World.EntityManager.GetComponentData<Translation>(e);
                                            var health = Shared.World.EntityManager.GetComponentData<UserHealth>(e);
                                            if (health.MaxHealth == 0) continue;
                                            userList += "placeMarker('" + user.CharacterName.ToString() + " " + Math.Round(health.Value) + "/" + Math.Round(health.MaxHealth) + "', " + location.Value.x + ", " + location.Value.z + ");";
                                        }
                                    }
                                    users.Dispose();
                                    var buffer = Encoding.UTF8.GetBytes("<!DOCTYPE html><html lang='en'><head><meta charset='utf-8'><meta name='viewport' content='width=device-width, initial-scale=1'><title>VRising Map</title> <link rel='stylesheet' href='https://unpkg.com/leaflet@1.8.0/dist/leaflet.css' integrity='sha512-hoalWLoI8r4UszCkZ5kL8vayOGVae1oxXe/2A4AO6J9+580uKHDO3JdHb7NzwwzK5xr/Fs0W40kiNHxM9vyTtQ==' crossorigin=''/> <script src='https://unpkg.com/leaflet@1.8.0/dist/leaflet.js' integrity='sha512-BB3hKbKWOc9Ez/TAwyWxNXeoV9c1v6FIeYiBieIWkpLjauysF18NzgR1MBNBXf8/KABdlkX68nAhlwcDFLGPCQ==' crossorigin=''></script><style>html, body{height: 100%;margin: 0;}.leaflet-container{height: 800px;width: 800px;max-width: 100%;max-height: 100%;}</style></head><body><div id='map' style='width: 800px; height: 800px;'></div><script>var map=L.map('map').setView([0.7, -0.7], 10);function placeMarker(name, pX, pY){var base=2915;var x=pX / base;var y=pY / base;L.marker([1.003 + y, -0.236 + x]).addTo(map).bindPopup(name,{closeOnClick: false, autoClose: false}).openPopup();}L.tileLayer('https://tiles.mapgenie.io/games/v-rising/vardoran/default-v1/{z}/{x}/{y}.jpg',{maxZoom: 15,minZoom: 8}).addTo(map);" + userList + "</script>Using mapgenie.io for tiles</body></html>");
                                    response.OutputStream.Write(buffer, 0, buffer.Length);
                                }
                            }
                        }
                        catch { }
                        response.OutputStream.Close();
                        return;
                    });

                }
                catch (Exception e)
                {
                    Console.WriteLine("error in webserver : " + e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }
        }
    }
}
