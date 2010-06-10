using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using System.Text;

using Bramble.Core;

using Microsoft.CSharp;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public class ItemScript
    {
        /// <summary>
        /// Creates a new ItemScript that executes the given C# script when used.
        /// </summary>
        public static ItemScript Create(string script)
        {
            // lazy compile it
            UncompiledScript uncompiled = new UncompiledScript(script);
            sUncompiledScripts.Add(uncompiled);

            return uncompiled.Script;
        }

        public bool Invoke(Entity user, Item item, Action action, Vec? target)
        {
            // lazy compile if needed
            if (mWrapper == null)
            {
                CompileScripts();
            }

            return mWrapper.Invoke(user, item, action, target);
        }

        private static void CompileScripts()
        {
            string code = GenerateScriptClasses();

            string[] referencedAssemblies = new string[]
                {
                    "mscorlib.dll",
                    "Amaranth.Engine.dll",
                    "Amaranth.Util.dll"
                };

            CompilerParameters parameters = new CompilerParameters(referencedAssemblies);
            CSharpCodeProvider provider = new CSharpCodeProvider();

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, code);

            if (results.Errors.Count > 0)
            {
                Console.WriteLine(code);
                foreach (CompilerError error in results.Errors)
                {
                    Console.WriteLine(error.ToString());
                }
            }

            Assembly assembly = results.CompiledAssembly;

            // bind the existing ItemUses to their compiled scripts
            foreach (UncompiledScript script in sUncompiledScripts)
            {
                Type type = assembly.GetType("Amaranth.Engine.Compiled." + script.ClassName);

                ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                ItemScriptWrapper wrapper = (ItemScriptWrapper)constructor.Invoke(new object[0]);

                script.Script.mWrapper = wrapper;
            }

            // all compiled now
            sUncompiledScripts.Clear();
        }

        private static string GenerateScriptClasses()
        {
            StringBuilder code = new StringBuilder();

            code.AppendLine("using System;");
            code.AppendLine("using System.Collections.Generic;");
            code.AppendLine("using System.Text;");
            code.AppendLine("");

            code.AppendLine("");
            code.AppendLine("namespace Amaranth.Engine.Compiled");
            code.AppendLine("{");

            foreach (UncompiledScript script in sUncompiledScripts)
            {
                code.AppendLine("    public class " + script.ClassName + " : ItemScriptWrapper");
                code.AppendLine("    {");
                code.AppendLine("        protected override bool Use()");
                code.AppendLine("        {");
                code.AppendLine("            // script:");
                code.AppendLine("            " + script.SourceCode);
                code.AppendLine("");
                code.AppendLine("            return true;");
                code.AppendLine("        }");
                code.AppendLine("    }");
            }

            code.AppendLine("}");

            return code.ToString();
        }

        /// <summary>
        /// Can only be constructed by ItemUse.Create().
        /// </summary>
        private ItemScript()
        {
        }

        private class UncompiledScript
        {
            public string ClassName
            {
                get { return "ItemScript_" + Index; }
            }

            public ItemScript Script;
            public int Index;
            public string SourceCode;

            public UncompiledScript(string sourceCode)
            {
                Index = sNextScriptIndex++;
                SourceCode = sourceCode;

                // if the code doesn't have an ending ;, add one
                if (!SourceCode.EndsWith(";"))
                {
                    SourceCode += ";";
                }

                Script = new ItemScript();
            }

            private static int sNextScriptIndex = 1;
        }

        private static List<UncompiledScript> sUncompiledScripts = new List<UncompiledScript>();

        private ItemScriptWrapper mWrapper;
    }

    public abstract class ItemScriptWrapper
    {
        public bool Invoke(Entity user, Item item, Action action, Vec? target)
        {
            mEntity = user;
            mItem = item;
            mAction = action;
            mTarget = target;

            return Use();
        }

        protected abstract bool Use();

        #region Base Item uses

        protected Item Item { get { return mItem; } }

        //### bob: should check type
        protected Hero Hero { get { return (Hero)mEntity; } }

        protected void GainHealth()                     { mAction.AddAction(new GainHealthAction(mEntity, Item.Attack)); }
        protected void Heal()                           { mAction.AddAction(new HealAction(mEntity, Item.Attack)); }
        protected void HealFull()                       { mAction.AddAction(new HealFullAction(mEntity)); }

        protected void Teleport(int distance)           { mAction.AddAction(new TeleportAction(mEntity, distance)); }
        protected void MakeTownPortal()                 { mAction.AddAction(new CreatePortalAction(mEntity)); }
        protected void Haste(int boost)                 { mAction.AddAction(new HasteAction(mEntity, Item.Attack.Roll(), boost)); }
        protected void Light(int radius, string noun)   { mAction.AddAction(new LightAction(mEntity, new Noun(noun), radius, Item.Attack)); }
        protected void Explode(int radius)              { mAction.AddAction(new ExplodeAction(mEntity, mItem, radius)); }

        protected void DetectFeatures()                 { mAction.AddAction(new DetectFeaturesAction(mEntity)); }
        protected void DetectItems()                    { mAction.AddAction(new DetectItemsAction(mEntity)); }

        protected void CurePoison()                     { mAction.AddAction(new CurePoisonAction(mEntity)); }
        protected void CureDisease()                    { mAction.AddAction(new CureDiseaseAction(mEntity)); }

        protected void RestoreAll()                     { mAction.AddAction(new RestoreAllAction(Hero)); }
        protected void RestoreStrength()                { mAction.AddAction(new RestoreAction(Hero, Hero.Stats.Strength)); }
        protected void RestoreAgility()                 { mAction.AddAction(new RestoreAction(Hero, Hero.Stats.Agility)); }
        protected void RestoreStamina()                 { mAction.AddAction(new RestoreAction(Hero, Hero.Stats.Stamina)); }
        protected void RestoreWill()                    { mAction.AddAction(new RestoreAction(Hero, Hero.Stats.Will)); }
        protected void RestoreIntellect()               { mAction.AddAction(new RestoreAction(Hero, Hero.Stats.Intellect)); }
        protected void RestoreCharisma()                { mAction.AddAction(new RestoreAction(Hero, Hero.Stats.Charisma)); }

        protected void GainStrength()                   { mAction.AddAction(new GainStatAction(Hero, Hero.Stats.Strength)); }
        protected void GainAgility()                    { mAction.AddAction(new GainStatAction(Hero, Hero.Stats.Agility)); }
        protected void GainStamina()                    { mAction.AddAction(new GainStatAction(Hero, Hero.Stats.Stamina)); }
        protected void GainWill()                       { mAction.AddAction(new GainStatAction(Hero, Hero.Stats.Will)); }
        protected void GainIntellect()                  { mAction.AddAction(new GainStatAction(Hero, Hero.Stats.Intellect)); }
        protected void GainCharisma()                   { mAction.AddAction(new GainStatAction(Hero, Hero.Stats.Charisma)); }

        protected void GainAll()
        {
            GainStrength();
            GainAgility();
            GainStamina();
            GainWill();
            GainIntellect();
            GainCharisma();
        }

        protected void SwapStrength()                   { mAction.AddAction(new SwapStatAction(Hero, Hero.Stats.Strength)); }
        protected void SwapAgility()                    { mAction.AddAction(new SwapStatAction(Hero, Hero.Stats.Agility)); }
        protected void SwapStamina()                    { mAction.AddAction(new SwapStatAction(Hero, Hero.Stats.Stamina)); }
        protected void SwapWill()                       { mAction.AddAction(new SwapStatAction(Hero, Hero.Stats.Will)); }
        protected void SwapIntellect()                  { mAction.AddAction(new SwapStatAction(Hero, Hero.Stats.Intellect)); }
        protected void SwapCharisma()                   { mAction.AddAction(new SwapStatAction(Hero, Hero.Stats.Charisma)); }
        
        protected void Bolt(string noun)
        {
            if (!mTarget.HasValue) throw new InvalidOperationException("Cannot use the Bolt() script with Items that do not have a target.");

            mAction.AddAction(new ElementBoltAction(mEntity, mTarget.Value, new Noun(noun), mItem.Type.Attack));
        }

        protected void Beam(string noun)
        {
            if (!mTarget.HasValue) throw new InvalidOperationException("Cannot use the Beam() script with Items that do not have a target.");

            mAction.AddAction(new ElementBeamAction(mEntity, mTarget.Value, new Noun(noun), mItem.Type.Attack));
        }

        protected void Ball(string noun, int radius)
        {
            if (!mTarget.HasValue) throw new InvalidOperationException("Cannot use the Ball() script with Items that do not have a target.");

            mAction.AddAction(new ElementBallAction(mEntity, mTarget.Value, radius, new Noun(noun), mItem.Type.Attack));
        }

        protected void Cone(string noun, int radius)
        {
            if (!mTarget.HasValue) throw new InvalidOperationException("Cannot use the Cone() script with Items that do not have a target.");

            mAction.AddAction(new ElementConeAction(mEntity, mTarget.Value, radius, new Noun(noun), mItem.Type.Attack));
        }

        protected void BallSelf(string noun, int radius)
        {
            //### bob: hack. figure out where the item is
            Vec pos = mItem.Position;
            if (mAction.Game.Hero.Inventory.Contains(mItem) ||
                mAction.Game.Hero.Equipment.Contains(mItem))
            {
                pos = mAction.Game.Hero.Position;
            }

            mAction.AddAction(new ElementBallAction(mEntity, pos, radius, new Noun(noun), mItem.Type.Attack));
        }

        #endregion

        private Entity mEntity;
        private Item mItem;
        private Action mAction;
        private Vec? mTarget;
    }
}
