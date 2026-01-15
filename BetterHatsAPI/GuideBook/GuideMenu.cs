using BetterHatsAPI.Framework;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley.Objects;
using UI.Component;
using UI.Menu;
using UI.Sprite;

// using static BetterHatsAPI.Framework.Utilities;

namespace BetterHatsAPI.GuideBook;

public class GuideMenu : BaseMenu
{
    /// <summary>
    /// Used to compatible with LookupAnything.
    /// </summary>
    [UsedImplicitly] public Hat HoveredItem;

    public const int MenuWidth = 1352;
    public const int MenuHeight = 792;

    private const int GridMenuWidth = 390;
    private const int GridMenuHeight = 520;

    private const int StatPanelWidth = 400;

    private const int TextPanelWidth = 650;
    private const int TextPanelHeight = 230;

    private const int HatIconSize = 128;

    private GridMenu _hatGridMenu;
    private ItemLabel<Hat> _hatIcon;
    private HatDataStatPanel _statPanel;
    private HatDataTextPanel _textPanel;
    private DropDownMenu<HatData> _dropDownMenu;

    private TextLabel _hatName;
    private TextLabel _hatDesc;

    public GuideMenu(int x, int y, Hat targetHat)
        : base(x, y, MenuWidth, MenuHeight)
    {
        var region = new TextureRegion(Texture.MenuBackground, 0, 0, MenuWidth / 4, MenuHeight / 4);
        Background = new SpriteLabel(region, x, y, MenuWidth, MenuHeight);
        BuildWidgets(targetHat);
    }

    private void BuildWidgets(Hat hat)
    {
        var gridX = 90 + xPositionOnScreen;
        var gridY = 138 + yPositionOnScreen;

        var panelX = 840 + xPositionOnScreen;
        var panelY = 138 + yPositionOnScreen;

        var iconX = 644 + 32 + xPositionOnScreen;
        var iconY = 156 + 32 + yPositionOnScreen;

        var textX = 620 + xPositionOnScreen;

        var nameY = 310 + yPositionOnScreen;
        var descY = 360 + yPositionOnScreen;
        var textY = 440 + yPositionOnScreen;

        var dropDownX = 964 + xPositionOnScreen;
        var dropDownY = Math.Max(12 + yPositionOnScreen, 0);

        _hatGridMenu = new GridMenu(gridX, gridY, GridMenuWidth, GridMenuHeight, 65);
        _hatIcon = new ItemLabel<Hat>((Hat)null, iconX, iconY, HatIconSize, HatIconSize);
        _hatName = new TextLabel("", Color.Black, Game1.smallFont, textX, nameY);
        _hatDesc = new TextLabel("", Color.DimGray, Game1.smallFont, textX, descY);
        _textPanel = new HatDataTextPanel(textX, textY, TextPanelWidth, TextPanelHeight);
        _statPanel = new HatDataStatPanel(panelX, panelY, StatPanelWidth);
        _dropDownMenu = new DropDownMenu<HatData>(600, dropDownX, dropDownY);
        _dropDownMenu.OnSelectionChanged += UpdateDataByData;

        foreach (var pack in ModEntry.ContentPacks)
            _dropDownMenu.AddOption(pack.Manifest.Name, null);
        _dropDownMenu.AddOption(I18n.String_CombinedData(), null);

        UpdateDataByHat(hat);

        var buttons = HatDataHelper.AllHatData.Keys
            .Select(h => new ItemButton<Hat>(h))
            .ToList();

        foreach (var button in buttons)
            button.OnPress += () => UpdateDataByHat(button.Item!);

        _hatGridMenu.AddComponents(buttons);

        // Add components to menu
        AddChild(_hatGridMenu);
        AddChild(_hatIcon);
        AddChild(_hatName);
        AddChild(_hatDesc);
        AddChild(_textPanel);
        AddChild(_statPanel);
        AddChild(_dropDownMenu);
    }

#nullable enable

    /// <summary>
    /// Update the stat and text panel with the selected hat. Include
    /// all the data from owned content packs. This method is called
    /// when first opened and when click the hat button in the
    /// left-side grid menu.
    /// </summary>
    private void UpdateDataByHat(Hat hat)
    {
        _hatIcon.Item = hat;
        var dataList = hat.GetHatData();
        var menuSelected = _dropDownMenu.SelectedValue;

        var packs = ModEntry.ContentPacks;
        var i = 0;
        for (; i < packs.Count; i++)
        {
            var id = packs[i].Manifest.UniqueID;
            var d = dataList.FirstOrDefault(d => d.UniqueBuffID == id);
            _dropDownMenu.Options[i].Value = d;
        }

        // combine the data from all the content packs, and assign to the last option
        var combinedData = HatData.Combine(dataList);
        _dropDownMenu.Options[i].Value = combinedData;

        var data = menuSelected is null // null means the menu is first opened 
            ? dataList.FirstOrDefault() // so we use the first hat data temporarily, can be null
            : menuSelected.UniqueBuffID != HatData.CombinedDataSign // check for combined data
                ? dataList.FirstOrDefault(d => d.ID == menuSelected.ID) ?? null // no data found, return null
                : combinedData; // use combined data
        UpdateDataByData(data);

        // it's so weired that the first time we open the menu, the hat desc is null
        var rawDesc = hat.description ?? hat.LoadDescription();
        var desc = Game1.parseText(rawDesc, Game1.smallFont, 640);
        _hatName.Text = hat.DisplayName;
        _hatDesc.Text = desc;
    }

    /// <summary>
    /// Update the stat and text panel with specific data from the
    /// selected content pack. This method is called when click the
    /// drop-down menu to change target content pack and when the
    /// method <see cref="UpdateDataByHat"/> is called.
    /// </summary>
    private void UpdateDataByData(HatData? data)
    {
        _statPanel.UpdateStats(data);
        _textPanel.UpdateData(data);
    }
}