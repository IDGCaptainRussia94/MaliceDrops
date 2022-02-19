using MonoMod.RuntimeDetour.HookGen;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace MaliceDrops
{
	public class MaliceDrops : Mod
	{
		public static readonly BindingFlags UniversalBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		public static MaliceDrops instance;


        public override void Load()
        {
			instance = this;

		}
        public override void Unload()
        {
			instance = null;
		}

        public override void PostSetupContent()
        {
			//Just incase
			if (ModLoader.GetMod("CalamityMod") != null)
			HookEndPointEdit.LoadInternal();
		}


		internal class HookEndPointEdit
		{

			internal static void LoadInternal()
			{
				_ = ApplyPlayerBadLifeRegenHook;
			}
			private static bool ApplyPlayerBadLifeRegenHook
			{
				get
				{
					HookEndpointManager.Add(typeof(NPCLoader).GetMethod("NPCLoot", MaliceDrops.UniversalBindingFlags), (hook_NPCLootDetour)DetourNPCLoot);
					HookEndpointManager.Add(typeof(NPCLoader).GetMethod("PreNPCLoot", MaliceDrops.UniversalBindingFlags), (hook_PreLootDetour)DetourPreNPCLoot);

					return false;
				}
			}

			private delegate void orig_NPCLootDetour(NPC npc);
			private delegate void hook_NPCLootDetour(orig_NPCLootDetour orig, NPC npc);

			private delegate bool orig_PreNPCLootDetour(NPC npc);
			private delegate bool hook_PreLootDetour(orig_PreNPCLootDetour orig, NPC npc);

			private static void DetourNPCLoot(orig_NPCLootDetour orig, NPC npc)
			{
				if (CalamityMod.World.CalamityWorld.malice)
				{
					orig(npc);
                }
                else
                {
					Main.NewText("mal test");
					CalamityMod.World.CalamityWorld.malice = true;
					orig(npc);
					CalamityMod.World.CalamityWorld.malice = false;
				}
			}

			private static bool DetourPreNPCLoot(orig_PreNPCLootDetour orig, NPC npc)
			{
				if (CalamityMod.World.CalamityWorld.malice)
				{
					return orig(npc);
				}
				else
				{
					Main.NewText("mal test pre");
					CalamityMod.World.CalamityWorld.malice = true;
					bool returner = orig(npc);
					CalamityMod.World.CalamityWorld.malice = false;
					return returner;
				}
			}
		}
	}


}