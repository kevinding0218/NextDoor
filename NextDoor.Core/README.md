# NextDoor.Core - class library project

## How to create the project
```
dotnet new classlib -n NextDoor.Core -o NextDoor.Core
dotnet sln add .\NextDoor.Core\NextDoor.Core.csproj
dotnet sln list
```
## Packages Included

### dev-01-mssql-repository-design
#### Newtonsoft.Json
```
dotnet add package Newtonsoft.Json
dotnet restore
```

#### Microsoft.EntityFrameworkCore
```
dotnet add package Microsoft.EntityFrameworkCore
dotnet restore
```

### dev-01-mongo-repository-design
#### Mongo.Driver
```
dotnet add package MongoDB.Driver
dotnet restore
```

### dev-03-webapi-core-design
#### Autofac
```
dotnet add package Autofac
dotnet restore
```

#### Microsoft.Extension.Configuration
```
dotnet add package Microsoft.Extensions.Configuration
// dotnet add package Microsoft.Extensions.Configuration.FileExtensions
// dotnet add package Microsoft.Extensions.Configuration.Json
dotnet restore
```

#### Microsoft.AspNetCore.Http
```
dotnet add package Microsoft.AspNetCore
dotnet add package Microsoft.AspNetCore.Mvc
dotnet add package Microsoft.AspNetCore.Mvc.Core
dotnet add package Microsoft.AspNetCore.Mvc.DataAnnotations
dotnet add package Microsoft.AspNetCore.Mvc.Formatters.Json
dotnet restore
```

#### Microsoft.Extensions
```
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet restore
```
-------------------------------------------------------
## Git Command
```
https://confluence.atlassian.com/bitbucketserver/basic-git-commands-776639767.html
```

### Show all remote and local branches
```
git branch -a
```

### figure out what branches are on your remote by actually using the remote related commands
```
git remote show origin
```

### Switch from one branch to another
```
git checkout <branchname>
```

### Create a new branch and switch to it:
```
git checkout -b <branchname>
```

### Rename a branch

- Rename your local branch.
* If you are on the branch you want to rename:
```
git branch -m new-name
```

* If you are on a different branch:
```
git branch -m old-name new-name

```
- Delete the old-name remote branch and push the new-name local branch.
```
git push origin :old-name new-name
```

- Reset the upstream branch for the new-name local branch.
* Switch to the branch and then:
```
git push origin -u new-name
```
-------------------------------------------------------
