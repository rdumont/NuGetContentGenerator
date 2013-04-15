NuGet Content Generator
=======================

An MSBuild task to insert replacement tokens in C# files.

What does it do?
----------------

This tool helps you in creating NuGet packages by automating the process of converting *.cs into *.cs.pp files.
In the process, it creates the desired replacement tokens that can be used by NuGet when adding content files.

### Here is an example

Given the following directory structure:

```
MySolution:
- MyProject:
  - content:
    - SomeClassToPackage.cs
  - MyProject.csproj
- MySolution.sln
```

And that you have installed NuGet Content Generator in the `MyProject` project, when you build it, a new file called `SomeClassToPackage.cs.pp` will be created at the same level of `SomeClassToPackage.cs`, resulting in the following structure:

```
MySolution:
- MyProject:
  - content:
    - SomeClassToPackage.cs
    - SomeClassToPackage.cs.pp
  - MyProject.csproj
- MySolution.sln
```

> **Important**: the new *.pp files will NOT be added to the project, as they will only be created on your file system.

### Content transformations

NuGet supports some replacement tokens that are evaluated at package installation time. The most common is `rootnamespace`, although many others are available. More on this can be found in [the NuGet documentation](http://docs.nuget.org/docs/creating-packages/configuration-file-and-source-code-transformations#Specifying_Source_Code_Transformations "the NuGet documentation").

Let's say that you have the following `CategoryInfo.cs` file:

```csharp
/** @pp
 * rootnamespace: MySolution.SomeNamespace.Content
 */
namespace MySolution.SomeNamespace.Content.Models {
    public struct CategoryInfo {
        public string categoryid;
        public string description;
        public string htmlUrl;
        public string rssUrl;
        public string title;
    }
}
```

After the transformation, the symbols defined in the `@pp` comment section will be inserted where specified, resulting in the following `CategoryInfo.cs.pp` file:

```csharp
namespace $rootnamespace$.Models {
    public struct CategoryInfo {
        public string categoryid;
        public string description;
        public string htmlUrl;
        public string rssUrl;
        public string title;
    }
}
```

You can define as many replacements as you would like, one in each line. The only restriction is that the comment section for defining these replacements **must be** the first thing in the file.

How do I install it?
--------------------

You can install it from NuGet, by looking for the `NuGetContentGenerator` package or running the following command in the Package Manager Console:

    PM> Install-Package NuGetContentGenerator
    
The package will then be installed and the project will automatically import the needed MSBuild targets. All that is left to do is build your project to see every `content\**\*.cs` be transformed to its NuGet content representation as `content\**\*.cs.pp`.

Don't forget to add your content files to the Nuspec. Ideally, given the `.nuspec` is at the same level as the project, the content should be added as follows:

```xml
<files>
  <file src="content\**\*.pp" target="content" />
</files>
```
