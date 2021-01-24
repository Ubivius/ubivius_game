﻿using UnityEngine;
using UnityEditor;

namespace ubv
{
    namespace udp
    {
        public static class Serialization
        {
            public enum BYTE_TYPE : byte
            {
                INPUT_FRAME = 0x00,
                INPUT_MESSAGE = 0x01,
                CLIENT_STATE = 0x02,
                PLAYER_STATE = 0x03,
                AUTH_MESSAGE = 0x04,
            }
        }
    }
}