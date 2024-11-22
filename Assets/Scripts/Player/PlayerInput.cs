using UnityEngine;

namespace Csce552
{
    [System.Serializable]
    public struct PlayerInput
    {
        public Button jump;
        public Button bound;
        public Button left;
        public Button right;

        public void Update(bool jump, bool bound, bool left, bool right)
        {
            this.jump.Update(jump);
            this.bound.Update(bound);
            this.left.Update(left);
            this.right.Update(right);
        }

        public float Move()
        {
            float output = 0f;
            if (left.Pressed)
                output += 1f;
            if (right.Pressed)
                output -= 1f;
            return output;
        }

        public PlayerInput Take()
        {
            return new PlayerInput()
            {
                jump = jump.Take(),
                bound = bound.Take(),
                left = left.Take(),
                right = right.Take(),
            };
        }
    }
}