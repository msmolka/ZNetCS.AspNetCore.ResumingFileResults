# ZNetCS.AspNetCore.ResumingFileResults

A small package to allow using resume during transfering data over MVC application in ASP.NET Core. 
It allows provide `Etag` header as well as 'Last-Modified` one. It also supports following precodition headers: `If-Match`, `If-None-Match`, `If-Modified-Since`, `If-Unmodified-Since`, `If-Range`.


## Installing 

Install using the [ZNetCS.AspNetCore.ResumingFileResults NuGet package](https://www.nuget.org/packages/ZNetCS.AspNetCore.ResumingFileResults)

```
PM> Install-Package ZNetCS.AspNetCore.ResumingFileResults
```

##Usage 

When you install the package, it should be added to your `package.json`. Alternatively, you can add it directly by adding:


```json
{
  "dependencies" : {
    "ZNetCS.AspNetCore.ResumingFileResults": "1.0.0"
  }
}
```

In order to use the ResumingFileResults, you must configure the services in the `ConfigureServices` call of `Startup`: 

```csharp
using ZNetCS.AspNetCore.ResumingFileResults.DependencyInjection;
```

```
...
```

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddResumingFileResult();
}
```

Then in your controller you can use it in similar way as build in `FileResult` helpers.

```csharp
using ZNetCS.AspNetCore.ResumingFileResults.Extensions;
```

```
...
```

```csharp
public IActionResult FileContents()
{
    string webRoot = this.hostingEnvironment.WebRootPath;
    var contents = System.IO.File.ReadAllBytes(Path.Combine(webRoot, "TestFile.txt"));

    return this.ResumingFile(contents, "text/plain", "TestFile.txt");
}

...

public IActionResult FileStream()
{
    string webRoot = this.hostingEnvironment.WebRootPath;
    FileStream stream = System.IO.File.OpenRead(Path.Combine(webRoot, "TestFile.txt"));
    return this.ResumingFile(stream, "text/plain", "TestFile.txt");
}

...
       
public IActionResult PhysicalFile()
{
    string webRoot = this.hostingEnvironment.WebRootPath;
    return this.ResumingPhysicalFile(Path.Combine(webRoot, "TestFile.txt"), "text/plain", "TestFile.txt");
}

...
    
public IActionResult VirtualFile()
{
    return this.ResumingFile("TestFile.txt", "text/plain", "TestFile.txt");
}
```

Above examples will serve your data as `Content-Disposition: attachment`. When fileName is not provided then data is served as `Content-Disposition: inline`.
Additionaly it is possible provide `Etag` and `LastModified` headers.

```csharp
public IActionResult File()
{
    return new ResumingVirtualFileResult("TestFile.txt", "text/plain", "\"MyEtagHeader\"") { FileDownloadName = "TestFile.txt", LastModified = DateTimeOffset.Now };
}
```


