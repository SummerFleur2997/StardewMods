using ConvenientChests.Framework.DataStructs;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UI.Component;
using UI.Menu;
using UI.Sprite;

namespace ConvenientChests.CategorizeChests.UI;

/// <summary>
/// Customized menu for <see cref="CategoryMenu{T}"/>.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
internal sealed class CategoryTopRow : IClickableMenu, IClickableComponent
{
    /// <inheritdoc/>
    public Rectangle Bounds => new(X, Y, Width, Height);

    /// <inheritdoc/>
    public int X { get; set; }

    /// <inheritdoc/>
    public int Y { get; set; }

    /// <inheritdoc/>
    public int Width { get; set; }

    /// <inheritdoc/>
    public int Height { get; set; }

    public NineSlice SelectAllButtonBackground;
    public LabeledCheckBox SelectAllButton;
    public SpriteButton PrevButton;
    public SpriteButton NextButton;
    public DropDownMenu<ItemCategoryName> CategorySelector;

    private readonly List<IClickableComponent> _components = new();

    public CategoryTopRow(int x, int y, int width, int height, int parentMenuHeight)
    {
        SelectAllButtonBackground = NineSlice.SmallMenuBackground();
        SelectAllButton = new LabeledCheckBox(I18n.Categorize_All(), Color.Black);
        PrevButton = new SpriteButton(TextureRegion.UpArrow());
        NextButton = new SpriteButton(TextureRegion.DownArrow());
        CategorySelector = new DropDownMenu<ItemCategoryName>(parentMenuHeight);

        var actualHeight = Math.Max(SelectAllButton.Height, PrevButton.Height);
        y -= Math.Max(0, actualHeight - height); // offset to the top
        this.SetDestination(x, y, width, height);
        SelectAllButtonBackground.SetDestination(x, y, SelectAllButton.Width + Game1.pixelZoom * 10, height);
        SelectAllButton.SetAtLeftCenterWithEqualMargins(SelectAllButtonBackground.Bounds);

        x += SelectAllButtonBackground.Width + Game1.pixelZoom * 4;
        PrevButton.SetDestination(x, y, height, height);
        x += height - Game1.pixelZoom * 2;
        NextButton.SetDestination(x, y, height, height);
        x += height + Game1.pixelZoom * 4;
        CategorySelector.SetPosition(x, y);

        _components.Add(SelectAllButton);
        _components.Add(PrevButton);
        _components.Add(NextButton);
    }

    public void Draw(SpriteBatch b)
    {
        SelectAllButtonBackground.Draw(b);
        SelectAllButton.Draw(b);
        PrevButton.Draw(b);
        NextButton.Draw(b);
        CategorySelector.Draw(b);
    }

    public bool ReceiveLeftClick(int x, int y)
    {
        var handled = CategorySelector.ReceiveLeftClick(x, y);
        if (handled) return true;

        if (!Bounds.Contains(x, y))
            return false;

        foreach (var component in _components)
            if (component.ReceiveLeftClick(x, y))
                return true;

        return true;
    }

    public bool ReceiveCursorHover(int x, int y) =>
        CategorySelector.Expanded && CategorySelector.ReceiveCursorHover(x, y);

    public bool ReceiveScrollWheelAction(int amount) =>
        CategorySelector.Expanded && CategorySelector.ReceiveScrollWheelAction(amount);

    public void Dispose()
    {
        CategorySelector.Dispose();
        _components.Clear();
    }
}