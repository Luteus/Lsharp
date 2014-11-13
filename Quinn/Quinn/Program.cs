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
        public static Spell Q, E, R;
        public static Menu Config;
        private static Obj_AI_Hero Player;


        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;

            Q = new Spell(SpellSlot.Q, 1010);
            E = new Spell(SpellSlot.E, 800);
            R = new Spell(SpellSlot.R, 550);

            Q.SetSkillshot(0.25f, 80f, 1150, true, SkillshotType.SkillshotLine);
            E.SetTargetted(0.25f, 2000f);

            Config = new Menu("Xcxooxl", "Xcxooxl", true);
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQ", "Use Q?").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseE", "Use E?").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseR", "Use R?").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("WaitH", "Dont E if target has Harrier").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseER", "Use ER valor mode (burst)").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Double", "Double Harrier").SetValue(true));
            Config.AddToMainMenu();

            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Game.OnGameUpdate += game_Update;

        }


        private static void game_Update(EventArgs args)
        {
            if (Orbwalker.ActiveMode.ToString().ToLower() == "combo")
            {
                Combo();
            }
            if (Orbwalker.ActiveMode.ToString().ToLower() == "mixed")
            {
                Harras();
            }
        }

        public static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && gapcloser.Sender.IsValidTarget(E.Range))
                E.CastOnUnit(gapcloser.Sender);
        }

        private static int countEnemies()
        {
            return DevHelper.CountEnemyInPositionRange(Player.Position, 1100);
        }

        private static void Harras()
        {
            var vTarget = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
            if (Q.IsReady() && Config.Item("UseQH").GetValue<bool>())
            {
                Q.Cast(vTarget);
            }
        }

        private static void Combo()
        {

            var vTarget = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical);

            if (!IsValorMode() && R.IsReady() && vTarget.Distance(Player) <= E.Range + 200 && countEnemies() < 3 && Config.Item("UseR").GetValue<bool>())// probably will cancel ulti too fast.. need to make  state check like jackisback
            {
                R.Cast(true);
            }

            if (!IsValorMode())
            {
                if (Config.Item("Double").GetValue<bool>() && E.IsReady() && Config.Item("UseE").GetValue<bool>())
                {
                    if (!vTarget.HasBuff("QuinnW"))
                    {
                        E.CastOnUnit(vTarget, true);
                    }
                }
                if (!Config.Item("Double").GetValue<bool>() && Config.Item("UseE").GetValue<bool>())
                {
                    E.CastOnUnit(vTarget, true);
                }
                if (Q.IsReady() && Config.Item("UseQ").GetValue<bool>())
                {
                    Q.Cast(vTarget, true);
                }
            }
            else // if IN valor mode do this
            {

                if (Config.Item("UseER").GetValue<bool>()) // will use E after R to return to human form
                {
                    E.CastOnUnit(vTarget, true);
                    if (vTarget.Distance(Player) <= 400)
                    {
                        R.Cast(true);
                    }
                    
                }
                else if (Config.Item("UseE").GetValue<bool>() && E.IsReady())
                {
                    E.CastOnUnit(vTarget);
                }

                if (Player.Distance(vTarget) <= 350 && Config.Item("UseQ").GetValue<bool>() && Q.IsReady())
                {
                    Q.Cast(true);
                }

                if (!E.IsReady() && !Q.IsReady() && Player.IsFacing(vTarget) && vTarget.IsFacing(Player) && R.IsReady()) //is facing just so you wont cancel valor if you are trying to run away :S
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
