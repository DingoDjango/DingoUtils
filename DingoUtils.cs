﻿using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Verse;

namespace DD_WorkTab
{
	public static class DingoUtils
	{
		/// <summary>
		/// The alphanumeric name of the mod defined in its About.xml file.
		/// </summary>
		private static string ModName = "Notifications Archiver";

		/// <summary>
		/// Provides string caching (for translations).
		/// </summary>
		public static Dictionary<object, string> CachedStrings = new Dictionary<object, string>();

		/// <summary>
		/// Provides quick storage and access to translations. Prevents calling the .Translate() chain multiple times.
		/// </summary>
		public static string CachedTranslation(this string inputText, object[] args = null)
		{
			if (!CachedStrings.TryGetValue(inputText, out string finalString))
			{
				finalString = inputText.Translate();

				CachedStrings[inputText] = finalString;
			}

			if (args != null)
			{
				return string.Format(finalString, args);
			}

			return finalString;
		}

		/// <summary>
		/// Generates a high quality texture from a PNG file.
		/// </summary>
		public static Texture2D GetHQTexture(string fileName, string folderName = null)
		{
			Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false); //Mipmaps off;

			ModContentPack content = null;

			foreach (ModContentPack mod in LoadedModManager.RunningMods)
			{
				if (mod.Name == ModName)
				{
					content = mod;
				}
			}

			if (content == null)
			{
				Log.Error("Could not find specific mod :: " + ModName);

				return texture;
			}

			string texturesPath = Path.Combine(content.RootDir, GenFilePaths.ContentPath<Texture2D>());

			string folderPath = folderName == null ? texturesPath : Path.Combine(new DirectoryInfo(texturesPath).ToString(), folderName);

			DirectoryInfo contentDirectory = new DirectoryInfo(folderPath);

			if (!contentDirectory.Exists)
			{
				Log.Error("Could not find specific textures folder");

				return texture;
			}

			FileInfo image = null;

			foreach (FileInfo f in contentDirectory.GetFiles("*", SearchOption.AllDirectories))
			{
				if (Path.GetFileNameWithoutExtension(f.Name) == fileName)
				{
					image = f;
				}
			}

			if (image == null)
			{
				Log.Error("Could not find specific file name :: " + fileName);

				return texture;
			}

			byte[] fileData = File.ReadAllBytes(image.FullName);

			texture.LoadImage(fileData); //Loads PNG data, sets format to ARGB32 and resizes texture by the source image size
			texture.name = "DD_" + Path.GetFileNameWithoutExtension(fileName);
			texture.filterMode = FilterMode.Trilinear;
			texture.anisoLevel = 9; //default 2, max 9

			/* texture.Compress(true); //Compresses texture
			 * texture.Apply(false, false); //Saves the compressed texture */

			if (texture.width == 2)
			{
				Log.Error("Could not load high quality texture :: " + fileName);
			}

			return texture;
		}

		/// <summary>
		/// Determines which list indexes to render when using a Unity scroll view and fixed item height.
		/// </summary>
		public static void VisibleScrollviewIndexes(float scrolledY, float outRectHeight, float itemHeight, int totalItems, out int FirstRenderedIndex, out int LastRenderedIndex)
		{
			int totalRenderedIndexes = (int)(outRectHeight / itemHeight) + 2; //Account for partly rendered surfaces on top/bottom of the Rect

			FirstRenderedIndex = (int)(scrolledY / itemHeight); //Get the first list item that should be at least partly visible

			LastRenderedIndex = Mathf.Min(FirstRenderedIndex + totalRenderedIndexes, totalItems); //Get the last item to render, don't go over list.Count
		}
	}
}
