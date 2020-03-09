
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TestListBehaviour2 : MonoBehaviour
{
    public List<string> MyList = new List<string>() { "test" };
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