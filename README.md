# hydra-basket
Repository for basket

This repository contains an Azure Function that update the Basket UI via external request.

The main feature implement is SignalR to send notification when the Basket is updated.

### GRPC packages:
- Grpc.AspNetCore

### GRPC configuration
In the csProject there is a specific key:
```cs
<ItemGroup>
    <Protobuf Include="Protos\basket.proto" GrpcServices="Server" />
</ItemGroup>
```

#### gRPC support
gRPC does not support to run in IIS, so you should run as SelfHosting.

##### Important:
If you are developing your project gRPC under another Operation system such MacOS, You will need to do few adjusts on the project, because I have stroggled few hours to figured out how to fix the error related to HTTP 2.0 communication error. You can add this code on the ```Program.cs``` file:
```cs
   if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                        webBuilder.ConfigureKestrel(options =>
                        {
                            // Setup a HTTP/2 endpoint without TLS.
                            options.ListenLocalhost(45055, o => o.Protocols = 
                                HttpProtocols.Http2);
                        });
                     }
```
###### This should not be used in production deployment, only for development and test propose. Also including this line will break other requests such HttpClient, so beware that using this solution is only to call gRPC functions, but in case that you need to test Rest API rather than gRPC, you have to get rid of this solution.
