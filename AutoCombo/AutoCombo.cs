using System;
using System.Runtime.InteropServices;
using Evade;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Autocombo
{
    internal class Autocombo
    {
        public static Obj_AI_Hero Player;
        public Menu Config;
        public static Spell R;
        public static Spell _Q;
        public static Spell _W;
        public static Spell _E;
        public DamageSpell Allydamage;
        public DamageSpell Mydamage;
        public string sq;
        public string sw;
        public string se;



        public Autocombo()
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private void OnGameLoad(EventArgs args)
        {
            Obj_AI_Base.OnProcessSpellCast += ObjAiBaseOnProcessSpellCast;
            Player = ObjectManager.Player;
            Config = new Menu("Auto Sick Combo", "AutoCombo", true);
            Config.AddToMainMenu();
            Config.AddSubMenu(new Menu("AutoCombo Settings", "AutoCombo"));
            Config.SubMenu("AutoCombo").AddItem(new MenuItem("Killable", "Combo Only Killable?").SetValue(true));

           
            
            
            Game.PrintChat("<font color='#F7A100'>Auto Combo by XcxooxL Loaded 1.0 .</font>");
            Game.PrintChat("<font color='#F7A100'>Credits to Diabaths and Pingo for helping me test =] </font>");
            
            sq = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.Name;
            sw = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.Name;
            se = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).SData.Name;
            var dataQ = SpellDatabase.GetByName(sq);
            var dataW = SpellDatabase.GetByName(sw);
            var dataE = SpellDatabase.GetByName(se);

            if (dataQ != null)
            {
                Config.SubMenu("AutoCombo").AddItem(new MenuItem("SKSQ", "Use Q?").SetValue(true));
                _Q = new Spell(SpellSlot.Q, dataQ.Range);
                if (_Q.IsSkillshot)
                {
                    _Q.SetSkillshot(dataQ.Delay, dataQ.Radius, dataQ.MissileSpeed, true, SkillshotType.SkillshotLine);
                }
                else
                {
                    _Q.SetTargetted(dataQ.Delay, dataQ.MissileSpeed);
                }
            }

            if (dataW != null)
            {
                Config.SubMenu("AutoCombo").AddItem(new MenuItem("SKSW", "Use W?").SetValue(true));
                _W = new Spell(SpellSlot.W, dataW.Range);
                if (_W.IsSkillshot)
                {
                    _W.SetSkillshot(dataW.Delay, dataW.Radius, dataW.MissileSpeed, true, SkillshotType.SkillshotLine);
                }
                else
                {
                    _W.SetTargetted(dataW.Delay, dataW.MissileSpeed);
                }

            }
            if (dataE != null)
            {
                Config.SubMenu("AutoCombo").AddItem(new MenuItem("SKSE", "Use E?").SetValue(true));
                _E = new Spell(SpellSlot.E, SpellDatabase.GetByName(se).Range);

                if (_E.IsSkillshot)
                {
                    _E.SetSkillshot(dataE.Delay, dataE.Radius, dataE.MissileSpeed, true, SkillshotType.SkillshotLine);
                }
                else
                {
                    _E.SetTargetted(dataE.Delay, dataE.MissileSpeed);
                }
                
            }
            string[] champions = { "Ezreal", "Lux", "Ashe", "Draven", "Fizz", "Graves", "Riven", "Sona", "Jinx", "Caitlyn", "Riven" };

            for (int i = 0; i <= 9; i++)
            {
                if (Player.BaseSkinName == champions[i])
                {
                    Game.PrintChat("<font color='#F7A100'>Champion : " + champions[i] + " Detected And Loaded !!" + " .</font>");
                }
            }

            if (Player.BaseSkinName == "Ezreal")
            {
                R = new Spell(SpellSlot.R, 2000);
                R.SetSkillshot(0.25f, 150f, 2000f, false, SkillshotType.SkillshotLine);
            }
            if (Player.BaseSkinName == "Lux")
            {
                R = new Spell(SpellSlot.R, 3200);
                R.SetSkillshot(0.25f, 150f, 3000f, false, SkillshotType.SkillshotLine);
            }
            if (Player.BaseSkinName == "Ashe")
            {
                R = new Spell(SpellSlot.R, 2000);
                R.SetSkillshot(0.25f, 130f, 1600f, false, SkillshotType.SkillshotLine);
            }
            if (Player.BaseSkinName == "Draven")
            {
                R = new Spell(SpellSlot.R, 2000);
                R.SetSkillshot(0.25f, 120f, 2000f, false, SkillshotType.SkillshotLine);
            }
            if (Player.BaseSkinName == "Fizz")
            {
                R = new Spell(SpellSlot.R, 1275);
                R.SetSkillshot(0.25f, 80f, 1200f, false, SkillshotType.SkillshotLine);
            }
            if (Player.BaseSkinName == "Graves")
            {
                R = new Spell(SpellSlot.R, 1000);
                R.SetSkillshot(0.25f, 100f, 1400f, false, SkillshotType.SkillshotLine);
            }
            if (Player.BaseSkinName == "Sona")
            {
                R = new Spell(SpellSlot.R, 2400);
                R.SetSkillshot(0.25f, 140f, 1400f, false, SkillshotType.SkillshotLine);
            }
            if (Player.BaseSkinName == "Jinx")
            {
                R = new Spell(SpellSlot.R, 2000);
                R.SetSkillshot(0.25f, 120f, 1400f, false, SkillshotType.SkillshotLine);
            }
            if (Player.BaseSkinName == "Caitlyn")
            {
                R = new Spell(SpellSlot.R, 2000);
                R.SetTargetted(0.5f, 2000, Player.Position);
            }
            if (Player.BaseSkinName == "Riven")
            {
                R = new Spell(SpellSlot.R, 1100);
                R.SetSkillshot(0.25f, 125, 2200, false, SkillshotType.SkillshotCone);
            }
        }

        static Vector2 V2E(Vector3 from, Vector3 direction, float distance)
        {
            return from.To2D() + distance * Vector3.Normalize(direction - from).To2D();
        }

        public void ObjAiBaseOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var spellData = SpellDatabase.GetByName(args.SData.Name);
            if (sender.IsAlly && !sender.IsMinion && !sender.IsMe && !sender.IsAutoAttacking)
            {
                string[] spelllist = {"EzrealTrueshotBarrage", "LuxMaliceCannon", "UFSlash", "InfernalGuardian", "EnchantedCrystalArrow", "DravenRCast", "FizzMarinerDoom", "GravesChargeShot",
 "LeonaSolarFlare", "RivenFengShuiEngine", "SejuaniGlacialPrisonStart", "shyvanatransformcast", "SonaCrescendo", "XerathArcaneBarrageWrapper", "ZiggsR",
 "CaitlynAceintheHole", "JinxRWrapper"};

                for (int i = 0; i <= 18; i++)
                {
                    if (args.SData.Name == spelllist[i])
                    {
                        //var spellData = SpellDatabase.GetByName(args.SData.Name);

                        if (spellData.ChampionName == "Caitlyn")
                        {
                            var diab = (Obj_AI_Base)args.Target;
                            Allydamage = sender.GetDamageSpell(diab, args.SData.Name);
                            Mydamage = Player.GetDamageSpell(diab, SpellSlot.R);

                            if (Config.Item("Killable").GetValue<bool>())
                            {
                                if ((Mydamage.CalculatedDamage + Allydamage.CalculatedDamage) < diab.Health || Allydamage.CalculatedDamage > diab.Health)
                                {
                                    return;
                                }
                            }
                            Game.PrintChat("Spell Detected: " + args.SData.Name + " By: " + sender.BaseSkinName +
                                           " Ally Casted it On : " + diab.BaseSkinName); //Checks..
                            if (diab.Distance(Player.Position) < 3200 && diab.Distance(Player.Position) <= R.Range)
                            {
                                if (R.IsSkillshot)
                                {
                                    R.Cast(diab, true);
                                }
                                else
                                {
                                    R.CastOnUnit(diab, true);
                                }
                            }
                        }
                        else
                        {
                            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>())
                            {
                                //   PredictionOutput allyOutput = Prediction.GetPrediction(enemy, 500, args.SData.LineWidth, args.SData.MissileSpeed);

                                /*
                                Game.PrintChat(sender.BaseSkinName + " Ultimate Damage is : " + Allydamage.CalculatedDamage);
                                Game.PrintChat("My Ultimate Damage is : " + Mydamage.CalculatedDamage);
                                Game.PrintChat("Total damage is : " + (Allydamage.CalculatedDamage + Mydamage.CalculatedDamage));
                                */


                                if (Config.Item("Killable").GetValue<bool>())
                                {
                                    Allydamage = sender.GetDamageSpell(enemy, args.SData.Name);
                                    Mydamage = Player.GetDamageSpell(enemy, SpellSlot.R);

                                    if ((Allydamage.CalculatedDamage + Mydamage.CalculatedDamage) < enemy.Health && Allydamage.CalculatedDamage > enemy.Health)
                                    {
                                        return;
                                    }
                                }

                                if (enemy.IsEnemy && enemy.Distance(V2E(args.Start, args.End, enemy.Distance(sender.Position))) <= (spellData.Radius - 50))
                                {
                            //better? yes, more readable not? yea :D
                                    Game.PrintChat("SkillShot Detected: " + args.SData.Name + " By: " + sender.BaseSkinName +
                                                       " Ally Casted it right.. On : " + enemy.BaseSkinName); //Checks..
                                    if (enemy.Distance(Player.Position) < 3200 && enemy.Distance(Player.Position) <= R.Range)
                                    {
                                        if (Player.BaseSkinName == "Riven")
                                        {
                                            R.Cast(false);
                                        }
                                        if (!R.IsSkillshot) // Casting for targetable spells
                                        {
                                            R.CastOnUnit(enemy, true);
                                        }
                                        else
                                        {
                                            R.Cast(enemy, true);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                Game.PrintChat("YEA");
                // this part is new.. anyway you should well indent the code ^^ indent? like Organize? yes :) is there any software for that? lol not at all
                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>())
                {
                    
                    Allydamage = sender.GetDamageSpell(enemy, args.SData.Name);
                    if (enemy.IsEnemy &&
                        enemy.Distance(V2E(args.Start, args.End, enemy.Distance(sender.Position))) <=
                        (spellData.Radius - 50))
                    {
                        
                        if ((Config.Item("SKSQ").GetValue<bool>()))
                        {
                            Mydamage = Player.GetDamageSpell(enemy, SpellSlot.Q);

                            if ((Allydamage.CalculatedDamage + Mydamage.CalculatedDamage) < enemy.Health &&
                                Allydamage.CalculatedDamage > enemy.Health)
                            {
                                return;
                            }
                            _Q.Cast(enemy, true);
                        }

                        if ((Config.Item("SKSW").GetValue<bool>()))
                        {
                            Mydamage = Player.GetDamageSpell(enemy, SpellSlot.W);

                            if ((Allydamage.CalculatedDamage + Mydamage.CalculatedDamage) < enemy.Health &&
                                Allydamage.CalculatedDamage > enemy.Health)
                            {
                                return;
                            }
                            _W.Cast(enemy, true);
                        }

                        if ((Config.Item("SKSE").GetValue<bool>()))
                        {
                            Mydamage = Player.GetDamageSpell(enemy, SpellSlot.E);

                            if ((Allydamage.CalculatedDamage + Mydamage.CalculatedDamage) < enemy.Health &&
                                Allydamage.CalculatedDamage > enemy.Health)
                            {
                                return;
                            }
                            _E.Cast(enemy, true);
                        }
                    }
                }
            }
        }
    }
}