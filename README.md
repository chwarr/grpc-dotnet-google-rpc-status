# Using google.rpc.Status with gRPC .NET

In the broad gRPC community, there is an as yet [_unspecified_
convention][doc-grpc-status-details-bin] that one way to include details
about failed requests is to serialize a
[`google.rpc.Status`][google-rpc-Status] message and put it in the response
trailer field `grpc-status-details-bin`. (Example implementations:
[C++][grpc-cpp], [Python][grpc-python], [Ruby][grpc-ruby]).

This repository shows one way to implement this convention using the [gRPC
.NET][grpc-dotnet] implementation.

Google publishes a NuGet package with a C# implementation of the
`google.rpc.Status` message,
[`Google.Api.CommonProtos`][google-api-commonprotos]. That package is used
in this example.

The most interesting directories are:

* `client`: client-side code that shows how to extract the detailed status
  from a failed request
* `service`: service-side code that fails requests with
  `StatusCode.FailedPrecondition` and additional details when the `Name`
  field in the request is missing. The interesting code is in the
  `GreeterService.cs` implementation. The rest is standard gRPC .NET
  bootstrapping.

# Running the example

To run this example, you will need to 

1. Clone the repository.
1. Build and run the service.
1. Build and run the client.

First, clone the repository and run the service:

```shell
mkdir example
cd example
# Notice the dot at the end of this command to clone into the current directory
git clone https://github.com/chwarr/grpc-dotnet-google-rpc-status.git .
cd service
dotnet run
```

Then, run the client from a different shell:

```shell
cd example/client
dotnet run
```

# Determining what status details an API may return

You are _likely_ safe assuming that `grpc-status-details-bin` is a Protocol
Buffer serialized `google.rpc.Status` message.

You will have to consult the documentation of the gRPC service methods you
are invoking to determine what messages to expect in the
`google.rpc.Status.details` field.

# Troubleshooting

If you try to use this approach in your own service and your don't see the
`grpc-status-details-bin` trailer on the client side, make sure both the
client and the server are using gRPC .NET 2.32 or later. Earlier versions
had a [bug that filtered out][filter-fix] this trailer inadvertently.

If the service cannot bind to port 5000 and 5001, it may fail to respond to
requests. The ports used can be adjusted by editing
`service/appsettings.json` _and_ `client/Program.cs`.

# Support

There is no support provided for this example code.

The existence of this example does not imply that Microsoft recommends or
does not recommend that this pattern be employed.

# Legal

This example is copyright 2020 Microsoft Corporation. It is released under
[the MIT license](./LICENSE.txt).

*Trademarks*: This project may contain trademarks or logos for projects,
products, or services. Authorized use of Microsoft trademarks or logos is
subject to and must follow [Microsoft's Trademark & Brand
Guidelines][msft-tbg]. Use of Microsoft trademarks or logos in modified
versions of this project must not cause confusion or imply Microsoft
sponsorship. Any use of third-party trademarks or logos are subject to those
third-party's policies.

[doc-grpc-status-details-bin]: https://github.com/grpc/grpc/issues/24007
[filter-fix]: https://github.com/grpc/grpc-dotnet/commit/139040e840bb03db7de3ff064b6f9629d5ce444c
[google-api-commonprotos]: https://www.nuget.org/packages/Google.Api.CommonProtos/
[google-rpc-Status]: https://github.com/googleapis/googleapis/blob/master/google/rpc/status.proto
[grpc-cpp]: https://github.com/grpc/grpc/blob/d6ad6d16d211a557451006bba4a501d430448c32/include/grpcpp/impl/codegen/call_op_set.h#L805
[grpc-dotnet]: https://github.com/grpc/grpc-dotnet/
[grpc-python]: https://github.com/grpc/grpc/blob/698a5a88695379e7f417e12856a875b51b94b9fe/src/python/grpcio_status/grpc_status/rpc_status.py#L64
[grpc-ruby]: https://github.com/grpc/grpc/blob/5a5105b89c1ebedf1a86dd185afcef64a9555f78/src/ruby/lib/grpc/errors.rb#L63-L75
[msft-coc-faq]: https://opensource.microsoft.com/codeofconduct/faq/
[msft-coc]: https://opensource.microsoft.com/codeofconduct/
[msft-tbg]: https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general
