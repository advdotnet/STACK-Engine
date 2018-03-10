using STACK.Graphics;

namespace STACK
{
    public interface IDraw
    {
        bool Visible { get; set; }
        void Draw(Renderer renderer);
        float DrawOrder { get; set; }
    }
}
