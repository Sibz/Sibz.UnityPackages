
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TestListBehaviour : MonoBehaviour
{
    public List<string> MyList = new List<string>();
    public TestList MyList2 = new TestList();
}

[System.Serializable]
public class TestList
{
    public List<GameObject> MyObjectList = new List<GameObject>();
}
public class TestType : MonoBehaviour
{

}