### UXML List

Adds a `ListVisualElement` that can be used for displaying properties that are lists or arrays.

Simply add the `ListVisualElement` to your hierachy and set the binding path to a list or array.

Can be added via UI Builder or via code.

#### Sample
```
public class TestListBehaviour : MonoBehaviour
{
    public List<string> MyList = new List<string>();
}
```
Once you have that code set up, and a custom inspector, set the binding path of the `ListVisualElement` to the property path, in this case it would simply be `MyList`.

<a href="https://imgbb.com/"><img src="https://i.ibb.co/Pgb1F9G/MyList.png" alt="MyList" border="0"></a>

*Note: You have to create a custom inspector or a property drawer in order to use UI Elements, this is out side the scope of this readme howeer you can find more info here: https://docs.unity3d.com/Manual/UIElements.html*

#### Property Drawers
Using property drawers unables displaying the list without writing a whole custom inspector. However unity does not allow you to make a property drawer for lists or array. To work around this you can create a class that contains a list, and create a property drawer for that class. I have created such a class in the EditorList package, you can extend that and treat it just like a normal list.  Just set the binding to the inner `List` property rather than the class, i.e. `MyEditorList.List`.
