#addin "Cake.WebDeploy"

var password = Argument("password", "");
var task = Argument("task", "Default");
var username = Argument("username", "");

Task("Production")
    .Description("Deploy to Azure using a custom Url")
    .Does(() =>
    {
        var settings = new DeploySettings()
        {
            Password = password.Replace("<semicolon>", ";"),
            PublishUrl = "https://softveloper.com:8172/msdeploy.axd",
            SiteName = "QrCo3ds",
            SourcePath = @"..\Publish",
            UseAppOffline = true,
            Username = username,
        };
        DeployWebsite(settings);
        Console.WriteLine(DateTime.Now);
    });

Task("UAT")
    .Description("Deploy to Azure using a custom Url")
    .Does(() =>
    {
        var settings = new DeploySettings()
        {
            Password = password.Replace("<semicolon>", ";"),
            PublishUrl = "https://demosoft.me:8172/msdeploy.axd",
            SiteName = "QrCo3ds",
            SourcePath = @"..\Publish",
            UseAppOffline = true,
            Username = username,
        };
        DeployWebsite(settings);
        Console.WriteLine(DateTime.Now);
    });

Task("Default")
    .Does(() => {
        Console.WriteLine("Default");
    });

RunTarget(task);
