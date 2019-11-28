# Sibz.UnityPackages
Group of generic packages to use in unity

To install any package you can 
 1. Use a tarball from https://pm.entityzero.com/ (Does not get dependencies)
 2. Install using the package manager (this will get dependencies)
 
#### Package Manager Setup

Add a scoped registry in scopedRegistries section of manifest.json in your project packages folder. Add the whole scopedRegistries if you don't have it. 
Currently Package manager isn't looking up packages, so you need to add it here manuall - but you can update it later from package manager. (note unity PM doesn't support the full version systax that npm does).
```
  "scopedRegistries": [
    {
      "name": "Main",
      "url": "https://pm.entityzero.com",
      "scopes": [
        "com.sibz"
      ]
    }
   ],
  "dependencies": {
    "com.sibz.uxml-list": "1.1.0"
  }
```
For more information on scoped registries, see https://docs.unity3d.com/Manual/upm-scoped.html
