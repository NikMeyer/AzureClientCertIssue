namespace Sample
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Server.Kestrel.Https;
    using Microsoft.Extensions.Hosting;

    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(
                webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().ConfigureKestrel(
                        ko =>
                        {
                            ko.ConfigureHttpsDefaults(co =>
                            {
                                // Allow client certs when used without a load balancer. When used behind a load balancer
                                // the certificate will not be on the TLS connection but injected via header.
                                co.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
                                co.AllowAnyClientCertificate();
                            });
                        });
                });
        }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
    }
}
