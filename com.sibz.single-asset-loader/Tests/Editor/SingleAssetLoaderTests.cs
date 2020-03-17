using System;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Sibz.Tests
{
    public class SingleAssetLoaderTests
    {
        private const string TestAssetName = "TestGameObject";
        private GameObject go;

        [Test]
        public void ShouldLoadAssetFromNameAndType()
        {
            Type type = typeof(GameObject);

            go = PrefabUtility.SaveAsPrefabAsset(Object.Instantiate(new GameObject(TestAssetName)), $"assets/{TestAssetName}.prefab");

            Object loadedAsset = SingleAssetLoader.Load(TestAssetName, type);

            Assert.AreEqual(type, loadedAsset.GetType());
            Assert.AreEqual(TestAssetName, loadedAsset.name);
        }

        [Test]
        public void ShouldThrowIfAssetDoesNotExist()
        {
            Type type = typeof(GameObject);

            bool errorThrown = false;
            try
            {
                SingleAssetLoader.Load(TestAssetName, type);
            }
            catch
            {
                errorThrown = true;
            }

            Assert.IsTrue(errorThrown);
        }

        [Test]
        public void GenericShouldLoadAssetFromNameAndType()
        {
            go = PrefabUtility.SaveAsPrefabAsset(Object.Instantiate(new GameObject(TestAssetName)), $"assets/{TestAssetName}.prefab");

            GameObject loadedAsset = SingleAssetLoader.Load<GameObject>(TestAssetName);

            Assert.IsNotNull(loadedAsset);
            Assert.AreEqual(TestAssetName, loadedAsset.name);
        }

        [TearDown]
        public void TearDown()
        {
            if (go != null)
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(go));
            }
        }
    }
}