# An Electron-Fable3 Minimum Framework

## Introduction

The application is mostly written in F#, which gets transpiled to JavaScript via the [Fable](https://fable.io/) compiler. [Electron](https://www.electronjs.org/) is then used to convert the developed web-app to a cross-platform application. [Electron](electronjs.org) provides access to platform-level APIs (such as access to the file system) which would not be available to vanilla browser web-apps.

[Webpack 4](https://webpack.js.org/) is the module bundler responsible for the JavaScript concatenation and automated building process: the electron-webpack build 
is automated with the all-in-one electron-webpack package.

## Project Structure

Electron bundles Chromium (View) and node.js (Engine), therefore as in every node.js project, the `package.json` file specifies the (Node) module dependencies.

* dependencies: node libraries that the executable code (and development code) needs
* dev-dependencies: node libraries only needed by development tools

Additionally, the section `"scripts"`:
```
 "scripts": {
    "compile": "dotnet fable src/main && dotnet fable src/renderer",
    "dev": "cd src/main && dotnet fable watch . --run npm run devrenderer",
    "devmain": "cd src/main && dotnet fable watch . --run npm run webpackdev",
    "devrenderer": "cd src/renderer && dotnet fable watch . --run npm run webpackdev",
    "webpackdev": "electron-webpack dev",
    "webpack": "electron-webpack",
    "dist": "npm run compile && npm run webpack &&  electron-builder",
  }
```
Defines the in-project shortcut commands as a set of `<key> : <value` lines, so that when we use `npm run <stript_key>` it is equivalent to calling `<script_value>`. 
For example, in the root of the project, running in the terminal `npm run dev` is equivalent to the command line:

```
cd src/main && dotnet fable watch . --run npm run devrenderer
```

This runs fable 3 to transpile the main process, then (`--run` is an option of fable to run another command) runs script `devrenderer` to transpile to javascript and watch the F# files in the renderer process. After the renderer transpilation is finished 
`electron-webpack dev` will be run. This invokes `webpack` to pack and lauch the javascript code, under electron, and also watches for changes in the javascript code, and *hot loads* these on the running application

As result of this, at any time saving an edited F# renderer project file causes (nearly) immediate:

* fable transpile to from F# to javascript file (dependent F# files may also be transpiled)
* webpack hot load of any changed javascript files to the running electron application

The build system depends on a `Fake` file `build.fsx`. Fake is a DSL written in F# that is specialised to automate build tasks. Build.fsx has targets representing build tasks, and normally these are run via `build.cmd` or `build.sh`, instead of using `dotnet fake` directly:

* `build <target>` ==> `dotnet fake build -t <target>`

## Code Structure

The source code consists of two distinct sections transpiled separately to Javascript to make a complete Electron application.

* The electron main process runs the Electron parent process under the desktop native OS, it starts the app process and provides desktop access services to it.
* The electron client (app) process runs under Chromium in a simulated browser environment (isolated from the native OS).

Electron thus allows code written for a browser (HTML + CSS + JavaScript) to be run as a desktop app with the additional capability of desktop filesystem access via communication between the two processes.

Both processes run Javascript under Node.

The `src/Main/Main.fs` source configures electron start-up and is boilerplate. It is transpiled to the root project directory so it can be automatically picked up by Electron.

The remaining app code is arranged in four different sections, each being a separate F# project. This separation allows all the non-web-based code (which can equally be run and tested under .Net) to be run and tested under F# directly in addition to being transpiled and run under Electron.

The code that turns the F# project source into `renderer.js` is the FABLE compiler followed by the Node Webpack bundler that combines multiple Javascript files into a single `renderer.js`.

The compile process is controlled by the `.fsproj` files (defining the F# source) and `webpack.additions.main.js`, `webpack.additions.renderer.js`
which define how Webpack combines F# outputs for both electron main and electron app processes and where the executable code is put. 
This is boilerplate which you do not need to change; normally the F# project files are all that needs to be modified.

## File Structure

### `src` folder

|   Subfolder   |                                             Description                                            |
|:------------:|:--------------------------------------------------------------------------------------------------:|
| `main/` | Code for the main electron process that sets everything up - not normally changed |
| `Renderer/`     | Contains the UI logic |

### `Tests` folder

Contains all tests.

### `Static` folder

Contains static files used in the application.  

## Build Magic

This project uses modern F# / dotnet cross-platform build. The build process does not normally concern a developer, but here is an overview for if it needs to be adjusted.

* Before anything can be built Dotnet & Node.js are manually be (globally) installed. Dotnet includes the `paket` tool which will manage other dotnet-related dependencies. Node.js includes `npm` which will do the same for Node-related dependencies. NB - there are other popular packet managers for Node, e.g. Yarn. They do not mix with npm, so make sure you do not use them. Confusingly, they will sort-of work, but cause install incompatibilities.
  * Dotnet dependencies are executable programs or libraries that run under dotnet and are written in C#'. F#, etc.
  * Node dependencies are (always) Javascript modules which run under node.
* Initially (the first time `build.cmd` is run) the build tools categorised in `dotnet-tools.json` are installed by `dotnet tool restore`.
   * fake (with the F# compiler) 
   * fable
* Next all the project Dotnet dependencies (`paket.dependencies` for the whole project, selected from by the `paket.references` in each project directory, are loaded by the `paket` packet manager.
* Finally fake runs `build.fsx` (this is platform-independent) which uses `npm` to install all the node (Javascript) dependencies listed in `package.json`. That includes tools like webpack and electron, which run under node, as well as the node libraries that will be used by needed by the running electron app, including electron itself. These are all loaded by the `npm` packet manager. 

## Install Prerequisites

Download and install (if you already have these tools installed just check the version constraints).

* [Dotnet Core SDK](https://www.microsoft.com/net/learn/get-started).  Version >= 3.1
    * For Mac and Linux users, download and install [Mono](http://www.mono-project.com/download/stable/) 
      from official website (the version from brew is incomplete, may lead to MSB error later). 

* [Node.js v12](https://nodejs.org/dist/latest-v12.x/). Version >= 12
    * Node.js includes the `npm` package manager, so this does not need to be installed separately.
    * The lastest LTS version of Node is now v14. That will almost certainly also work.

## Reinstalling Compiler and Libraries

To reinstall the build environment (without changing project code) rerun `build.cmd` (Windows) or `build.sh` (Linux and MacOS). You may need first to
run `build killzombies` to remove orphan processes that lock build files.


