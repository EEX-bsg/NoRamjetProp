using System;
using System.Collections;
using Modding;
using UnityEngine;
using Modding.Blocks;

namespace NoRamjetPropNS
{
    public class NoRamjetPropCore : MonoBehaviour
    {
        void Awake()
        {
            StartCoroutine(CheckVersion());
        }
        private void Start()
        {
            Events.OnBlockPlaced += AddScript;
        }
        private void AddScript(Block block)
        {
            int type = block.Prefab.Type;//設置されたブロックのID
            if(type == 26 || type == 52 || type == 55)//26:長プロペラ, 52:52プロペラ, 55:小プロペラ
            {
                PropellorController propController = block.GameObject.GetComponent<PropellorController>();
                if(propController == null){
                    Debug.LogWarning("NoRamjetProp: Unfind PropellorController");
                    return;
                }
                NoRamjetPropController newPropController = block.GameObject.AddComponent<NoRamjetPropController>();//新しいプロペラコントローラーの追加
            }
        }
        private IEnumerator CheckVersion()
        {
             yield return new WaitForSeconds(1f);
             Debug.Log("NoRamjetProp Version :" + Mods.GetVersion(new Guid("512bd801-8402-4e01-a800-7ed6e45a3949")));
        }
    }
}
