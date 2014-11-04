using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;


namespace TeemoAntiGap
{
    class Program
    {
        public static Obj_AI_Hero Player;
        public static Spell _R;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args)
        {
            _R = new Spell(SpellSlot.R, 300);
            Player = ObjectManager.Player;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
        }

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Player.Distance(gapcloser.End) <= 300)
            {
                if (Player.IsFacing(gapcloser.Sender))
                {
                    _R.Cast(gapcloser.End, true);
                }
                else
                {
                    _R.Cast(Player.Position + (Player.Direction * 150), true);
                }
            }
        }
    }
}
