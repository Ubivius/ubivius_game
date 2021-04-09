using UnityEngine;
using System.Collections;
using ubv.common;
using ubv.common.data;
using System.Collections.Generic;
using UnityEngine.Events;

namespace ubv.server.logic
{
    public class EnemyBehaviorUpdater : ServerGameplayStateUpdater
    {
        public UnityEvent OnGenerateEnemy { get; private set; }
        private Dictionary<int, Rigidbody2D> m_bodies;


        public override void Setup()
        {
            m_bodies = new Dictionary<int, Rigidbody2D>();
            OnGenerateEnemy = new UnityEvent();
        }

        public override void InitClient(ClientState state)
        {
            int id = state.PlayerGUID;
            OnGenerateEnemy.Invoke();

        }

        public override void InitPlayer(PlayerState player)
        {

        }

        public override void FixedUpdateFromClient(ClientState client, InputFrame frame, float deltaTime)
        {
            Rigidbody2D body = m_bodies[client.PlayerGUID];
        }

        public override void UpdateClient(ClientState client)
        {
            Rigidbody2D body = m_bodies[client.PlayerGUID];
            PlayerState player = client.GetPlayer();
            player.Position.Value = body.position;
            player.Rotation.Value = body.rotation;
        }
    }
}
