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
        private Transform upTransform;
        private Vector3 AxisDrag = new Vector3(0, 0.015f, 0);
        private Vector3 xyz;
        private float currentVelocitySqr;
        private float velocityCap = 100f;
        private float sqrCap;
        private Vector3 liftNormalRot;//ペラの水平角度
        private int id;//ブロックID
        private PropellorController propController;

        public void Awake()
        {
            if(StatMaster.levelSimulating)
            {
                sqrCap = velocityCap * velocityCap;
            }
        }
        public void Start()
        {
            rigidBody = base.GetComponent<Rigidbody>();//ブロックのrigidbody取得
            propController = base.GetComponent<PropellorController>();
            id = propController.BlockID;
            switch(id)
            {
                case 26:
                    liftNormalRot = new Vector3(0, 0, 23.06876f);
                    break;
                case 52:
                case 55:
                    liftNormalRot = new Vector3(0, 0, 22.845f);
                    break;
            }
            propController.AxisDrag = new Vector3(0, 0, 0);//既存プロペラコントローラーの空気抵抗をゼロにする
        }
        public void FixedUpdate()
        {
            if(StatMaster.levelSimulating)
            {
                if(isFirstFrame)
                {
                    isFirstFrame = false;
                }
                else
                {
                    FixedUpdateBlock();
                }
            }
        }
        private void FixedUpdateBlock()
        {
            upTransform = propController.upTransform;
            currentVelocity = rigidBody.velocity;
            Vector3 vector = upTransform.InverseTransformDirection(currentVelocity);
            xyz = Vector3.Scale(-vector, AxisDrag);
            currentVelocitySqr = Mathf.Min(currentVelocity.sqrMagnitude, sqrCap);
            Vector3 force = xyz * currentVelocitySqr;
            Quaternion qua = Quaternion.FromToRotation(Vector3.up, upTransform.up);
            rigidBody.AddRelativeForce(Quaternion.Euler(liftNormalRot) * force);
        }
    }
}