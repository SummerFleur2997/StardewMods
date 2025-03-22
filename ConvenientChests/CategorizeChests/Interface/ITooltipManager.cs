using ConvenientChests.Framework.CategorizeChests.Interface.Widgets;
using Microsoft.Xna.Framework.Graphics;

namespace ConvenientChests.Framework.CategorizeChests.Interface
{
    interface ITooltipManager
    {
        void ShowTooltipThisFrame(Widget tooltip);
        void Draw(SpriteBatch batch);
    }
}