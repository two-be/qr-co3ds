## Cake Web Deploy

```csharp
Task("DeployUrl")
    .Description("Deploy to Azure using a custom Url")
    .Does(() =>
    {
        var settings = new DeploySettings()
        {
            SourcePath = "../angular2-template",
            PublishUrl = "https://<ip>:8172/msdeploy.axd",
            SiteName = "SiteName",
            Username = "User",
            Password = "Password"
        };
        DeployWebsite(settings);
    });
```

## Run

```
deploy.cmd
```