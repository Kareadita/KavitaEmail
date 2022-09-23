using System.Net.Http;
using Flurl.Http.Configuration;

namespace Skeleton.Misc;

public class UntrustedCertClientFactory : DefaultHttpClientFactory
{
    public override HttpMessageHandler CreateMessageHandler() {
        return new HttpClientHandler {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
    }
}