namespace CategoryRecipeIcons.Framework;

public interface ICategoryRecipeIconsAPI
{
    /// <summary>
    /// Add a custom category and specify its icon.
    /// </summary>
    /// <param name="key">The category id to add, e.g. -77</param>
    /// <param name="value">The item id to show, e.g. (O)74</param>
    public void AddIcon(string key, string value);

    /// <summary>
    /// Edit the icon of an existing category.
    /// </summary>
    /// <param name="key">The category id you want to edit, e.g. -77</param>
    /// <param name="value">The item id to show, e.g. (O)74</param>
    public void EditIcon(string key, string value);

    /// <summary>
    /// Edit the display name of an existing category.
    /// </summary>
    /// <param name="key">The category id you want to edit, e.g. -77</param>
    /// <param name="value">The new display name to show, e.g., Magic Item</param>
    public void EditName(int key, string value);
}