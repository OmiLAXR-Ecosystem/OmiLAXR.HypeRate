# OmiLAXR.HypeRate

This module is using HypeRate SDK for measuring heart rate and is providing the values to OmiLAXR pipeline. 

To use it just drag `HypeRateProvider` to the target actor pipeline and attach it to the `Heart Rate Monitor` component (see example in Prefab).

## Compatibility

Because of the less dependencies and modular design of OmiLAXRv2 this framework is compatible with all XR frameworks. It doesnt matter if you use MRTK, VRTK, SteamVR, UnityXR, ... and it doesnt matter what equipment you use.

You can look for existing modules tracking the behaviour you need. The rest can be easily implemented by you own - and we hope you will share it with the community!

The only thing we need to consider are the Unity version. We try to support as much as possible upwards of Unity 2019.4.40f1. You are invited to contribute!

The compatibility was tested on

- [ ]  Unity 2019.4.40f1 (LTS)
- [X]  Unity 2020.3.48f1 (LTS)
- [X]  Unity 2021.3.15f1 (LTS)
- [X]  Unity 2021.3.45f1 (LTS)
- [X]  Unity 2022.3.52f1 (LTS)
- [X]  Unity 2023.2.20f1 (no LTS)
- [X]  Unity 6000.0.25f1 (LTS)

Please contact us on https://discord.gg/yJgk4ATg if it is not running on your Unity version. We will try to make it possible.
## Module dependencies
The adapter has following third-party dependencies attached:

- `HypeRate`: We adjusted and attached the source files of the SDK https://github.com/HypeRate/HypeRate-Unity/tree/master/Assets/HypeRate/HypeRate%20Heart%20Rate%20SDK. No installations are needed.
- `Supported Device`: Install on the device of your choice the `HypeRate` app: https://www.hyperate.io/downloads
- `API Key`: Get an API key from https://www.hyperate.io/api. It is free for research and indie projects.

## Install by using scoped registry
1. Ensure in "Project settings" > "Package Manager" that you have the scoped registry with following settings:
  - Name: npmjs
  - URL: http://registry.npmjs.com
  - Scope(s): `com.rwth.unity.omilaxr.hyperate`
2. Go to Package Manager.
3. Click on the (+) button.
4. Select 'Add package by name'.
5. Place in 'Name' field: `com.rwth.unity.omilaxr.hyperate`.

### Adding scoped registry by using manifest.json (also recommended - quick way)
1. Alternatively, instead of adding the scoped registry inside Unity editor you can do it by using `manifest.json` file.
2. Go to you project root and then open `Packages/manifest.json`.
3. Ensure following entries in your file: `"scopedRegistries": [
   {
   "name": "npmjs",
   "url": "http://registry.npmjs.com/",
   "scopes": [ 
   "com.rwth.unity.omilaxr",
   "com.rwth.unity.omilaxr.hyperate"
   ]
   }]`.
4. Go to the Unity Package Manager to `My registries` and install the package `com.rwth.unity.omilaxr.hyperate`.


## Install by using Git url
1. Go to Package Manager.
2. Click on the (+) button.
3. Select 'Add package from git URL'.
4. Paste `https://gitlab.com/learntech-rwth/omilaxr-ecosystem/v2/heart-rate/omilaxr.hyperate.git` and confirm.

## For Developers

To work with this package we recommend to place it somewhere outside your Unity project (if the package gets an own git repository) or in root of your project.
Than, you can include the package into your project by going to `Window > Package Manager`, click on `(+)` button and finally import the `package.json` of this project by clicking on `Add package from disk`.

For production use we recommend to use `Add package form git URL` or using scoped registries (see below).

## Publication

You can publish your package at any npm registry.
It makes sense to publish packages for easier distribution in other projects.
But we recommend to use `npmjs.com`. [Here](https://docs.npmjs.com/creating-and-publishing-scoped-public-packages) you can get more details.
But the steps are very easy.

1. Create an account on `npmjs.com`.
2. On demand increase your `version` number in `package.json`.
3. Commit and push your changes.
4. Open a terminal.
5. Go to the root of your project.
6. Run `npm login` and login via browser (or what else you like).
7. Run `npm publish --access public`.
8. Wait until publication is ready.
