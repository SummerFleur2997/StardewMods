using BetterHatsAPI.Framework;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;
using UI.Component;
using UI.Menu;
using UI.Sprite;

namespace BetterHatsAPI.GuideBook;

public class GuideMenu : BaseMenu
{
    /// <summary>
    /// Used to compatible with LookupAnything.
    /// </summary>
    [UsedImplicitly] public Hat HoveredItem;

    private const int MenuWidth = 1352;
    private const int MenuHeight = 792;

    private const int GridMenuWidth = 390;
    private const int GridMenuHeight = 520;

    private const int StatPanelWidth = 400;
    private const int StatPanelHeight = 160;

    private const int TextPanelWidth = 650;
    private const int HatIconSize = 128;

    private GridMenu _hatGridMenu;
    private ItemLabel<Hat> _hatIcon;
    private HatDataStatPanel _statPanel;
    private HatDataTextPanel _textPanel;

    private TextLabel _hatName;
    private TextLabel _hatDesc;

    private Status _dataStatus;

    public GuideMenu(int x, int y)
        : base(x, y, MenuWidth, MenuHeight)
    {
        var region = new TextureRegion(Texture.MenuBackground, 0, 0, MenuWidth / 4, MenuHeight / 4);
        Background = new SpriteLabel(region, x, y, MenuWidth, MenuHeight);
        BuildWidgets();
    }

    private void BuildWidgets()
    {
        var gridX = 90 + xPositionOnScreen;
        var gridY = 138 + yPositionOnScreen;

        var panelX = 840 + xPositionOnScreen;
        var panelY = 150 + yPositionOnScreen;

        var iconX = 644 + xPositionOnScreen + 32;
        var iconY = 156 + yPositionOnScreen + 32;

        var textX = 620 + xPositionOnScreen;

        var nameY = 310 + yPositionOnScreen;
        var descY = 350 + yPositionOnScreen;
        var textY = 440 + yPositionOnScreen;

        _hatGridMenu = new GridMenu(gridX, gridY, GridMenuWidth, GridMenuHeight, 65);
        _hatIcon = new ItemLabel<Hat>((Hat)null, iconX, iconY, HatIconSize, HatIconSize);
        _statPanel = new HatDataStatPanel(panelX, panelY, StatPanelWidth, StatPanelHeight);
        _textPanel = new HatDataTextPanel(textX, textY, TextPanelWidth, 200);
        _hatName = new TextLabel("", Color.Black, Game1.smallFont, textX, nameY);
        _hatDesc = new TextLabel("", Color.DimGray, Game1.smallFont, textX, descY);

        UpdateDataByHat();

        var buttons = HatDataHelper.AllHatData.Keys
            .Select(h => new ItemButton<Hat>(h))
            .ToList();

        foreach (var button in buttons)
            button.OnPress += () => UpdateDataByHat(button.Item);

        _hatGridMenu.AddComponents(buttons);

        // Add components to menu
        AddChild(_hatGridMenu);
        AddChild(_hatIcon);
        AddChild(_statPanel);
        AddChild(_textPanel);
        AddChild(_hatName);
        AddChild(_hatDesc);
    }

    private void UpdateDataByHat(Hat hat = null)
    {
        hat ??= Game1.player.hat.Value;

        var dataList = hat.GetHatData();

        var data = dataList.FirstOrDefault() ?? new HatData();
        // var data = HatData.Combine(dataList);
        UpdateDataByData(data); // todo
        _hatIcon.Item = hat;

        if (hat is null) return;

        var desc = Game1.parseText(hat.description, Game1.smallFont, 640);
        _hatName.Text = hat.DisplayName;
        _hatDesc.Text = desc;
    }

    private void UpdateDataByData(HatData data, bool isCombinedData = false)
    {
        if (data is null && !isCombinedData)
        {
            _dataStatus = Status.Null;
            // todo
            return;
        }

        _dataStatus = isCombinedData ? Status.CombinedData : Status.SingleData;
        _statPanel.UpdateStats(data);
        _textPanel.UpdateData(data);
    }

    public override void Draw(SpriteBatch b) { }

    public override void Dispose()
    {
        _hatGridMenu.Dispose();
        _statPanel.Dispose();
        base.Dispose();
    }

    private enum Status
    {
        CombinedData,
        SingleData,
        Null
    }
}