using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Realms.NPCs
{
	// Party Zombie is a pretty basic clone of a vanilla NPC. To learn how to further adapt vanilla NPC behaviors, see https://github.com/tModLoader/tModLoader/wiki/Advanced-Vanilla-Code-Adaption#example-npc-npc-clone-with-modified-projectile-hoplite
	public class PartyZombie : ModNPC
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("BebugBube");
		}

		public override void SetDefaults() {
			NPC.width = 18;
			NPC.height = 40;
			NPC.damage = 14;
			NPC.defense = 6;
			NPC.lifeMax = 200;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
			NPC.value = 60f;
			NPC.knockBackResist = 0.5f;
			NPC.aiStyle = -1;
			AIType = -1;
			NPC.noGravity = true;
			NPC.friendly = true;
			Banner = Item.NPCtoBanner(NPCID.Zombie);
			BannerItem = Item.BannerToItem(Banner);
		}

        public override void AI()
        {
			NPC.position.X += (Vector2.Normalize(Main.LocalPlayer.position - NPC.position) * 0.75f).X;
			NPC.ai[0]++;
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			Main.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			float scaling = MathHelper.Clamp(NPC.ai[0] / 800f, 0, 0.5f);
			Vector3 off = (new Vector3((NPC.Center - Main.screenPosition) - Main.ViewSize / 2, 0) * new Vector3(1, -1, 1)) - new Vector3(0, 200 * scaling, 0);
			ContentHandler.GetModel("Realms/Models/Sans").Draw(Matrix.CreateScale(scaling) * Matrix.CreateRotationY(Main.GameUpdateCount / 30f) * Matrix.CreateTranslation(off),
				Matrix.CreateLookAt(new Vector3(0, 0, 100), new Vector3(0, 0, 0), Vector3.UnitY),
				Matrix.CreateOrthographic(Main.screenWidth, Main.screenHeight, -100, 1000));
			return false;
        }
    }
}
