using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RepzScreenshot.Helper
{
    class UIHelper
    {
        
        public static string RemoveColor(string name)
        {
            return Regex.Replace(name, Regex.Escape("^") + "[0-9]|\\0", "");
        }

       
        public static string GetMapName(string name)
        {
            return GetFromDict(maps, name);
        }

        public static string GetGameTypeName(string name)
        {
            return GetFromDict(gametypes, name);
        }

        private static string GetFromDict(Dictionary<string, string> dict, string key)
        {
            string val = key;
            if (dict.ContainsKey(key))
            {
                val = dict[key];
            }
            return val;
        }




        #region dictionaries
        private static Dictionary<string, string> maps = new Dictionary<string, string>()
            {
               { "mp_afghan", "Afghan" },
               { "mp_derail", "Derail" },
               { "mp_estate", "Estate" },
               { "mp_favela", "Favela" },
               { "mp_highrise", "Highrise" },
               { "mp_invasion", "Invasion" },
               { "mp_checkpoint", "Checkpoint" },
               { "mp_quarry", "Quarry" },
               { "mp_rundown", "Rundown" },
               { "mp_rust", "Rust" },
               { "mp_boneyard", "Scrapyard" },
               { "mp_nightshift", "Skidrow" },
               { "mp_subbase", "Sub Base" },
               { "mp_terminal", "Terminal" },
               { "mp_underpass", "Underpass" },
               { "mp_brecourt", "Wasteland" },
                    //Stimulus
               { "mp_complex", "Bailout" },
               { "mp_crash", "Crash" },
               { "mp_overgrown", "Overgrown" },
               { "mp_compact", "Salvage" },
               { "mp_storm", "Storm" },
                    //Resurgence
               { "mp_abandon", "Carnival" },
               { "mp_fuel2", "Fuel" },
               { "mp_strike", "Strike" },
               { "mp_trailerpark", "Trailer Park" },
               { "mp_vacant", "Vacant" },
                    //Repz / other
               { "mp_nuked", "Nuketown" },
            };

        private static Dictionary<string, string> gametypes = new Dictionary<string, string>()
            {
               { "dm", "Free For All" },
               { "dom", "Domination" },
               { "sd", "Search & Destroy" },
               { "sab", "Sabotage" },
               { "war", "Team Deathmatch" },
               { "koth", "koth" },
               { "oneflag", "OneFlag" },

               { "ctf", "Capture The Flag" },
               { "gtnw", "Global Thermonuclear War" },
               { "oitc", "One in the chamber" },
               { "gg", "Gun Game" },
               { "ss", "Sharp Shooter" },
               { "killcon", "Kill Confirmed" },
               { "dzone", "Drop Zone" },

               { "m40a3", "M40A3" },
               { "snipe", "Snipe" },
            };
        #endregion //dictionaries
    }
}
