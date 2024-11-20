using UnityEngine;

namespace Csce552
{
    [System.Serializable]
    public struct PlayerInput
    {
        public Button jump;
        public Button bound;
        public float move;

        public void Update(bool jump, bool bound, float move)
        {
            this.jump.Update(jump);
            this.bound.Update(bound);
            this.move = move;
        }

        public PlayerInput Take()
        {
            return new PlayerInput()
            {
                jump = jump.Take(),
                bound = bound.Take(),
                move = move,
            };
        }
    }
}