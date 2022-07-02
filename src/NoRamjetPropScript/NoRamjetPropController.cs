using System;
using Modding;
using UnityEngine;

namespace NoRamjetPropNS
{
    public class NoRamjetPropController : MonoBehaviour
    {
        private bool isFirstFrame = true;

        private Rigidbody rigidBody;
        private Vector3 currentVelocity;
        private Transform upTransform;//速度検出面と言われるもの
        private Vector3 AxisDrag = new Vector3(0, 0.015f, 0);//空力のかかる量
        private float velocityCap = 30f;//この速度より小さいと、速度の３乗比例で空力かかる
        private Vector3 liftNormalRot;//ペラの水平角度
        private Vector3 xyz;
        private float currentVelocitySqr;
        private float sqrCap;
        private PropellorController propController;
        private MToggle ramjetToggleBtn;
        private bool ramjetFlag = false;

        public void Awake()
        {
            if(StatMaster.levelSimulating)
            {
                sqrCap = velocityCap * velocityCap;
            }
            ramjetToggleBtn = GetComponent<BlockBehaviour>().AddToggle("Toggle Ramjet", "ramjet_button", ramjetFlag);
            if(StatMaster.isMP)
            {
                if(StatMaster.isHosting || StatMaster.isLocalSim)
                {
                    ServerHostAwake();
                }
            }
            else{
                ServerHostAwake();
            }
        }
        public void Start()
        {
            rigidBody = base.GetComponent<Rigidbody>();//ブロックのrigidbody取得
            propController = base.GetComponent<PropellorController>();//既存のプロペラコントローラーを取得
            if(ramjetFlag)
            {
                propController.AxisDrag = AxisDrag;
            }
            else
            {
                liftNormalRot = new Vector3(0, 0, propController.upTransform.localEulerAngles.z);//ペラの水平角度を持ってくる Fで反転するから固定値ではダメ
                propController.AxisDrag = new Vector3(0, 0, 0);//既存プロペラコントローラーの空気抵抗をゼロにする
            }
        }
        public void FixedUpdate()
        {
            if(StatMaster.levelSimulating)//シミュレートをしているとき
            {
                if(isFirstFrame)//最初のフレームはスキップした方が良いらしい(少なくともBesiegeのコードはそうなっている)
                {
                    isFirstFrame = false;
                }
                else
                {
                    if(StatMaster.isMP)
                    {
                        if(StatMaster.isHosting || StatMaster.isLocalSim)//サーバーに入ってるので、ホストかローカルシミュのときのみ実行
                        {
                            FixedUpdateBlock();
                        }
                    }
                    else
                    {
                        FixedUpdateBlock();//サーバーに入ってないので実行
                    }
                }
            }
        }
        private void FixedUpdateBlock()
        {
            if(!ramjetFlag)
            {
                upTransform = propController.upTransform;
                currentVelocity = rigidBody.velocity;
                Vector3 vector = upTransform.InverseTransformDirection(currentVelocity);//ここで速度検出面から速度を取得してる
                xyz = Vector3.Scale(-vector, AxisDrag);
                currentVelocitySqr = Mathf.Min(currentVelocity.sqrMagnitude, sqrCap);
                Vector3 force = xyz * currentVelocitySqr;
                Quaternion qua = Quaternion.FromToRotation(Vector3.up, upTransform.up);
                rigidBody.AddRelativeForce(Quaternion.Euler(liftNormalRot) * force);//速度検出面垂直方向に空力を傾けてる(謎加速を消してる)
            }
        }
        private void ServerHostAwake()
        {
            ramjetToggleBtn.Toggled += delegate (bool value)
            {
                ramjetFlag = value;
            };
        }
    }
}