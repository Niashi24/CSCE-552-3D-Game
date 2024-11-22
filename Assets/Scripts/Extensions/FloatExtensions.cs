
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Csce552
{
    
    public static class FloatExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AtLeast(this float f, float min) => Mathf.Max(min, f);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 WithX(this Vector2 v, float x)
        {
            return new Vector2(x, v.y);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 WithY(this Vector2 v, float y)
        {
            return new Vector2(v.x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 WithX(this Vector3 v, float x)
        {
            return new Vector3(x, v.y, v.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 WithY(this Vector3 v, float y)
        {
            return new Vector3(v.x, y, v.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 WithZ(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }
        
        public static float WithMaxMagnitude(this float f, float maxMagnitude)
        {
            return Mathf.Min(Mathf.Abs(f), Mathf.Abs(maxMagnitude)) * Mathf.Sign(f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="curVel"></param>
        /// <param name="dir"></param>
        /// <param name="accel"></param>
        /// <param name="skidAccel"></param>
        /// <param name="maxSpeed"></param>
        /// <returns></returns>
        public static float AddAccelerationInDirection(float curVel, float dir, float accel, float skidAccel, float maxSpeed)
        {
            float sign = Mathf.Sign(curVel);
            float a = sign == Mathf.Sign(dir) ? accel : skidAccel;
            curVel *= sign;  // now is positive/abs
            a *= sign * dir;
            return sign * AddAcceleration(curVel, a, maxSpeed);
        }
        
        /// <summary>
        /// Adds the given acceleration without going over maximum speed
        /// </summary>
        /// <param name="curSpeed">Current speed (absolute value)</param>
        /// <param name="accel">Acceleration (may be negative)</param>
        /// <param name="maxSpeed">Max speed (absolute value)</param>
        /// <returns>min(curSpeed + accel, maxSpeed)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AddAcceleration(float curSpeed, float accel, float maxSpeed)
        {
            if (curSpeed < maxSpeed || accel < 0)
                return Mathf.Min(curSpeed + accel, maxSpeed);
            else
                return curSpeed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 WithMaxMagnitude(this Vector2 v, float maxMag)
        {
            if (v.magnitude < maxMag) return v;
            return v.normalized * maxMag;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 WithMaxMagnitude(this Vector3 v, float maxMag)
        {
            if (v.magnitude < maxMag) return v;
            return v.normalized * maxMag;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ChangeToSurface(Vector2 dir, Vector2 surfaceNormal)
        {
            if (Vector2.Dot(dir, surfaceNormal) >= 0) return dir;
            return CastToSurface(dir, surfaceNormal);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ChangeToSurface(Vector3 dir, Vector3 surfaceNormal)
        {
            if (Vector3.Dot(dir, surfaceNormal) >= 0) return dir;
            return CastToSurface(dir, surfaceNormal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 CastToSurface(Vector2 dir, Vector2 surfaceNormal)
        {
            return dir - Vector2.Dot(dir, surfaceNormal) * surfaceNormal;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 CastToSurface(Vector3 dir, Vector3 surfaceNormal)
        {
            return dir - Vector3.Dot(dir, surfaceNormal) * surfaceNormal;
        }

        // Returns `v` with a value of `val` in the `dir` direction
        public static Vector3 InDirection(this Vector3 v, Vector3 dir, float val)
        {
            v -= Vector3.Dot(v, dir) * dir;
            return v + dir * val;
        }

        // Returns `v` with a max `max` magnitude in the `dir` direction
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 MaxInDirection(this Vector3 v, Vector3 dir, float max)
        {
            float magnitude = Vector3.Dot(v, dir);
            v -= magnitude * dir;
            
            magnitude = Mathf.Min(magnitude, max);

            return v + dir * magnitude;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 WithDirection(this Vector2 cur, Vector2 newDir) => newDir * cur.magnitude;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 WithDirection(this Vector3 cur, Vector3 newDir) => newDir * cur.magnitude;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 WithMagnitude(this Vector2 cur, float magnitude) => cur.normalized * magnitude;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Reflect(this Vector2 v, Vector2 normal)
        {
            // Dot product of v and normal
            float dot = Vector2.Dot(v, normal);
    
            // Reflection formula: v - 2 * dot * normal
            return v - 2 * dot * normal;
        }
    }
}