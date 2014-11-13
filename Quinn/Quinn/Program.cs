using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using DevCommom;

namespace Quinn
{
    class Program
    {
        public const string Champion = "Quinn";
        public static Orbwalking.Orbwalker Orbwalker;
        public static Spell Q, W, E, R;
        public static Menu Config;
        private static Obj_AI_Hero Player;


        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += onGameLoad;
            
        }

        private static void onGameLoad(EventArgs args)
        {
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Game.OnGameUpdate += game_Update;
            Q = new Spell(SpellSlot.Q, 1010);
            E = new Spell(SpellSlot.E, 800);
            R = new Spell(SpellSlot.R, 550);

            

            Q.SetSkillshot(0.25f, 160f, 1150, true, SkillshotType.SkillshotLine);
            E.SetTargetted(0.25f, 2000f);


            Config = new Menu("Xcxooxl " + Champion, Champion, true);
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.AddItem(new MenuItem("UseQ", "USE Q in Combo?")).SetValue(true);
            Config.AddItem(new MenuItem("UseE", "Use E in Combo?")).SetValue(true);
            Config.AddItem(new MenuItem("UseR", "Use R in Combo?")).SetValue(true);
            Config.AddItem(new MenuItem("UseER", "Use ER when in valor mode?")).SetValue(true);

        }

        private static void game_Update(EventArgs args)
        {
            if (Orbwalker.ActiveMode.ToString().ToLower() == "combo")
            {
                Combo();
            }
            if (Orbwalker.ActiveMode.ToString().ToLower() == "Harras")
            {
                Harras();
            }
        }

        public static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && gapcloser.Sender.IsValidTarget(E.Range))
                E.CastOnUnit(gapcloser.Sender);
        }

        private int countEnemies()
        {
            return DevHelper.CountEnemyInPositionRange(Player.Position, 1100);
        }

        private static void Harras()
        {
            var vTarget = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
            if (Q.IsReady() && vTarget.IsValid)
            {
                Q.Cast(vTarget);
            }
        }

        private static void Combo()
        {
            var vTarget = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical);
            if (!IsValorMode() && R.IsReady() && vTarget.Distance(Player) <= E.Range + 200  && countEnemies() < 3)// probably will cancel ulti too fast.. need to make  state check like jackisback
            {
                R.Cast(true);
            }

            if (!IsValorMode())
            {
                if (E.IsReady())
                {
                    E.CastOnUnit(vTarget, true);
                }

                if (Q.IsReady())
                {
                    Q.Cast(vTarget, true);
                }
            }
            else // if IN valor mode do this
            {
                E.CastOnUnit(vTarget, true);

                if (Config.Item("UseER").GetValue<bool>())
                {
                    R.Cast();
                }

                if (Player.Distance(vTarget) <= 350)
                {
                    Q.Cast(true);
                }

                if (!E.IsReady() && !Q.IsReady() && Player.IsFacing(vTarget) && R.IsReady()) //is facing just so you wont cancel valor if you are trying to run away :S
                {
                    R.Cast();
                }
            }
        }

        private static bool IsValorMode()
        {
            return ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "QuinnRFinale";
        }
    }
}
