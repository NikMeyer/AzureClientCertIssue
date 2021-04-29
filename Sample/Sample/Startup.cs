namespace Sample
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.Certificate;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Add certificate forwarding and authN/authZ
            app.UseCertificateForwarding();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();

            // Setup forwarding as certificates in Azure Web App are terminated by the frontend.
            services.AddCertificateForwarding(options => { options.CertificateHeader = "X-ARR-ClientCert"; });

            services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme).AddCertificate(
                options =>
                {
                    // Disable most validation options
                    options.ChainTrustValidationMode = X509ChainTrustMode.System;
                    options.RevocationMode = X509RevocationMode.NoCheck;
                    options.AllowedCertificateTypes = CertificateTypes.All;
                    options.ValidateCertificateUse = false;
                    options.ValidateValidityPeriod = false;

                    options.Events = new CertificateAuthenticationEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine($"OnAuthenticationFailed {context.Exception}");
                            context.Fail("Invalid Certification!");
                            return Task.CompletedTask;
                        },
                        OnCertificateValidated = context =>
                        {
                            var cert = context.ClientCertificate;
                            Console.WriteLine($"OnCertificateValidated {cert}");
                            context.Success();
                            return Task.CompletedTask;
                        }
                    };
                });
        }
    }
}
