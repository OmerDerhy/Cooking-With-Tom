// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("t9hwLLpQh37h74FMqHLMlX9/Jtvfkgf81qY15Ed3fsLg7eSzWFXu1/NBwuHzzsXK6UWLRTTOwsLCxsPAfo8P2rkY5qOoyThSmVch+icTr810Mli0X/M98taXwznO3tOWQSkrUUHCzMPzQcLJwUHCwsNsD0QEYiCA81VEAixgUCaPL5iZXGDG2sWeXSeViR9IUOETUjNzKSY8Fg8iU6pTNgtWyrVviCyLGeS5tytXOCxbE8wCkavtxy3/274qgnH8TRZkZa0Tccq4YcyqMO3PzLAHdFoO/jYTugBXwuuJmFeMwSp4SatUd1bLwfj+tyO2LwHAo6q7vxS8mKrdEo/sokmWlW6CXoQKF2g9k+VGBSP53D/T1r7Ud4qSJcXwIXAy9sHAwsPC");
        private static int[] order = new int[] { 4,11,4,10,5,11,12,7,10,10,11,13,12,13,14 };
        private static int key = 195;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
