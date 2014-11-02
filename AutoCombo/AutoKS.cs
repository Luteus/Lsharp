﻿using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evade;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Autocombo
{
    internal class AutoKS
    {
        public static Obj_AI_Hero Player;
        public Menu Config;
        public static Spell _Q;
        public static Spell _W;
        public static Spell _E;
        public DamageSpell Allydamage;
        public DamageSpell Mydamage;
        public string sq;
        public string sw;
        public string se;

        public AutoKS()
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
            Obj_AI_Base.OnProcessSpellCast += ObjAiBaseOnProcessSpellCast;
        }

        private void OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;

            sq = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.Name;
            sw = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.Name;
            se = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).SData.Name;
            var dataQ = SpellDatabase.GetByName(sq);
            var dataW = SpellDatabase.GetByName(sw);
            var dataE = SpellDatabase.GetByName(se);
            Config = new Menu("Auto KillSecure", "AutoCombo", true);
            Config.AddToMainMenu();
            Config.AddSubMenu(new Menu("AutoKillSteal Settings", "AutoCombo"));

            if (dataQ != null)
            {
                Config.SubMenu("AutoCombo").AddItem(new MenuItem("SKSQ", "Use Q?").SetValue(true));
                _Q = new Spell(SpellSlot.Q, dataQ.Range);
                if (_Q.IsSkillshot)
                {
                    _Q.SetSkillshot(dataQ.Delay / 1000f, dataQ.Radius, dataQ.MissileSpeed, true, (SkillshotType)dataQ.Type);
                }
                else
                {
                    _Q.SetTargetted(dataQ.Delay / 1000f, dataQ.MissileSpeed);
                }
            }

            if (dataW != null)
            {
                Config.SubMenu("AutoCombo").AddItem(new MenuItem("SKSW", "Use W?").SetValue(true));
                _W = new Spell(SpellSlot.W, dataW.Range);
                if (_W.IsSkillshot)
                {
                    _W.SetSkillshot(dataW.Delay / 1000f, dataW.Radius, dataW.MissileSpeed, false, (SkillshotType)dataW.Type);
                }
                else
                {
                    _W.SetTargetted(dataW.Delay / 1000f, dataW.MissileSpeed);
                }

            }
            if (dataE != null)
            {
                Config.SubMenu("AutoCombo").AddItem(new MenuItem("SKSE", "Use E?").SetValue(true));
                _E = new Spell(SpellSlot.E, SpellDatabase.GetByName(se).Range);

                if (_E.IsSkillshot)
                {
                    _E.SetSkillshot(dataE.Delay / 1000f, dataE.Radius, dataE.MissileSpeed, true, (SkillshotType)dataE.Type);
                }
                else
                {
                    _E.SetTargetted(dataE.Delay / 1000f, dataE.MissileSpeed);
                }

            }
        }
        static Vector2 V2E(Vector3 from, Vector3 direction, float distance)
        {
            return from.To2D() + distance * Vector3.Normalize(direction - from).To2D();
        }

        private void ObjAiBaseOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsAlly && !sender.IsMinion)
            {
                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>())
                {
                    if (args.SData.Name.ToLower().Contains("attack") && !args.Target.Name.ToLower().Contains("minion") && !args.Target.Name.ToLower().Contains("light") || enemy.IsEnemy &&
                        enemy.Distance(V2E(args.Start, args.End, enemy.Distance(sender.Position))) <=
                        (args.SData.LineWidth))
                    {
                        Allydamage = sender.GetDamageSpell(enemy, args.SData.Name);
                        Game.PrintChat("WillHit : " + "Spell Name : " + args.SData.Name + " Damage : " + Allydamage.CalculatedDamage + "Target Name : " + args.Target.Name);

                        if ((Config.Item("SKSQ").GetValue<bool>()))
                        {
                            Mydamage = Player.GetDamageSpell(enemy, SpellSlot.Q);
                            if ((Allydamage.CalculatedDamage + Mydamage.CalculatedDamage) > enemy.Health &&
                                Allydamage.CalculatedDamage < enemy.Health)
                            {
                                Game.PrintChat("YEAAAAAAAAAAAAAAAAAA");
                                _Q.Cast(enemy, true);
                            }
                        }

                        if ((Config.Item("SKSW").GetValue<bool>()))
                        {
                            Mydamage = Player.GetDamageSpell(enemy, SpellSlot.W);

                            if ((Allydamage.CalculatedDamage + Mydamage.CalculatedDamage) > enemy.Health &&
                                Allydamage.CalculatedDamage < enemy.Health)
                            {
                                _W.Cast(enemy, true);
                            }

                        }

                        if ((Config.Item("SKSE").GetValue<bool>()))
                        {
                            Mydamage = Player.GetDamageSpell(enemy, SpellSlot.E);

                            if ((Allydamage.CalculatedDamage + Mydamage.CalculatedDamage) > enemy.Health &&
                                Allydamage.CalculatedDamage < enemy.Health)
                            {
                                _E.Cast(enemy, true);
                            }

                        }
                    }
                }
            }

        }
    }

}
