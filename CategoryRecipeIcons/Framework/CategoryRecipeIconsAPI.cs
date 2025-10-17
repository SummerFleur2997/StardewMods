namespace CategoryRecipeIcons.Framework;

public class CategoryRecipeIconsAPI : ICategoryRecipeIconsAPI
{
    /// <inheritdoc/>
    public void AddIcon(string key, string value) => CategoryDataHelper.OverrideSpriteIndex.Add(key, value);

    /// <inheritdoc/>
    public void EditIcon(string key, string value) => CategoryDataHelper.SpriteIndex[key] = value;

    /// <inheritdoc/>
    public void EditName(int key, string value) => CategoryDataHelper.OverrideNameIndex[key] = value;
}