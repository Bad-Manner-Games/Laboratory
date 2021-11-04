using System.Collections.Generic;
using Laboratory.Debugging;
using Laboratory.Mods.Buttons;
using Laboratory.Mods.Effects.Utils;
using Laboratory.Mods.Enums;
using Laboratory.Mods.Systems;
using Laboratory.Utils;
using Reactor;
using UnityEngine;

namespace Laboratory.Mods
{
    public class ModDebugger : Debugger
    {
        public override IEnumerable<DebugTab> DebugTabs()
        {
            yield return new DebugTab("Mods", BuildUI);
        }
        
            
        private void BuildUI()
        {
            if (CameraZoomController.Instance != null)
            {
                CustomGUILayout.Label($"Camera Zoom: {CameraZoomController.Instance.OrthographicSize}");
                CameraZoomController.Instance.OrthographicSize = GUILayout.HorizontalSlider(CameraZoomController.Instance.OrthographicSize, 1f, 24f, DebugWindow.EmptyOptions);
            }

            CustomGUILayout.Button("Load Unity Explorer", () =>
            {
                RuntimePluginLoader.DownloadPlugin("UnityExplorer");
            });

            CustomGUILayout.Button("Test Button", () =>
            {
                CooldownButton.Create<EffectButton>().OnClickAction += () => Logger<ModPlugin>.Warning("TEST WARNING");
            });
            
            if (AmongUsClient.Instance.AmHost && ShipStatus.Instance)
            {
                List<(byte playerId, int newHealth)> list = new();
                var system = HealthSystem.Instance!;
                foreach ((var pid, var health) in system.PlayerHealths)
                {
                    GUILayout.Label(GameData.Instance.GetPlayerById(pid).PlayerName, DebugWindow.EmptyOptions);
                    var newHealth = Mathf.RoundToInt(GUILayout.HorizontalSlider(health, 0, HealthSystem.MaxHealth, DebugWindow.EmptyOptions));
                    if (newHealth != health) list.Add((pid, newHealth));
                }

                foreach ((var playerId, var newHealth) in list)
                {
                    system.SetHealth(playerId, newHealth);
                }
            }
        }
    }
}