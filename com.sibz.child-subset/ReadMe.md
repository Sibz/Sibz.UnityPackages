# ChildSubset

Returns a children of a GameObject. Can specify to return only certain components and/or a filtered set.
Can also sort on any `IComparable` field.

## Usage
```
var childSubset = new ChildSubset(rootGameObject);
var results = childSubset.List;
```
*Note: the list is updated every call to `List` or `Count` properties*

To select components instead of game objects use:
```
var childSubset = new ChildSubset<MyComponent>(rootGameObject)
```

### Filter
To set a filter:
```
var childSubset = new ChildSubset(rootGameObject) { Filter = x=>x.name.StartWith("Test")}
```
This would select all game objects whose name starts with 'Test'.filtered.

Note: the variable in the lambda will be what ever type is specified, in the above case `GameObject`,
however if you use `new ChildSubset<MyComponent>` then it would be `MyComponent`.

### Recursive
All queries are recursive by default, this can be disabled:
```
new ChildSubset(rootGameObject) { Recursive = false }
```

### Sort
```
new ChildSubset(rootGameObject) { Sort = x=>x.name }
```
This would sort by name. Queries are ascending by default. To change use:
```
new ChildSubset(rootGameObject) { Sort = x=>x.name, SortDirection = ChildSubset.SortDir.Descending }
```