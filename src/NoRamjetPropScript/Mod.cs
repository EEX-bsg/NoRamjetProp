using System;
using Modding;
using UnityEngine;
using NoRamjetPropNS;

namespace NoRamjetPropNS
{
	public class Mod : ModEntryPoint
	{
		public override void OnLoad()
		{
			GameObject gameObject = GameObject.Find("EsModControllerObject");
			if(!gameObject)
			{
				UnityEngine.Object.DontDestroyOnLoad(gameObject = new GameObject("EsModControllerObject"));
			}
			gameObject.AddComponent<NoRamjetPropCore>();
		}
	}
}
