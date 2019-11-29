## UXML List

Adds a `ListVisualElement` that can be used for displaying properties that are lists or arrays.

Simply add the `ListVisualElement` to your hierachy and set the binding path to a list or array.

Can be added via UI Builder or via code.

For information in installing custom packages see main repo [readme](https://github.com/Sibz/Sibz.UnityPackages)

#### Sample
```
public class TestListBehaviour : MonoBehaviour
{
    public List<string> MyList = new List<string>();
}
```
Once you have that code set up, and a custom inspector, set the binding path of the `ListVisualElement` to the property path, in this case it would simply be `MyList`.

<a href="https://imgbb.com/"><img src="https://i.ibb.co/Pgb1F9G/MyList.png" alt="MyList" border="0"></a>

*Note: You have to create a custom inspector or a property drawer in order to use UI Elements, this is out side the scope of this readme however you can find more info here: https://docs.unity3d.com/Manual/UIElements.html*

#### Property Drawers
Using property drawers enables displaying the list without writing a whole custom inspector. However unity does not allow you to make a property drawer for lists or array. To work around this you can create a class that contains a list, and create a property drawer for that class. I have created such a class in the EditorList package, you can extend that and treat it just like a normal list.  Just set the binding to the inner `List` property rather than the class, i.e. `MyEditorList.List`.

### Customising
You can create styles rules for every component of the list - they all have there own class. Additionally using properties, you can hide any of the buttons and change their text (even to blank in order to use an image).

#### USS Classes
 - `sibz-list-header-section` - Encompasses header label, cleal list button and add button
 - `sibz-list-header-label` - The label at the top of the list 
 - `sibz-list-delete-all-button` -The clear list / delete all button
 - `sibz-list-add-button` - The add button
 - `sibz-list-add-object-field` - The add object field button
 - `sibz-list-delete-all-confirm-section` - Encompasses confirm label and yes/no buttons
 - `sibz-list-delete-confirm-label` - Confirmation label
 - `sibz-list-delete-yes-button` - Yes button
 - `sibz-list-delete-no-button` - No Button
 - `sibz-list-items-section` - Encompasses the list items
 - `sibz-list-item-section` - Encompasses item property field, delete and re-order buttons
 - `sibz-list-item-property-field` - Item property field
 - `sibz-list-delete-item-button` - Delete item button
 - `sibz-list-move-up-button` - Move up button
 - `sibz-list-move-down-button` - Move down button
 
 #### Properties
 
 These can be set in the UI Builder or via code/uxml, hopefully most the names are self explainitory.
 
 ```
string Label
bool DisableLabelContextMenu /* Properties field labels automatically get a context menu
                              * that enables removing/duplicating elements
                              * as it isn't easy to intercept these events, they can be disabled */
bool DisablePropertyLabel    /* Disables the label for the property field in the list */
bool HideAddButton
bool UseAddObjectField    /* When list item is subclass of Unity.Object, use an object field 
                            * instead of the add button */
bool HideDeleteAllButton
bool HideDeleteItemButton
bool HideReorderItemButtons
string AddButtonText 
string AddObjectFieldText 
string DeleteAllButtonText
string DeleteAllConfirmLabelText 
string DeleteAllYesButtonText
string DeleteAllNoButtonText 
string DeleteItemButtonText
string ReorderItemUpButtonText
string ReorderItemDownButtonText
```
