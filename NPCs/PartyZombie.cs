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
			npc.width = 18;
			npc.height = 40;
			npc.damage = 14;
			npc.defense = 6;
			npc.lifeMax = 200;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath2;
			npc.value = 60f;
			npc.knockBackResist = 0.5f;
			npc.aiStyle = -1;
			aiType = -1;
			npc.noGravity = true;
			npc.friendly = true;
			banner = Item.NPCtoBanner(NPCID.Zombie);
			bannerItem = Item.BannerToItem(banner);
		}

        public override void AI()
        {
			npc.position.X += (Vector2.Normalize(Main.LocalPlayer.position - npc.position) * 0.75f).X;
			npc.ai[0]++;
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
			Main.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			float scaling = MathHelper.Clamp(npc.ai[0] / 800f, 0, 0.5f);
			Vector3 off = (new Vector3((npc.Center - Main.screenPosition) - Main.ViewSize / 2, 0) * new Vector3(1, -1, 1)) - new Vector3(0, 200 * scaling, 0);
			ContentHandler.GetModel("Realms/Models/Sans").Draw(Matrix.CreateScale(scaling) * Matrix.CreateRotationY(Main.GameUpdateCount / 30f) * Matrix.CreateTranslation(off),
				Matrix.CreateLookAt(new Vector3(0, 0, 100), new Vector3(0, 0, 0), Vector3.UnitY),
				Matrix.CreateOrthographic(Main.screenWidth, Main.screenHeight, -100, 1000));
			return false;
        }
    }
}
