# List Element

List usable in UI Elements for the editor. Bound to a serialized property that is an array type, it will display the contents and enable additions, deletions, edits and redordering.
Each row uses a `PropertyField` so if you have a custom type in your list, you can simply create a `CustomDrawer` and the list will display it.

## Options

* `EnableDeletions` - Allows the list to be cleared or items to be individually removed
* `EnableAdditions` - Allows addition of items
* `EnableModify` - Allows items to be edited
* `EnableReordering` - Allows items to be moved up or down

Once imported into the project simply add the ListElement to your uxml file using UI Builder and set the property path. 

It can be done programatically using `new ListElement(serializedProperty)`. Parameterless contructor is also supported but serialized property will have to bound afterwards using `listElement.BindProperty(serializedProperty)`.

