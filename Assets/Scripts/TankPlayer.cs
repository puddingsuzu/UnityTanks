using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Tanks
{
    public class TankPlayer : MonoBehaviourPunCallbacks
    {
        // Reference to tank's movement script, used to disable and enable control.
        private Complete.TankMovement m_Movement;
        // Reference to tank's shooting script, used to disable and enable control.
        private Complete.TankShooting m_Shooting;
        private void Awake()
        {
            m_Movement = GetComponent<Complete.TankMovement>();
            m_Shooting = GetComponent<Complete.TankShooting>();

            //若這個物件不是(!)自己的話，desable移動(m_Movement)以及射擊(m_Shooting)，以及停止這個script (enabled)
            if (!photonView.IsMine)
            {
                m_Movement.enabled = false;
                m_Shooting.enabled = false;
                enabled = false;
            }
        }



        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
