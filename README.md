# FarmerMarket API Serverless Application

This project  run as a ASP.NET Core WebAPI serverless application.  

It is accessible @ https://cixvvde8v7.execute-api.us-east-1.amazonaws.com/Prod/swagger/ui/index.html

# FarmerMarket Client
This project  run as a ASP.NET Core Web serverless application with Razor Pages. 


It can be accessed @ https://pch9v5w8z5.execute-api.us-east-1.amazonaws.com/Prod

From the UI, You just need to enter the product code in the text box and onchange event will take 
care of giving the cart value as real time as possible instead of waiting till we click a button.
 
I have used swagger end points for the API to make testing of the API simpler.
For Testing, I have used XUNIT & Moq Frameworks.
I have written tests for Controller and Service Methods as two Seperate categories

### Adding AWS SDK for .NET ###

To integrate the AWS SDK for .NET with the dependency injection system built into ASP.NET Core add the NuGet 
package [AWSSDK.Extensions.NETCore.Setup](https://www.nuget.org/packages/AWSSDK.Extensions.NETCore.Setup/). Then in 
the `ConfigureServices` method  in `Startup.cs` file register the AWS service with the `IServiceCollection`.
 
### Configuring for API Gateway HTTP API ###

API Gateway supports the original REST API and the new HTTP API. In addition HTTP API supports 2 different
payload formats. When using the 2.0 format the base class of `LambdaEntryPoint` must be `Amazon.Lambda.AspNetCoreServer.APIGatewayHttpApiV2ProxyFunction`.
For the 1.0 payload format the base class is the same as REST API which is `Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction`.
**Note:** when using the `AWS::Serverless::Function` CloudFormation resource with an event type of `HttpApi` the default payload
format is 2.0 so the base class of `LambdaEntryPoint` must be `Amazon.Lambda.AspNetCoreServer.APIGatewayHttpApiV2ProxyFunction`.


## Packaging as a Docker image.

This project is configured to package the Lambda function as a Docker image. The default configuration for the project and the Dockerfile is to build 
the .NET project on the host machine and then execute the `docker build` command which copies the .NET build artifacts from the host machine into 
the Docker image. 

The `--docker-host-build-output-dir` switch, which is set in the `aws-lambda-tools-defaults.json`, triggers the 
AWS .NET Lambda tooling to build the .NET project into the directory indicated by `--docker-host-build-output-dir`. The Dockerfile 
has a **COPY** command which copies the value from the directory pointed to by `--docker-host-build-output-dir` to the `/var/task` directory inside of the 
image.

Alternatively the Docker file could be written to use [multi-stage](https://docs.docker.com/develop/develop-images/multistage-build/) builds and 
have the .NET project built inside the container. Below is an example of building the .NET project inside the image.

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FarmerMarketAPI.CheckoutSystem/FarmerMarketAPI.CheckoutSystem.csproj", "FarmerMarketAPI.CheckoutSystem/"]
RUN dotnet restore "FarmerMarketAPI.CheckoutSystem/FarmerMarketAPI.CheckoutSystem.csproj"
COPY . .
WORKDIR "/src/FarmerMarketAPI.CheckoutSystem"
RUN dotnet build "FarmerMarketAPI.CheckoutSystem.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FarmerMarketAPI.CheckoutSystem.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
```

### Configuring for Application Load Balancer ###

To configure this project to handle requests from an Application Load Balancer instead of API Gateway change
the base class of `LambdaEntryPoint` from `Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction` to 
`Amazon.Lambda.AspNetCoreServer.ApplicationLoadBalancerFunction`.

### Project Files ###

* serverless.template - an AWS CloudFormation Serverless Application Model template file for declaring your Serverless functions and other AWS resources
* aws-lambda-tools-defaults.json - default argument settings for use with Visual Studio and command line deployment tools for AWS
* LambdaEntryPoint.cs - class that derives from **Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction**. The code in 
this file bootstraps the ASP.NET Core hosting framework. The Lambda function is defined in the base class.
Change the base class to **Amazon.Lambda.AspNetCoreServer.ApplicationLoadBalancerFunction** when using an 
Application Load Balancer.
* LocalEntryPoint.cs - for local development this contains the executable Main function which bootstraps the ASP.NET Core hosting framework with Kestrel, as for typical ASP.NET Core applications.
* Startup.cs - usual ASP.NET Core Startup class used to configure the services ASP.NET Core will use.


## Here are some stepsI followed from Visual Studio:

To deploy Serverless application, right click the project in Solution Explorer and select *Publish to AWS Lambda*.

To view deployed application open the Stack View window by double-clicking the stack name shown beneath the AWS CloudFormation node in the AWS Explorer tree. The Stack View also displays the root URL to your published application.

## Here are some steps to follow to get started from the command line:

Once you have edited your template and code you can deploy your application using the [Amazon.Lambda.Tools Global Tool](https://github.com/aws/aws-extensions-for-dotnet-cli#aws-lambda-amazonlambdatools) from the command line.

Install Amazon.Lambda.Tools Global Tools if not already installed.
```
    dotnet tool install -g Amazon.Lambda.Tools
```

If already installed check if new version is available.
```
    dotnet tool update -g Amazon.Lambda.Tools
```

Execute unit tests
```
    cd "FarmerMarketAPI.Client/test/FarmerMarketAPI.Client.Tests"
    dotnet test
```

Deploy application
```
    cd "FarmerMarketAPI.Client/src/FarmerMarketAPI.Client"
    dotnet lambda deploy-serverless
```
