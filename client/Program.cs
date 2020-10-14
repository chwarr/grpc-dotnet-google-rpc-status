namespace grpc_dotnet
{
    using System;
    using System.Threading.Tasks;

    using Google.Protobuf.WellKnownTypes;

    using Grpc.Core;
    using Grpc.Net.Client;

    public class Program
    {
        // This is the conventional name for the status details trailer.
        private const string StatusDetailsTrailerName = "grpc-status-details-bin";

        public static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);

            try
            {
                // This is expect to work.
                var reply = await client.SayHelloAsync(
                                  new HelloRequest { Name = "GreeterClient" });
                Console.WriteLine("Greeting: " + reply.Message);

                // This is expected to fail.
                reply = await client.SayHelloAsync(new HelloRequest());
                Console.WriteLine("Greeting: " + reply.Message);
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Failed: {ex.StatusCode}: {ex.Message}");
                PrintRpcExceptionDetails(ex);
            }
        }

        private static void PrintRpcExceptionDetails(RpcException ex)
        {
            byte[]? statusBytes = null;

            // There are nicer access methods in forthcoming versions of
            // gRPC#. For now, walk all the items and take the last one
            // that matches.
            foreach (Metadata.Entry me in ex.Trailers)
            {
                if (me.Key == StatusDetailsTrailerName)
                {
                    statusBytes = me.ValueBytes;
                }
            }

            if (statusBytes is null)
            {
                return;
            }

            var status = Google.Rpc.Status.Parser.ParseFrom(statusBytes);
            
            foreach (Any any in status.Details)
            {
                PrintAny(any);
            }
        }

        private static void PrintAny(Any any)
        {
            // fallback to printing the any directly, which will include the
            // type URL at a minimum
            object objToPrint = any;

            // additional message types can be added here
            if (any.TryUnpack(out Google.Rpc.BadRequest br))
            {
                objToPrint = br;
            }
            else if (any.TryUnpack(out Google.Rpc.PreconditionFailure pf))
            {
                objToPrint = pf;
            }

            Console.WriteLine($"  {objToPrint}");
        }
    }
}
