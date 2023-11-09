using UnityEngine;

namespace Module.Core {
    public partial class Utilities {
        public class Math {
            public static float Min(Vector2 value) {
                return (double) value.x >= value.y ? value.y : value.x;
            }

            public static float Min(Vector3 value) {
                var num = value.x;
                if (value.y < (double) num) {
                    num = value.y;
                }
                if (value.z < (double) num) {
                    num = value.z;
                }
                return num;
            }

            public static int Min(Vector2Int value) {
                return value.x >= value.y ? value.y : value.x;
            }

            public static float Min(Vector3Int value) {
                var num = value.x;
                if (value.y < num) {
                    num = value.y;
                }
                if (value.z < num) {
                    num = value.z;
                }
                return num;
            }

            public static float Max(Vector2 value) {
                return (double) value.x <= value.y ? value.y : value.x;
            }

            public static float Max(Vector3 value) {
                var num = value.x;
                if (value.y > (double) num) {
                    num = value.y;
                }
                if (value.z > (double) num) {
                    num = value.z;
                }
                return num;
            }

            public static int Max(Vector2Int value) {
                return value.x <= value.y ? value.y : value.x;
            }

            public static float Max(Vector3Int value) {
                var num = value.x;
                if (value.y > num) {
                    num = value.y;
                }
                if (value.z > num) {
                    num = value.z;
                }
                return num;
            }
        }
    }
}
