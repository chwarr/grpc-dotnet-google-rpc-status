namespace grpc_dotnet
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using Google.Protobuf;
    using Google.Protobuf.WellKnownTypes;

    using Grpc.Core;

    using Microsoft.Extensions.Logging;

    public class GreeterService : Greeter.GreeterBase
    {
        // This is the conventional name for the status details trailer.
        private const string StatusDetailsTrailerName = "grpc-status-details-bin";

        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                FailBecauseNameMissing();
            }

            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        [DoesNotReturn]
        private static void FailWithDetails(StatusCode code, string topLevelMessage, IEnumerable<IMessage> details)
        {
            var status = new Google.Rpc.Status
            {
                Code = (int)code,
                Message = topLevelMessage,
            };
            status.Details.AddRange(details.Select(d => Any.Pack(d)));

            var failureMetadata = new Metadata();
            failureMetadata.Add(StatusDetailsTrailerName, status.ToByteArray());

            throw new RpcException(
                new Status(code, topLevelMessage),
                failureMetadata);
        }

        [DoesNotReturn]
        private static void FailBecauseNameMissing()
        {
            var badRequest = new Google.Rpc.BadRequest
            {
                FieldViolations =
                {
                    new Google.Rpc.BadRequest.Types.FieldViolation
                    {
                        Field = "name",
                        Description = "name is required",
                    },
                },
            };

            FailWithDetails(StatusCode.FailedPrecondition, "Required field missing", new[] { badRequest });
        }
    }
}
