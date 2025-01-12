# Permission Manager
# Quick Start
Permission Manager Core Project requires the Microsoft Entity Framework with Sqlite:
```
dotnet add core/PermissionManagerCore.csproj package Microsoft.EntityFrameworkCore.Sqlite --version 6.0.35
```
You can run the application from the root directory:
```
dotnet run --project app/app.csproj
```
**Or**
```
cd app
dotnet run
```
## Info 
PermissionManager is a class library intended for use primary by small portable server applications, such as game servers. The admin console implements a terminal interface using the PermissionManager Library to guide a user (most likely an administrator) through a series of menus to create, read, update, and delete users, groups, and permissions. Manual changes to a user's permissions should be done through a tool such as the admin console rather than a database viewer to ensure the desired result is achieved, developers are welcome to develop their own management interfaces based on the core if more features than the admin console provides are desired.
## Repo Structure
There are four projects(.csproj) in this repository, **core**, **admin_console**, **demo_app**, and **common**. The primary reason for their separation is to demonstrate how the core is intended to be used by implementors. Any classes which are not needed by application code are hidden via the internal keyword to the core assemblies. Application code simply interacts using the `PermissionManager` class.

**core** is the Permission Manager library. It exposes a static class with all the methods needed to manipulate permissions.

**admin_console** is a CLI application that implements the core with a series of menus that allow the user to manipulate their permissions database, this should be used when manually changing permissions for a server application.

**demo_app** is the hello world of this library, it shows how to very quickly setup permissions and get an application running.

**common** contains some helper classes.

## Database Schema
The image below is a look at the demo.db database that is created with the CLI app for demonstration purposes.
## Publishing
```
dotnet publish --self-contained True
```
## Developer Info
To create migrations for **core**:
```
dotnet ef migrations add <MIGRATION-NAME> --project core/PermissionManagerCore.csproj
```
## TODO
1. Possibly switch to exceptions rather than returning error codes.
2. Wrap all context managers in core with try catch and handle Sqlite exceptions.
3. Write tests which can simulate Sqlite exceptions to test against them.
4. Write tests to benchmark performance with large number of Users/Permissions
5. Remove demo.db from admin_console
6. Add cli args to admin console for db
7. Better documentation
8. C++ Bindings
9. Python Bindings
## Languages
- **C#**: All program code is written in C#
- **XML**: `.csproj` uses XML for project configuration

## Design Philosophy Q/A
```
Q: Should I check for a specific permission or membership to a group?

A: NEVER check for membership to a group, this is not a function of the API and for good reason. Imagine you have a webserver and you want to check if a user can upload data, and you have decided that only members of the admin group can upload data. While the server is running, the only way to allow a new user to upload data is to make them an admin. There is a very high likelihood that they do not need all of the other permissions that admins have, but you have no choice. The better solution is to create a permission such as canUploadData, and give the admins this permission. Now if a new user needs to upload data, we can give them the canUploadData permission directly without making them an admin.
```
```
Q: Why can you not remove a permission that is granted to a user via a group without removing them from the group?

A: Group members and their Permissions should be predictable and congruent. If you could be a member of a group while simultaneously having none of the permissions which the group allows, the membership would mean nothing. Consider lessening the number of permissions a group has if this is a frequent problem.
```