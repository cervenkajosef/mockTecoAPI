using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using mockTecoAPI;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication("Basic")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Basic", policy =>
    {
        policy.AuthenticationSchemes.Add("Basic");
        policy.RequireAuthenticatedUser();
    });
});

//var certificatePath = Environment.GetEnvironmentVariable("CERTIFICATE_PATH");
//var certificatePassword = Environment.GetEnvironmentVariable("CERTIFICATE_PASSWORD");

//if (certificatePath == null)
//    throw new ArgumentNullException(nameof(certificatePath));

//var certificate = new X509Certificate2(certificatePath, certificatePassword);

//builder.WebHost.ConfigureKestrel((context, options) =>    
//{
//    options.ListenAnyIP(443, listenOptions =>
//    {
//        listenOptions.UseHttps(certificate);
//    });
//});

builder.Services.AddAuthentication(
        CertificateAuthenticationDefaults.AuthenticationScheme)
    .AddCertificate();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseCertificateForwarding();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
