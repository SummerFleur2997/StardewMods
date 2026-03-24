using ConvenientChests.Framework.DataStructs;
using ConvenientChests.Framework.UserInterfaceService;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UI.Component;
using UI.Menu;
using UI.Sprite;

namespace ConvenientChests.AliasForChests;

internal class AliasSetMenu : SubMenu
{
    private readonly TextBox _textBox;
    private readonly ItemButton _itemIconButton;
    private readonly GridMenu _itemPicker;
    private readonly ChestData _chestData;

    private bool _itemPickerOn;
    private ChestOverlay _parent;

    public AliasSetMenu(ChestData data, ChestOverlay parent) : base(400, 240)
    {
        _chestData = data;
        _parent = parent;

        var y = Y + 24;

        // init title label
        var textLabel = new TextLabel(I18n.UI_ChestAlias(), Color.Black, Game1.smallFont);
        textLabel.SetInCenterOfTheBounds(Bounds);
        textLabel.Y = y;
        Components.Add(textLabel);

        // init item button and textbox
        y += textLabel.Height + 16;
        var itemButtonBackground = UIHelper.LightButtonBackground();
        itemButtonBackground.SetDestination(X + 24, y, 64, 64);
        Components.Add(itemButtonBackground);

        var item = _chestData.ItemIcon;
        _itemIconButton = new ItemButton(item);
        _itemIconButton.SetPosition(X + 32, y + 8);
        _itemIconButton.OnPress += () => _itemPickerOn = true;
        Components.Add(_itemIconButton);

        var text = _chestData.Alias ?? "";
        _textBox = new TextBox(X + 100, y, 272, 64, text, UIHelper.TextBubble());
        Game1.keyboardDispatcher.Subscriber = _textBox;
        Components.Add(_textBox);

        // init item picker
        var itemPickerBackground = NineSlice.SmallMenuBackground();
        itemPickerBackground.SetDestination(X - 416, y, 404, 404);

        _itemPicker = new GridMenu(X - 408, y + 8, 384, 384, 64);
        _itemPicker.Background = itemPickerBackground;

        var buttons = _chestData.GetChest()?.Items
            .DistinctBy(i => i.QualifiedItemId)
            .Select(i => new ItemButton(i.QualifiedItemId))
            .Append(ItemButton.GetANullInstance())
            .ToList() ?? new List<ItemButton>();

        foreach (var button in buttons)
            button.OnPress += () =>
            {
                _itemIconButton.Item = button.Item;
                _itemPickerOn = false;
            };

        _itemPicker.AddComponents(buttons);
        OnOk += SetAlias;
    }

    private void SetAlias(SubMenu s)
    {
        _chestData.SetAlias(_textBox.Text);
        _chestData.SetIcon(_itemIconButton.Item);
        ModEntry.AliasModule.ForceUpdateOnce = true;
    }

    /// <inheritdoc/>
    public override bool ReceiveLeftClick(int x, int y)
    {
        if (!_itemPickerOn) return base.ReceiveLeftClick(x, y);

        if (_itemPicker.Contains(x, y))
        {
            _itemPicker.ReceiveLeftClick(x, y);
            return true;
        }

        Game1.playSound("bigDeSelect");
        _itemPickerOn = false;
        return true;
    }

    /// <inheritdoc/>
    public override bool ReceiveScrollWheelAction(int amount)
    {
        if (!_itemPickerOn) return false;

        _itemPicker.ReceiveScrollWheelAction(amount);
        return true;
    }

    /// <inheritdoc/>
    public override bool ReceiveKeyPress(Keys key)
    {
        if (_itemPickerOn || _textBox.Selected)
            switch (key)
            {
                case Keys.Escape:
                    _itemPickerOn = false;
                    _textBox.Selected = false;
                    Game1.playSound("bigDeSelect");
                    return true;
                case Keys.Enter:
                    _itemPickerOn = false;
                    _textBox.Selected = false;
                    break; // fall back to base method to handle enter key
                default:
                    return true;
            }

        return base.ReceiveKeyPress(key);
    }

    /// <summary>
    /// Customized logic for <see cref="ChestOverlay.ReceiveKeyPress"/>.
    /// </summary>
    public bool HandleOrSuppressThisKeyPress(Keys key)
    {
        ReceiveKeyPress(key);
        return true;
    }

    /// <inheritdoc/>
    public override void Draw(SpriteBatch b)
    {
        base.Draw(b);

        if (!_itemPickerOn) return;

        _itemPicker.Draw(b);
    }

    public override void Dispose()
    {
        _textBox.Dispose();
        _itemPicker.Dispose();
        _parent.SetItemsClickable(true);

        var parent = _parent;
        _parent = null!;
        parent.AliasMenu = null;
        // parent.RootMenu

        base.Dispose();
    }

    internal sealed class ItemButton : ItemButton<Item>
    {
        private readonly TextureRegion _redX = new(Game1.mouseCursors, 268, 470, 16, 16);

        public ItemButton(Item? item) : base(item, width: 48, height: 48, setTooltip: false) { }
        public ItemButton(string item) : base(item, width: 48, height: 48, setTooltip: false) { }

        public static ItemButton GetANullInstance() => new((Item)null!);

        public override void Draw(SpriteBatch b)
        {
            if (Item is null)
            {
                _redX.Draw(b, new Rectangle(X, Y, 48, 48));
                return;
            }

            Item.drawInMenu(b, new Vector2(X - 8, Y - 8), 0.75f);
        }
    }
}