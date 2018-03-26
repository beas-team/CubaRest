# CubaRest 

> C# library for [Cuba](https://www.cuba-platform.com/) [REST API](http://files.cuba-platform.com/swagger/6.8/)

### Prerequisites
* C# 7
* Add [RestSharp](https://github.com/restsharp/RestSharp) as NuGet package
* [Cuba](https://www.cuba-platform.com/) 6.8 or higher at server side

## Usage example

### Connecting to Cuba
```
string endpoint = "http://your-cuba-server/app/rest/v2/";
string basicUsername = "client";
string basicPassword = "secret";
string username = "admin";
string password = "admin";

CubaRestApi api = new CubaRestApi(endpoint, basicUsername, basicPassword, username, password);
YourEntityType entities = api.ListEntities<YourEntityType>();
```
Normally username and password should not be stored at client side.

Instead of that save refreshToken and initialize CubaRestApi like this:
```
string refreshToken = "b83f472c-b51f-4271-9abb-c37f4ce5a883";
CubaRestApi api = new CubaRestApi(endpoint, basicUsername, basicPassword, refreshToken);
```
If unauthorized you can create CubaRestApi object without refreshToken and obtain refreshToken 
later with username and password provided by user
```
var refreshToken = api.RequestRefreshToken(username, password);
```

### Manual client authenification
If CubaRest has no refreshToken during request, instead of throwing exception immediately it will try to call RequestCredentials() delegate first.

Use it to ask user to enter his username and password. "shouldContinue" parameter determines whether request should be continued normally or cancelled.

Please note that call from CubaRest comes in syncronious mode. RequestCredentials() should pause current thread until username and password 
are provided by user. You can use tecnique based on TaskCompletionSource for that:
```
using System.Threading.Tasks;

api.RequestCredentials = MyUIClass.RequestCredentials;

private (string, string, bool) MyUIClass.RequestCredentials(CubaRestApi.RequestCredentialsReason reason,
                                                            string usernameCached = null,
                                                            string passwordCached = null)
{
    string message = null;
    switch(reason)
    {
        case CubaRestApi.RequestCredentialsReason.IncorrectCredentials:
            message = "Incorrect password"; break;
    }

    return Task.Run(() =>
    {
        var tcs = new TaskCompletionSource<(string, string, bool)>();

        RunOnUiThread(() => {
			var dialog = new MyAuthDialog() { ErrorMessage = message, Username = usernameCached, Password = passwordCached };
			dialog.Closed += (username, password, shouldContinue) => { tcs.SetResult((username, password, shouldContinue)); };
			dialog.Show();
        });

        return tcs.Task;
    }).Result;
}   
```


### List entities of specific type
```
var result = api.ListEntities<YourEntityType>();
```

### Get entity by id
```
var result = api.GetEntity<YourEntityType>(entityId);
```

### Reflection
List server types with optional prefix in name
```
List<EntityType> types = api.ListTypes(prefix);
```
Get metadata on particular server type
```
EntityType typeMeta = api.GetTypeMetadata(typeName);
```
List server enums with optional prefix in name
```
List<EnumType> enums = api.ListEnums(prefix);
```
Get metadata on particular server enum
```
EnumType enumMeta = api.GetEnumMetadata(enumName);
```


## Entity type example
* Each entity type should be derived from Entity class and placed inside "prefix" class.
* Each entity type should be annotated with CubaName attribute, which sets the connection between client-side class and Cuba entity type.
* Each entity field should be annotated with Description attribute, which value corresponds to property name in Cuba's metadata.

Description attribute values can then be used in client application UI.

Comments are mostly for additional convenience with IntelliSense
```
public class Sys {
	[CubaName("sys$Config")]
	public class Config : Entity
	{
	   /// <summary>Config.createdBy</summary>
	   [Description("Config.createdBy")]
	   public string CreatedBy { get; set; }

	   /// <summary>Config.createTs</summary>
	   [Description("Config.createTs")]
	   public DateTime CreateTs { get; set; }

	   /// <summary>ID</summary>
	   [Description("ID")]
	   public string Id { get; set; }

	   /// <summary>Название</summary>
	   [Description("Название")]
	   public string Name { get; set; }

	   /// <summary>Config.updatedBy</summary>
	   [Description("Config.updatedBy")]
	   public string UpdatedBy { get; set; }

	   /// <summary>Config.updateTs</summary>
	   [Description("Config.updateTs")]
	   public DateTime UpdateTs { get; set; }

	   /// <summary>Значение</summary>
	   [Description("Значение")]
	   public string Value { get; set; }

	   /// <summary>Config.version</summary>
	   [Description("Config.version")]
	   public int Version { get; set; }
	}
}
```

## Codegeneration and tests
[CubaRest.Codegenerator](https://github.com/beas-team/CubaRest.Codegenerator) can help you in creating project-specific client-side entities model.

Tests can be found in separate repository: [CubaRest.Tests](https://github.com/beas-team/CubaRest.Tests)
Use them to compare project-specific Cuba entities model with your client-side entities model as well.

Unlike CubaRest itself CubaRest.Tests and CubaRest.Codegenerator are not intended to be included into final product code.

## Built With
* [RestSharp](https://github.com/restsharp/RestSharp)

## License

This project is licensed under the Apache License 2.0.

## Meta

Sergey Larionov

https://github.com/Zidar