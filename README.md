# Rivet

**A simple package manager for C++ projects.**

Rivet makes adding and managing C++ libraries easier. Instead of manually cloning repositories, configuring dependencies, and modifying your CMake setup, Rivet handles the repetitive parts for you.

## What is Rivet?

C++ dependency management can quickly become messy.

Adding a library often means finding the repository, cloning the correct version, figuring out where it belongs, and updating your build system.

Rivet aims to make that workflow simpler.

```bash
rivet init
rivet add glfw
rivet add glm
```

Rivet uses a package registry containing information about supported libraries and how they should be retrieved and integrated into a project.

## Features

* Initialize Rivet inside an existing C++ project
* Add libraries using simple package names
* Central package registry
* Git-based dependency retrieval
* CMake integration
* Version information stored with packages
* Simple `rivet.json` project configuration
* Extensible registry for adding more C++ libraries

## Getting Started

Initialize Rivet in your project:

```bash
rivet init
```

This creates a `rivet.json` file containing your project's Rivet configuration.

Add a dependency:

```bash
rivet add glfw
```

Or:

```bash
rivet add glm
```

Rivet looks up the package in its registry, retrieves the dependency, and configures it for the project.

## Example

A Rivet project might look like this:

```text
MyGame/
|- CMakeLists.txt
|- rivet.json
|- rivet.cmake
|- src/
│   └── main.cpp
|-
└── ...
```

Instead of manually setting up every dependency, you can install registered packages through Rivet:

```bash
rivet add glfw
rivet add glm
```

## Package Registry

Rivet uses a shared package registry to define available packages.

A registry entry contains information such as:

```json
{
  "name": "glfw",
  "description": "Multi-platform library for OpenGL",
  "repository": "https://github.com/glfw/glfw.git",
  "version": "3.5.1"
}
```

This gives Rivet a predictable source of information for installing and managing libraries.

The long-term goal is for developers to be able to contribute new package definitions so that once a package is added to the registry, it becomes available to other Rivet users.

## Commands

```bash
rivet init
```

Initializes Rivet in the current project.

```bash
rivet add <package>
```

Adds a package from the Rivet registry to the current project.

Example:

```bash
rivet add glfw
```

More commands will be added as Rivet develops.

## Philosophy

Rivet is designed around a few simple ideas:

**Simple commands.** Installing a C++ library should not require a complicated workflow.

**Predictable packages.** Package definitions explicitly describe where dependencies come from and how Rivet should handle them.

**C++ focused.** Rivet is being designed specifically around the realities of C++ projects and CMake rather than trying to be a universal package manager.

**Community extensible.** Developers should be able to add support for libraries that Rivet does not already know about.

## Project Status

Rivet is currently under active development.

The command structure, registry format, dependency handling, and CMake integration may change as the project evolves.

It is not yet intended for production use.

## Roadmap

Planned areas of development include:

* More packages in the Rivet registry
* Package version selection
* Package removal
* Package updates
* Improved CMake generation and integration
* Dependency conflict handling
* Custom package definitions
* Registry contribution tooling
* Better error handling and diagnostics

## Contributing

Rivet is still early in development, but contributions, package definitions, bug reports, and ideas are welcome.

If you want to add a package, improve Rivet's package handling, or fix an issue, open an issue or pull request.

## License

License information will be added as the project develops.

This readme was written by ai for short term and will be rewritten for the first release.

---

Built to make C++ dependencies less painful.
