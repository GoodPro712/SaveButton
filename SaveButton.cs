using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.UI;

namespace SaveButton
{
	public class SaveButton : Mod
	{
		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int mouseText = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (mouseText != -1)
			{
				layers.Insert(mouseText, new LegacyGameInterfaceLayer("SaveButton: Save Button", delegate
				{
					if (!Main.gameMenu && !Main.playerInventory)
					{
						int X = 2;
						int Y = 2;
						string text = "Save";
						string hoverText = "Click to save game.";

						Vector2 size = Utils.DrawBorderString(Main.spriteBatch, text, new Vector2(X, Y), Color.WhiteSmoke);
						Rectangle rectangle = new Rectangle(X, Y, (int)size.X + Y, (int)size.Y - 10);
						if (rectangle.Contains(new Point(Main.mouseX, Main.mouseY)))
						{
							Main.hoverItemName = hoverText;
							if(Main.mouseLeftRelease && Main.mouseLeft)
							{
								if (Main.netMode == NetmodeID.SinglePlayer)
								{
									ThreadPool.QueueUserWorkItem(new WaitCallback(SavePlayer), 1);
									ThreadPool.QueueUserWorkItem(new WaitCallback(SaveWorld), 1);
									Main.NewText("Saved player and world!", Color.CornflowerBlue);
								}
								else if (Main.netMode == NetmodeID.MultiplayerClient)
								{
									ThreadPool.QueueUserWorkItem(new WaitCallback(SavePlayer), 1);
									Main.NewText("Saved player!", Color.CornflowerBlue);
								}
							}
						}
					}
					return true;
				},
					InterfaceScaleType.UI)
				);
			}
		}

		internal void SavePlayer(object threadContext)
		{
			Player.SavePlayer(Main.ActivePlayerFileData, false);
		}

		internal void SaveWorld(object threadContext)
		{
			WorldFile.saveWorld();
		}
	}
}