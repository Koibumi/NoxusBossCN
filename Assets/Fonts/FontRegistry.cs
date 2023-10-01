using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using NoxusBoss.Content.Items.LoreItems;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace NoxusBossCN.Assets.Fonts {
    public class FontRegistry : ModSystem {
        public static Asset<DynamicSpriteFont> KaiTiFont;
        private static ILHook hook1, hook2;
        public override void Load() {
            KaiTiFont = ModContent.Request<DynamicSpriteFont>("NoxusBossCN/Assets/Fonts/KaiTi", AssetRequestMode.AsyncLoad);
        }
        public override void PostSetupContent() {
            Type fontRegistry = typeof(NoxusBoss.Assets.Fonts.FontRegistry);
            ILContext.Manipulator mani = il => {
                ILCursor c = new ILCursor(il);
                if (!c.TryGotoNext(MoveType.After,
                i => i.MatchCall(fontRegistry.GetMethod("get_Instance", BindingFlags.Static | BindingFlags.Public)),
                i => i.MatchCallvirt(fontRegistry.GetMethod("get_XerocText", BindingFlags.Instance | BindingFlags.Public))
                ))
                    return;
                c.Emit(OpCodes.Pop);
                c.EmitDelegate(() => KaiTiFont.Value);
            };
            hook1 = new ILHook(typeof(LoreXeroc).GetMethod("ModifyTooltips", BindingFlags.Instance | BindingFlags.Public), mani);
            hook1.Apply();

            hook2 = new ILHook(typeof(LoreXeroc).GetMethod("PreDrawTooltipLine", BindingFlags.Instance | BindingFlags.Public), mani);
            hook2.Apply();

        }
        public override void Unload() {
            hook1?.Dispose();
            hook2?.Dispose();
            hook1 = hook2 = null;
        }
    }
}
