using UnityEngine;

namespace Csce552
{
    [System.Serializable]
    public struct Button
    {
        [SerializeField]
        private bool justPressed;
        [SerializeField]
        private bool justReleased;
        [SerializeField]
        private bool pressed;
        
        public bool JustPressed => justPressed;
        public bool JustReleased => JustReleased;
        public bool Pressed => pressed;
        public bool Released => !pressed;

        public void Update(bool pressed)
        {
            if (!this.pressed && pressed)
                justPressed = true;
            if (this.pressed && !pressed)
                justReleased = true;
            this.pressed = pressed;
        }

        public Button Take()
        {
            Button output = new Button()
            {
                justPressed = justPressed,
                justReleased = justReleased,
                pressed = pressed
            };

            justPressed = justReleased = false;

            return output;
        }
    }
}