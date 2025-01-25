# LabelsOnFloor_chinese

为<https://github.com/Falconne/LabelsOnFloor>添加了中文标签渲染的功能。

其实我不会写C#也不会Unity，但是好像对着代码直接改然后它就能跑，好耶！

没有打进去完整的中文字体，这是因为我发现Unity的单个纹理是有边长上限，改起来复杂一些，所以就先把rimworld里的常用字注入进去了。


## 以下是原作者的readme

To build, just build the .sln in VS 2017. A distribution zip will be created in a `dist` directory at the root of the repo.

Before a release, update `AssemblyInfo.cs` in the project directory to increase the patch number (3rd version field) of `AssemblyVersion` and `AssemblyFileVersion` by one. If the a Steam installation of RimWorld is detected, the major & minor version fields of `AssemblyInfo.cs` and the `targetVersion` field of `mod-structure\About\About.xml` will be updated automatically. If a Steam installation is not detected, these must be updated manually.

HugsLib dependencies are fetched via NuGet and the RimWorld and Unity assemblies will be copied out of the game's Steam directory if found. The output will automatically be copied into the game's mod directory if a Steam installation of the game is detected.

If you are not using Steam, create a `ThirdParty` directory at the root of the repository and copy `Assembly-CSharp.dll` and `UnityEngine.dll` from the game into there.
